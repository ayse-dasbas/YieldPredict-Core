using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

var mlContext = new MLContext(seed: 1);

// Model eğitici konsol uygulamasının, çözüm yapısı değişse bile veri setine göreli (portable) erişebilmesi için CSV yolu çoklu kökten resolve edilir.
var dataPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "cleaned_hotel_data.csv");
if (!File.Exists(dataPath))
{
    var altPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "YieldPredict.Api", "cleaned_hotel_data.csv");
    if (File.Exists(altPath))
        dataPath = altPath;
}

Console.WriteLine($"Veri dosyası aranıyor: {dataPath}");

// Kaggle tabanlı ham veri, ML.NET şema sınıfı üzerinden strongly-typed olarak yüklenir; bu sayede eğitim pipeline'ı ile domain modeli gevşek bağlı kalır.
IDataView dataView = mlContext.Data.LoadFromTextFile<HotelData>(
    path: dataPath,
    hasHeader: true,
    separatorChar: ',');

// Hold-out validation stratejisi ile veri seti train/test olarak ayrıştırılır; böylece model üretime alınmadan önce offline performans metrikleri gözlemlenebilir.
var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

// Özellik mühendisliği (One-Hot Encoding + feature concatenation) ve FastTreeRegression algoritmasını bir araya getiren uçtan uca regresyon pipeline'ı tanımlanır.
var pipeline =
    mlContext.Transforms.Categorical.OneHotEncoding(
            new[]
            {
                new InputOutputColumnPair("HotelEncoded", nameof(HotelData.Hotel)),
                new InputOutputColumnPair("ArrivalDateMonthEncoded", nameof(HotelData.ArrivalDateMonth)),
                new InputOutputColumnPair("MealEncoded", nameof(HotelData.Meal)),
                new InputOutputColumnPair("MarketSegmentEncoded", nameof(HotelData.MarketSegment)),
                new InputOutputColumnPair("ReservedRoomTypeEncoded", nameof(HotelData.ReservedRoomType)),
            })
        .Append(mlContext.Transforms.Concatenate(
            "Features",
            "IsCanceled",
            "LeadTime",
            "Adults",
            "Children",
            "HotelEncoded",
            "ArrivalDateMonthEncoded",
            "MealEncoded",
            "MarketSegmentEncoded",
            "ReservedRoomTypeEncoded"))
        .Append(mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(HotelData.Adr)))
        .Append(mlContext.Regression.Trainers.FastTree());

Console.WriteLine("Model eğitiliyor... (Bu işlem birkaç saniye sürebilir)");

var model = pipeline.Fit(split.TrainSet);

Console.WriteLine("Model test verisi ile değerlendiriliyor...");

var predictions = model.Transform(split.TestSet);
var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

Console.WriteLine($"\n--- MODEL BAŞARI METRİKLERİ ---");
Console.WriteLine($"R-Squared: {metrics.RSquared:F4} (1'e ne kadar yakınsa o kadar iyi)");
Console.WriteLine($"RMSE     : {metrics.RootMeanSquaredError:F4} (Tahminlerdeki ortalama sapma)");

// Eğitilmiş model, API katmanında PredictionEnginePool tarafından tüketilen tekil .zip artefaktı olarak diske kaydedilir; böylece eğitim ve tahmin boru hattı ayrıştırılır.
var modelPath = Path.Combine(AppContext.BaseDirectory, "OtelFiyatModeli.zip");
mlContext.Model.Save(model, split.TrainSet.Schema, modelPath);

Console.WriteLine($"\nHarika! Model başarıyla kaydedildi: {modelPath}");


// ML.NET giriş şeması; Kaggle veri setindeki sütunlarla bire bir hizalanmış olup, domain entity'lerinden bağımsız tutulur.
public class HotelData
{
    [LoadColumn(0)] public string Hotel { get; set; } = string.Empty;
    [LoadColumn(1)] public float IsCanceled { get; set; }   // İptal durumunu, regresyon pipeline'ında sürekli özellik olarak temsil etmek için sayısal forma dönüştürülmüş bayrak.
    [LoadColumn(2)] public float LeadTime { get; set; }
    [LoadColumn(3)] public string ArrivalDateMonth { get; set; } = string.Empty;
    [LoadColumn(4)] public float Adults { get; set; }
    [LoadColumn(5)] public float Children { get; set; }
    [LoadColumn(6)] public string Meal { get; set; } = string.Empty;
    [LoadColumn(7)] public string MarketSegment { get; set; } = string.Empty;
    [LoadColumn(8)] public string ReservedRoomType { get; set; } = string.Empty;
    [LoadColumn(9)] public float Adr { get; set; }   // Label
}

// ML.NET tahmin çıktısı; API tarafında ADR tahmin skorunun sade ve okunabilir şekilde taşınmasını sağlayan view-model benzeri yapı.
public class HotelPrediction
{
    [ColumnName("Score")]
    public float Score { get; set; }
}