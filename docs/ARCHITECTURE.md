# Mimari

## Mevcut prototip

Unity 6.3 LTS / Windows hedefi için tek enerji istasyonlu prototip kaynakları hazırlanmıştır. Gerçek Unity projesi, sahne ve `.meta` dosyaları bu Mac üzerinde oluşturulmadı. Unity derleme, Editor ve oynanış doğrulaması Windows ekibinde yapılmalıdır.

`UnityAssets/SonBirUmutDemo/` kaynakların asıl konumudur. Unity projesine buradan kopyalanır; ekip test projesindeki kodu değiştirirse değişiklikleri bu konuma geri taşımalıdır. İki ayrı kaynak kopyasında paralel geliştirme yapılmaz. Kalıcı bir Unity projesine geçiş ayrı bir ekip kararıdır.

## Sorumluluklar

| Konum | Sorumluluk |
| --- | --- |
| `Scripts/Simulation/ResourceState.cs` | Değiştirilemeyen bütçe, enerji, su, bitki ve emisyon anlık durumu; değer sınırları |
| `Scripts/Simulation/SimulationRules.cs` | Tek karar turunun kaynak hesabı; kuraklık ve gelecekteki pompalar için kapasite hesabı |
| `Scripts/Simulation/GameSession.cs` | Kayıtlı karar noktaları, maliyet doğrulaması, seçimin tek uygulanması ve oturum durumu |
| `Scripts/Decisions/DecisionCatalog.cs` | Değiştirilemeyen karar tanımları ve üç enerji seçeneği |
| `Scripts/Core` | Unity oturum koordinasyonu ve koşullu yeni/eski klavye giriş adaptörü |
| `Scripts/Player` | CharacterController hareketi ve sabit açılı, ortografik smooth follow kamera |
| `Scripts/Interaction` | İstasyon menzili ve seçime bağlı panel/jeneratör/tasarruf görselleri |
| `Scripts/UI` | Üç IMGUI kartı, kaynak göstergeleri, E ipucu, sonuç ve yeniden oynama |
| `Editor/DemoSceneBuilder.cs` | Yalnızca Editor'da çalışan primitive sahne ve materyal üretimi |
| `Data/Decisions`, `Data/Simulation`, `Scripts/World` | Sonraki aşamalara ayrılan mevcut klasörler; şu an ek sistem içermez |
| `Tests/LogicTests` | Unity dışından gerçek simülasyon kaynaklarını derleyen, harici paketsiz C# test koşucusu |

`Simulation` ve `Decisions` UnityEngine'e bağımlı değildir. Unity bileşenleri sonucu okur; kaynakları doğrudan değiştirmez. `GameSession` yeni karar tanımlarıyla genişletilebilir; mevcut Unity etkileşimi yalnızca enerji istasyonuna bağlıdır. Singleton, üçüncü taraf UI paketi veya harici model gerektirmez. IMGUI fare girdisi nedeniyle Active Input Handling **Both** veya **Old** olmalıdır; New-only modunda araç ve oturum açıklayıcı uyarıyla durur. Both modunda klavye yeni backend ile okunur.

## Veri ve oyun akışı

1. Sahne oluşturucu tüm nesneleri ve serileştirilen referansları bağlar.
2. `DemoGameController.Awake` yeni oturum açar. Erken çalışma sırası sayesinde oyuncu bu oturumu kullanır.
3. Oyuncu istasyonun önündeki etkileşim konumuna 3 birim yatay mesafe içinde yaklaşır. E ile kartlar açılır; hareket girdisi durur, yerçekimi sürer.
4. `GameSession.TryChoose` nokta ve seçenek kimliğini, tamamlanma durumunu ve bütçeyi kontrol eder. Başarısız seçim hiçbir kaynağı ve ilerlemeyi değiştirmez.
5. Başarılı seçimde `SimulationRules.ApplyTurn` bir kez çalışır; yeni durum ve tamamlanma kaydı saklanır.
6. İstasyon yalnızca seçilen görseli ve yeşil tamamlanma lambasını açar; kart kapanır, sonuç gösterilir. Aynı nokta tekrar kullanılamaz.
7. Yeniden oynama yeni `GameSession` yaratır, görselleri kapatır, oyuncuyu başlangıca taşır. Kamera yeniden oyuncuya yumuşakça yaklaşır.

## Örnek dengeleme

Tüm değerler boyutsuz oyun puanlarıdır; bilimsel ölçüm, para birimi veya gerçek emisyon miktarı değildir. Başlangıç: bütçe **100**, enerji **20**, su **40**, bitki **55**, emisyon **0**. Bütçe 0–999, diğer kaynaklar 0–100 arasında tutulur.

| Seçenek | Bütçe maliyeti | Enerji farkı | Doğrudan su farkı | Emisyon farkı |
| --- | ---: | ---: | ---: | ---: |
| Güneş | 60 | +35 | 0 | +1 |
| Dizel | 25 | +50 | -3 | +18 |
| Tasarruf | 10 | -5 | +8 | 0 |

Her başarılı karar bir turdur. Seçenek etkisinden sonra **8 su** tüketilir ve su sınırlandırılır. Kalan su en az 35 ise bitki etkisi +4, daha azsa -8 olur; ardından dışsal kuraklık baskısı **6** çıkarılır. Bitki değeri sınırlandırılır. Beklemek tur ilerletmez. İlk seçimden sonraki durumlar:

| Seçenek | Bütçe | Enerji | Su | Bitki | Emisyon |
| --- | ---: | ---: | ---: | ---: | ---: |
| Güneş | 40 | 55 | 32 | 41 | 1 |
| Dizel | 75 | 70 | 29 | 41 | 18 |
| Tasarruf | 90 | 15 | 40 | 53 | 0 |

Gelecekteki pompa eylemleri `PumpCapacity(energy, requestedWater)` ile `min(talep, enerji × 2)` sınırını kullanabilir. Mevcut prototipte pompa çalışmaz; güneş/dizel seçimi kendiliğinden su üretmez. Pompa eyleminin suya uygulanması sonraki karar noktasının işidir.

Emisyon ayrı izlenir. Jeneratör çevrede anlık sıcaklık artışı yaratmaz; emisyon bitki sağlığına anlık ceza uygulamaz. Kuraklık baskısı dışarıdan belirlenen sabit oyun parametresidir.

## Kapsam dışı

Diğer iki karar noktası, crafting, çöp toplama, NPC, hikâye, animasyon, kayıt sistemi ve üç kararlı tam final sonraki aşamalardadır. Mevcut tamamlanma paneli yalnızca tek istasyonun sonucunu ve yeniden oynamayı sunar. Sahne, materyal, `.meta`, `ProjectSettings` ve paket dosyaları Unity Editor tarafından üretilecektir; elle taklit edilmez.
