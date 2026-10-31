# Mimari planı

Bu belge sonraki geliştirme aşamasına yönelik taslaktır. Aşağıdaki sorumluluklar henüz C# sınıfları, Unity bileşenleri veya paket bağımlılıkları olarak uygulanmamıştır.

## Kapsam

Demo tek küçük kırsal haritada, izometrik kamerayla oynanacak. Oyuncu WASD ile hareket edecek, E ile karar noktalarını açacak. Üç noktanın her biri üç seçenek sunacak. Kararlar enerji, su ve bitki durumunu birlikte etkileyecek; dünya bu sonuçları görsel olarak yansıtacak. Oyun final ekranı ve yeniden oynama akışıyla tamamlanacak.

## Planlanan klasör sorumlulukları

| Klasör | Sorumluluk |
| --- | --- |
| `Scripts/Core` | Oturum yaşam döngüsü, başlangıç, final ve yeniden başlatma koordinasyonu |
| `Scripts/Player` | Oyuncu hareketi ve kamera takibiyle ilgili davranışlar |
| `Scripts/Interaction` | Etkileşim menzili, hedef seçimi ve E girdisinin yönlendirilmesi |
| `Scripts/Decisions` | Karar noktalarının ilerlemesi, seçeneklerin uygulanması ve tekrar seçimi önleme |
| `Scripts/Simulation` | Enerji, su ve bitki durumunun kuralları ve birbirine etkileri |
| `Scripts/World` | Durumu haritadaki bitkiler, su ve diğer görsel öğelere yansıtma |
| `Scripts/UI` | Kaynak göstergeleri, üç seçenekli kartlar, final ve yeniden oynama arayüzü |
| `Data/Decisions` | Karar metinleri, seçenekler ve etki tanımları için ileride oluşturulacak veri varlıkları |
| `Data/Simulation` | Başlangıç değerleri, sınırlar ve dengeleme ayarları için veri varlıkları |

## Planlanan veri akışı

1. Oyuncu karar noktasına yaklaşır ve E ile etkileşir.
2. Karar sistemi uygun noktayı doğrular; arayüz üç seçeneği gösterir.
3. Seçim tek kez uygulanır; simülasyon kaynak değişimlerini ve bağlantılı etkileri hesaplar.
4. Güncel durum arayüze ve dünya görsellerine iletilir.
5. Üç nokta tamamlandığında final değerlendirilir.
6. Yeniden oynama; kaynakları, karar ilerlemesini, oyuncu konumunu ve dünya görünümünü başlangıca döndürür.

Simülasyon kurallarının arayüzden ve dünya görsellerinden ayrılması hedeflenir. Arayüz kaynak değerlerini doğrudan değiştirmemeli; görsel bileşenler güncel oyun durumunu yansıtmalıdır. Sayısal etkiler ve bağlantılı kaynak kuralları sonraki aşamada belirlenecek.

## Açık teknik kararlar

- Unity 6.x'in kesin Editor sürümü ve render pipeline.
- Input System seçimi ve paket sürümleri.
- Karar ve dengeleme verilerinin biçimi; gerekirse gerçek Editor içinde ScriptableObject varlıkları.
- Simülasyonun tur/karar bazlı veya zaman bazlı işleyişi ve kaynakların sayısal sınırları.
- Final koşulları, görsel durum eşikleri ve Windows demo build ayarları.

Şimdilik sahne, prefab, materyal, `.meta`, `ProjectSettings`, `Packages/manifest.json` veya `.csproj` dosyası üretilmez. Gerçek Unity dosyaları Editor ile oluşturulup ekipçe doğrulandığında eklenir.
