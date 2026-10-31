# Mimari — anlaşılır kararlar / polish

## Sınırlar ve durum

Unity 6.3 LTS / Windows hedefi. Tek primitive harita, üç karar ve üç final vardır. Gerçek Unity projesi, sahne veya `.meta` bu Mac üzerinde üretilmez. `UnityAssets/SonBirUmutDemo` kaynakların asıl konumudur; Windows test projesine kopyalanır. Kaynak complete-demo dalının oynanışı kullanıcı aktarımıyla doğrulandı; yeni polish arayüzünün Editor/oynanış doğrulaması Hakan veya Enes'tedir.

## Sorumluluklar

| Sınıf / konum | Sorumluluk |
| --- | --- |
| `ResourceState` | Değiştirilemeyen kaynak anlık durumu ve sınırları |
| `DecisionCatalog` | Üç nokta, dokuz seçenek, maliyetler ve etki türleri |
| `SimulationRules` | Su tüketimi, pompa, bitki desteği, kuraklık ve uygulanabilirlik hesabı |
| `TurnBreakdown` | Seçim, tur su kullanımı, su koşulu, kuraklık ve sınır düzeltmelerinin ayrımı |
| `DecisionMessaging` | Hesap kayıtlarından kart, bildirim ve kritik kaynak açıklaması |
| `EndingSummary` | Mevcut durum/karar ID'lerinden su, tarım, doğa, iki karar ve risk açıklaması |
| `DecisionPreview`, `DecisionRecord` | Net etkiler ve önceki/sonraki durumla seçim geçmişi |
| `GameSession` | Enerji → su → tarım sırası, tek uygulama, oturum ve final |
| `EndingResult` | Birden fazla kaynağa dayalı üç sonuç kategorisi ve açıklaması |
| `DemoGameController` | Giriş anlatımı, oynama/kart/final akışı, kapatılabilir bildirim ve Unity referansları |
| `DemoStory` | Merkezi, kolay değiştirilebilir giriş, hedef ve kapanış metni |
| `DemoInput` | Koşullu eski/yeni klavye API'si; Both/Old IMGUI gereksinimi |
| `DemoPlayerMotor` | Korunan CharacterController; yatay hız ve Y dönüşü yumuşatması |
| `IsometricFollowCamera` | Sabit açı; normal takip ve finalde panoramik kadraj |
| `EnergyStation` | Önceki enerji noktasının korunan menzil ve model referansları |
| `DecisionStation` | Su ve tarım menzili; seçime karşılık gelen görsel grubu |
| `DemoWorldView` | Su göstergesi, ağaç ve tarla renklerinin durumdan türetilmesi |
| `DemoHud` | IMGUI giriş, HUD, kartlar, net etki ve final geçmişi |
| `Editor/DemoSceneBuilder` | Primitive çevre, referans bağlama ve benzersiz Editor varlıkları |

`Simulation` ve `Decisions` UnityEngine'den bağımsızdır. `Tests/LogicTests` bu dosyaları doğrudan derler; mantığın kopyasını kullanmaz. UnityEditor referansları yalnızca `Editor` klasöründedir. Harici model, UI paketi, singleton veya Rigidbody eklenmedi.

## Akış

1. Sahne oluşturucu mevcut köyü yeni bir sahne olarak kurar; enerji istasyonu korunur, su/tarım noktaları eklenir.
2. Oturum başlangıç değerleriyle açılır; giriş metni açıkken hareket kilitlidir. Enter/Başla keşfi başlatır.
3. Yalnızca sıradaki noktanın menzilinde E kartları açar. Kart açıkken yatay hız aynı karede sıfırlanır; yerçekimi sürer.
4. Her kart `GameSession.GetPreview` üzerinden gerçek maliyeti, ayrı seçim/tur/kuraklık bileşenlerini ve tur sonu net etkilerini okur. Karşılanamayan seçenek açıklamayla devre dışıdır.
5. `TryChoose` aynı hesapla doğrular; reddedilen seçim hiçbir durum veya geçmişi değiştirmez. Kabul edilen seçim tam bir tur ilerletir.
6. Görsel gruplar ve `DemoWorldView` yeni durumu gösterir. Emisyon bitkiye veya sıcaklığa anlık etki etmez.
7. Üçüncü karardan sonra `EndingResult` hesaplanır; hareket kilitlenir. Kamera aynı açıda uzaklaşıp köyü sonuç panelinin solunda çerçeveler.
8. Yeniden Oyna yeni GameSession açar; oyuncu, hız, kamera, modeller, su seviyesi ve renkler başlangıca döner. Giriş yeniden görünür.

Karar formülleri ve eşikler [DECISIONS](DECISIONS.md), tüm temel diziler [DECISION_MATRIX](DECISION_MATRIX.md) içinde tutulur.

## Görsel ve sahne güvenliği

Ağaçlar bitki sağlığı ≥55'te yeşil, 30–54'te sarı, daha düşükte kurudur. Tarla rengi sağlıkla sürekli değişir. Depo önündeki mavi gösterge suyla ölçeklenir; suyun sıfırında çok ince bir taban kalır. `MaterialPropertyBlock` ile renkler değiştirilir, ortak materyal varlıkları değiştirilmez. Yeniden oynama yeni materyal kopyaları yaratmaz.

Evler, ağaç gövdeleri/taçları, depo, istasyon kaidesi ve harita sınırları çarpışma içerir. Karar sonrasında görünen dekoratif modeller oyuncuyu içine hapsetmemek için çarpışmasızdır. Gösterge ve tarla ürünleri de dekoratiftir.

Araç `SonBirUmutCompleteDemo.unity` ve `Generated/CompleteDemo...` için benzersiz yollar kullanır. Kirli açık sahne, Prefab düzenleme modu veya New-only input durumunda hiçbir sahne üretmeden durur. Mevcut sahne dosyası silinmez veya kaydedilmez.

## Geriye dönüş

`EnergyStation` sınıfı ve eski serileştirilmiş `player`, `station`, kamera `target`, hareket `speed` alanları korunur. Eski sahnelerde yeni `completeDemo` alanı false olduğundan tek noktalı akış sürer. Üç noktalı akış yalnızca yeni sahne oluşturucunun `ConfigureCompleteDemo` çağrısıyla açılır. Birebir eski denge ve kod `main` dalındadır; mevcut feature dalında tasarruf ücretsizdir.

## Bu aşamada korunanlar

CharacterController, kamera, Input adaptörü, EnergyStation/DecisionStation, DemoWorldView, Scene Builder ve EndingResult eşik dosyaları değiştirilmedi. Yeni harita veya model yoktur. Giriş metni ve kart ödünleşimleri merkezidir. İki sayısal değişiklik güneş emisyonu 0 ve sulama bütçesi 5'tir; ayrıntı [POLISH_DESIGN](POLISH_DESIGN.md) içindedir.

## Açık doğrulamalar

C# mantığı otomatik test edildi; Unity API tür denetimi, Editor derlemesi, input/fizik, görsel kadraj, eski sahne uyumu, 3–5 dakika hedefi ve Windows build henüz test edilmedi. [Test listesi](../UNITY_TEST_CHECKLIST.md) bu ayrımı korur. Yeni giriş hastalık veya ayrı hikâye görevi içermez.
