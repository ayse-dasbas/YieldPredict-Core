using Microsoft.Extensions.ML;
using YieldPredict.API.ML;

namespace YieldPredict.API.Services;

// API katmanında, PredictionEnginePool üzerinden thread-safe model erişimi sağlayan ve controller'lar ile ML modelini gevşek bağlı tutan application service.
public class PredictionService
{
    private readonly PredictionEnginePool<HotelData, HotelPrediction> _predictionEngine;

    public PredictionService(PredictionEnginePool<HotelData, HotelPrediction> predictionEngine)
    {
        _predictionEngine = predictionEngine; // DI konteyneri tarafından yönetilen, yüksek trafikte bile thread-safe tahmin akışları sunan paylaşımlı prediction havuzu.
    }

    public HotelPrediction Predict(HotelData input)
    {
        return _predictionEngine.Predict(input);
    }
}

