using System.Globalization;
using YieldPredict.Core.Entities;
using YieldPredict.Infrastructure.Persistence;

namespace YieldPredict.Infrastructure.Seeding;

public static class CsvDataSeeder
{
    public static void SeedFromCsv(ApplicationDbContext context, string csvFilePath)
    {
        // Idempotent seeding: In-Memory veritabanı zaten doluysa tekrar veri yazmayarak, uygulama her ayağa kalktığında deterministik başlangıç durumu korunur.
        if (context.HotelBookings.Any())
            return;

        // Operasyonel güvenilirlik adına, CSV dosyası bulunamazsa seeding sessizce atlanır; API'nin çalışabilirliği veri yükünden bağımsız tutulur.
        if (!File.Exists(csvFilePath))
            return;

        var lines = File.ReadAllLines(csvFilePath);

        if (lines.Length <= 1)
            return;

        var header = lines[0].Split(',');

        // CSV başlık satırı dinamik olarak indekslenerek, kolon adlarındaki küçük isim farklılıklarına karşı esnek bir mapping katmanı sağlanır.
        var headerIndex = header
            .Select((name, index) => new { name = name.Trim().ToLowerInvariant(), index })
            .ToDictionary(x => x.name, x => x.index);

        string? Get(string columnName, string[] values)
        {
            var key = columnName.Trim().ToLowerInvariant();
            if (!headerIndex.TryGetValue(key, out var idx))
                return null;

            if (idx < 0 || idx >= values.Length)
                return null;

            return values[idx].Trim();
        }

        int ParseInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
                ? result
                : 0;
        }

        decimal ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0m;

            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result)
                ? result
                : 0m;
        }

        bool ParseBool(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Trim().ToLowerInvariant();
            return value is "1" or "true" or "yes";
        }

        // Satır bazlı parse işlemi, hatalı kayıtları sessizce atlayarak seeding'in bütününü bozmayan "best-effort" veri yükleme stratejisi uygular.
        var bookings = new List<HotelBooking>();

        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = line.Split(',');

            try
            {
                var booking = new HotelBooking
                {
                    Id = Guid.NewGuid(),
                    Hotel = Get("hotel", values) ?? string.Empty,
                    IsCanceled = ParseBool(Get("is_canceled", values)),
                    LeadTime = ParseInt(Get("lead_time", values)),
                    ArrivalDateMonth = Get("arrival_date_month", values) ?? string.Empty,
                    Adults = ParseInt(Get("adults", values)),
                    Children = ParseInt(Get("children", values)),
                    Meal = Get("meal", values) ?? string.Empty,
                    MarketSegment = Get("market_segment", values) ?? string.Empty,
                    ReservedRoomType = Get("reserved_room_type", values) ?? string.Empty,
                    Adr = ParseDecimal(Get("adr", values))
                };

                if (!string.IsNullOrWhiteSpace(booking.Hotel))
                {
                    bookings.Add(booking);
                }
            }
            catch
            {
            }
        }

        if (bookings.Count > 0)
        {
            context.HotelBookings.AddRange(bookings);
            context.SaveChanges();
        }
    }
}
