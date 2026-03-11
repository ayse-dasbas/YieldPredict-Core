import pandas as pd

print("Veri temizleme işlemi başlıyor...")

# Kaggle ham veri seti, yalnızca .NET tarafındaki domain modeliyle hizalı olacak hedef kolonlara indirgenerek feature space ile API kontratı arasında tutarlılık sağlanır.
df = pd.read_csv('hotel_bookings.csv')

kullanilacak_kolonlar = [
    'hotel', 'is_canceled', 'lead_time', 'arrival_date_month',
    'adults', 'children', 'meal', 'market_segment',
    'reserved_room_type', 'adr'
]
df_temiz = df[kullanilacak_kolonlar].copy()

# Eksik çocuk sayısı gözlemleri, modelin eğitiminde yanlılığa sebep olmadan, domain açısından nötr kabul edilen 0 değeri ile imputasyon stratejisi kapsamında doldurulur.
df_temiz['children'] = df_temiz['children'].fillna(0)

# ADR metriğinde 0 veya negatif değer içeren kayıtlar, fiyat sinyalini bozup regresyon modelini çarpıtmaması için uç değer (outlier / invalid record) olarak veri setinden çıkarılır.
df_temiz = df_temiz[df_temiz['adr'] > 0]

# Temizlenmiş ve daraltılmış veri seti, ML.NET eğitim pipeline'ı ile API In-Memory seeding sürecinin ortak referans datası olacak şekilde normalize edilmiş CSV çıktısı olarak persist edilir.
df_temiz.to_csv('cleaned_hotel_data.csv', index=False)

print("İşlem tamam! 'cleaned_hotel_data.csv' dosyası başarıyla oluşturuldu.")
