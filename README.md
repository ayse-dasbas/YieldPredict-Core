# YieldPredict Core | Akıllı Otel Fiyat Tahminleyici API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![ML.NET](https://img.shields.io/badge/ML.NET-Machine%20Learning-blue?style=flat)
![C#](https://img.shields.io/badge/C%23-Backend-239120?style=flat&logo=c-sharp&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-API%20Docs-85EA2D?style=flat&logo=swagger&logoColor=black)

**YieldPredict Core**, turizm ve konaklama sektörü için geliştirilmiş, **Makine Öğrenimi (ML.NET)** destekli bir Verim Yönetimi (Yield Management) ve Fiyat Tahmin (ADR - Average Daily Rate) API servisidir.

Bu proje, Kaggle "Hotel Booking Demand" veri setindeki yaklaşık 117.000 satırlık geçmiş rezervasyon verisini analiz ederek; misafir profili, iptal durumu, rezervasyon önceliği (lead time) ve yemek paketleri gibi parametrelere göre en ideal oda fiyatını saniyeler içinde tahmin eden bir yapay zeka beynine sahiptir.

## Projenin Öne Çıkan Özellikleri

* **Yapay Zeka Entegrasyonu:** `Microsoft.ML` kütüphanesi ile `FastTreeRegression` algoritması kullanılarak eğitilmiş özel bir tahmin modeli (Model Başarısı: **R-Squared: 0.7897**).
* **Clean Architecture Mimarisi:** Katmanlı mimari prensiplerine (Core, Infrastructure, API) sıkı sıkıya bağlı kalınarak tasarlanmış ölçeklenebilir altyapı.
* **Büyük Veri İşleme (Data Seeding):** 117.000+ satırlık temizlenmiş CSV verisinin uygulama ayağa kalkarken otomatik olarak `Entity Framework Core` ile In-Memory veritabanına tohumlanması (Seeding).
* **Thread-Safe ML Tahminleri:** Yüksek trafikli ortamlar için `PredictionEnginePool` kullanılarak optimize edilmiş model servisi.
* **RESTful API & Dokümantasyon:** Geliştirici deneyimini artırmak için Swagger / OpenAPI ile uçtan uca dokümante edilmiş endpoint'ler.

## Mimari Yapı

Proje, Sorumlulukların Ayrılığı (Separation of Concerns) prensibi gözetilerek 4 ana projeye bölünmüştür:

1. **YieldPredict.Core:** Domain modelleri (Entities) ve arayüzler (Interfaces).
2. **YieldPredict.Infrastructure:** Veritabanı bağlantıları (EF Core), Data Seeding işlemleri (CsvDataSeeder) ve veri kalıcılığı.
3. **YieldPredict.ModelTrainer:** Makine öğrenimi modelinin (%80 Train, %20 Test) eğitildiği, Feature Engineering (OneHotEncoding) işlemlerinin yapıldığı ve `.zip` model dosyasının üretildiği izole konsol uygulaması.
4. **YieldPredict.Api:** Sunum katmanı. Dış dünyaya açılan HTTP endpoint'lerini ve Swagger arayüzünü barındırır.

## Makine Öğrenimi Performansı

Tahmin modeli, `YieldPredict.ModelTrainer` üzerinden aşağıdaki metriklerle eğitilmiştir:
* **Algoritma:** FastTreeRegression
* **R-Squared:** 0.7897 *(Modelin gerçek dünyadaki fiyat değişkenliklerini açıklama oranı)*
* **RMSE:** 21.43 *(Tahminlerdeki ortalama sapma payı)*

## Kurulum ve Çalıştırma

Projeyi kendi bilgisayarınızda çalıştırmak için aşağıdaki adımları izleyin:

**1. Depoyu Klonlayın:**
```bash
git clone [https://github.com/kullaniciadiniz/YieldPredict-Core.git](https://github.com/kullaniciadiniz/YieldPredict-Core.git)
cd "YieldPredict Core"

```

**2. API'yi Çalıştırın:**

```bash
dotnet run --project YieldPredict.Api

```

Uygulama başladığında, veritabanı tohumlaması otomatik olarak gerçekleşecek ve model hazır hale gelecektir. Tarayıcınızda `http://localhost:5067/swagger` adresine giderek API'yi test edebilirsiniz.

## 📡 Örnek İstek (cURL)

**ADR (Ortalama Günlük Fiyat) Tahmini Almak İçin:**

```bash
curl -X POST "http://localhost:5067/api/HotelBooking/predict" \
     -H "Content-Type: application/json" \
     -d '{
           "hotel": "Resort Hotel",
           "isCanceled": false,
           "leadTime": 45,
           "arrivalDateMonth": "August",
           "adults": 2,
           "children": 1,
           "meal": "HB",
           "marketSegment": "Online TA",
           "reservedRoomType": "D"
         }'

```

**Örnek Başarılı Yanıt (200 OK):**

```json
{
  "predictedAdr": 145.82
}

```

## Gelecek Geliştirmeler (Üretime Geçiş Vizyonu)

Bu proje şu an bir MVP (Minimum Viable Product) ve konsept kanıtı (Proof of Concept) olarak tasarlanmıştır. Sistemi gerçek bir canlı ortama (Production) taşımak için planlanan mimari geliştirmeler şunlardır:

* **Gerçek İlişkisel Veritabanı:** Geliştirme kolaylığı için kullanılan In-Memory DB yerine, kalıcı ve ilişkisel veri yönetimi için **PostgreSQL** veya **Microsoft SQL Server** entegrasyonu.
* **API Güvenliği:** Uç noktalara yetkisiz erişimi engellemek için **JWT (JSON Web Token)** tabanlı kimlik doğrulama ve yetkilendirme mekanizması.
* **Test Kapsamı:** Modelin tutarlılığını ve API endpoint'lerinin kararlılığını sürekli ölçmek için **Unit Test (Birim Testi)** ve entegrasyon testlerinin yazılması.
* **Konteynerizasyon:** Projenin bulut ortamlarına sorunsuz, bağımsız ve izole bir şekilde dağıtılabilmesi için **Docker** entegrasyonu.


## Geliştirme Yaklaşımı (AI-Assisted Engineering)

Bu projenin iş mantığı, kullanılacak teknoloji yığını (C#, ML.NET) ve Clean Architecture katmanlandırması tarafımca tasarlanmıştır. Geliştirme sürecinde modern yazılım mühendisliği pratikleri benimsenerek, rutin kod üretimini hızlandırmak ve karmaşık hata ayıklama (debugging) süreçlerini optimize etmek için yapay zeka araçları (Cursor & LLM'ler) birer asistan (Pair Programmer) olarak kullanılmıştır. Bu modern "Vibe Coding" geliştirme yaklaşımı sayesinde, sözdizimi detaylarında boğulmadan doğrudan ürünün mimari kalitesine ve verimliliğine odaklanılmıştır.