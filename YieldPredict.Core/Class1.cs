namespace YieldPredict.Core.Entities;

// Clean Architecture'da Core katmanı, otel rezervasyonlarıyla ilgili iş kurallarını temsil eden saf domain modelini taşır; persistence ve ML detaylarından tamamen bağımsızdır.
public class HotelBooking
{
    public Guid Id { get; set; }

    public string Hotel { get; set; } = default!;

    public bool IsCanceled { get; set; }

    public int LeadTime { get; set; }

    public string ArrivalDateMonth { get; set; } = default!;

    public int Adults { get; set; }

    public int Children { get; set; }

    public string Meal { get; set; } = default!;

    public string MarketSegment { get; set; } = default!;

    public string ReservedRoomType { get; set; } = default!;

    public decimal Adr { get; set; }
}
