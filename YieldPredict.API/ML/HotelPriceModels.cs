using Microsoft.ML.Data;

namespace YieldPredict.API.ML;

// API yüzeyine yakın ML şema tipleri; eğitim sırasında kullanılan model şeması ile HTTP istek/yanıt sözleşmeleri arasında ince bir bağlayıcı katman görevi görür.
public class HotelData
{
    public string Hotel { get; set; } = string.Empty;
    public float IsCanceled { get; set; }
    public float LeadTime { get; set; }
    public string ArrivalDateMonth { get; set; } = string.Empty;
    public float Adults { get; set; }
    public float Children { get; set; }
    public string Meal { get; set; } = string.Empty;
    public string MarketSegment { get; set; } = string.Empty;
    public string ReservedRoomType { get; set; } = string.Empty;
    public float Adr { get; set; }
}

public class HotelPrediction
{
    [ColumnName("Score")]
    public float Score { get; set; }
}

public class HotelPredictionRequest
{
    public string Hotel { get; set; } = string.Empty;
    public bool IsCanceled { get; set; }
    public int LeadTime { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public string Meal { get; set; } = string.Empty;
    public string MarketSegment { get; set; } = string.Empty;
    public string ReservedRoomType { get; set; } = string.Empty;
    public string ArrivalDateMonth { get; set; } = string.Empty;
}

