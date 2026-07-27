using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;
using YieldPredict.API.ML;
using YieldPredict.API.Services;
using YieldPredict.Infrastructure.Persistence;
using YieldPredict.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

// API katmanının HTTP endpoint'lerini ve kendini dokümante eden (Swagger/OpenAPI) sözleşme yüzeyini kompoze eden composition root.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Geliştirme ve PoC senaryoları için In-Memory EF Core context; kalıcı veritabanı mimarisine geçişte Infrastructure katmanı değiştirilebilir tutulur.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("YieldPredictDb");
});

// PredictionEnginePool ile thread-safe ML model havuzu; model tekil .zip artefaktından okunur ve DI konteynerine application-level servis olarak enjekte edilir.
var modelPath = Path.Combine(builder.Environment.ContentRootPath, "OtelFiyatModeli.zip");

builder.Services
    .AddPredictionEnginePool<HotelData, HotelPrediction>()
    .FromFile(
        modelName: "HotelPriceModel",
        filePath: modelPath,
        watchForChanges: true);

// PredictionService, request-scope yaşam süresiyle controller'lara gevşek bağlı (loosely coupled) bir domain servis adaptörü olarak enjekte edilir.
builder.Services.AddScoped<PredictionService>();

var app = builder.Build();

// Yalnızca geliştirme ortamında Swagger UI'ı expose ederek, sözleşme odaklı API keşfi ve manuel test akışlarını destekler.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "YieldPredict V1");
        c.RoutePrefix = "swagger";
    });
}

// Yerel geliştirme senaryolarında HTTP üzerinden kolay test için HTTPS yönlendirmesi isteğe bağlı olarak devre dışı bırakılabilir.
app.MapControllers();

// Uygulama ayağa kalkarken Kaggle kaynaklı temizlenmiş CSV veri setini In-Memory veritabanına seed ederek model ile tutarlı örnek veri sağlar.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var csvPath = Path.Combine(app.Environment.ContentRootPath, "cleaned_hotel_data.csv");
    CsvDataSeeder.SeedFromCsv(context, csvPath);
}

// Sadece geliştirme ve doğrulama amacıyla kullanılan, ham rezervasyon verisini hızlıca gözlemlemeye yönelik hafif bir okuma endpoint'i.
app.MapGet("/api/hotel-bookings", async (ApplicationDbContext db) =>
{
    var bookings = await db.HotelBookings.Take(50).ToListAsync();
    return Results.Ok(bookings);
});

app.Run();