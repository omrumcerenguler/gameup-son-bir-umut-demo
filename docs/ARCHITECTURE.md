# Mimari — Kriz Nöbeti Aşama 1/2

## Yeni oturum katmanı

`feature/live-village-demo` mevcut stratejik hesapların üstüne saf C# `GameRunState` ekler. `GameSession.IsComplete` stratejik hazırlıktır; `GameRunState.IsEnded` tüm oturumun sonudur. Unity kontrolcüsünün `IsComplete` özelliği artık ikincisini yansıtır. Seçimler `Run.TryChoose` üzerinden yapılır.

| Sınıf | Eklenen sorumluluk |
| --- | --- |
| GameRunState / GameMode / GameRunPhase | Mod, faz, odak/pause, seçim ve zaman sonu koordinasyonu |
| GameRunSettings | Doğrulanan süre, başlangıç baskısı ve artış katsayıları |
| SessionTimer | Bağımsız duraklayan sayaç; yalnız gerçek kalan süreyi tüketir |
| CrisisPressure | Sınırlandırılmış gösterge ve ilerideki bakım için azaltma noktası |
| RunOutcome | Erken süre sonu dahil null olmayan final/özet |
| MovementPolicy | Saf C# sprint hızı ve kilit politikası |
| DemoCrisisAtmosphere | Yalnız opt-in sahnede hafif sis/ışık sunumu |

Normal sahne `LegacyDecisions`: Intro → Preparation → üçüncü kararda Ending. Yeni ayrı sahne `CrisisFoundation`: üçüncü kararda CrisisManagement, süre sonu Ending. Eski sahnelerin serileştirilmiş referansları ve false özellik anahtarı korunur. Yeni mod varsayılan değildir. Bakım/acil arıza/yeni kazanma kuralları Aşama 2'dedir.

[Zaman, baskı, modlar ve ayarların ayrıntısı](LIVE_VILLAGE_STAGE1.md). Aşağıdaki görsel temel korunur; akışın üçüncü karar sonrası final adımı yalnız eski mod için geçerlidir.

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
| `DemoVisualBuilder` (Editor) | Çokgen mesh, köy/karakter ayrıntıları; gerçek Editor varlıkları |
| `DemoCharacterVisual` | Root hareketini değiştirmeden, controller hızından eklem pozu |
| `DemoGuiTheme` | HUD/kart/final renkleri, stilleri ve temizlenen runtime dokular |
| `DemoPlayerMotor` | Korunan CharacterController; yatay hız ve Y dönüşü yumuşatması |
| `IsometricFollowCamera` | Sabit açı; normal takip, panoramik final ve rapor paneline göre yumuşak merkezleme |
| `EnergyStation` | Önceki enerji noktasının korunan menzil ve model referansları |
| `DecisionStation` | Su ve tarım menzili; seçime karşılık gelen görsel grubu |
| `DemoWorldView` | Su göstergesi/havuz, kuru dal/taç, ürün boyu ve toprak renginin durumdan türetilmesi; görsel geçişler |
| `DemoHud` | Temalı IMGUI, kaynak çubukları, hedef etiketi, kaydırılabilir final sekmeleri ve köyü izleme modu |
| `Editor/DemoSceneBuilder` | Primitive çevre, referans bağlama ve benzersiz Editor varlıkları |

`Simulation` ve `Decisions` UnityEngine'den bağımsızdır. `Tests/LogicTests` bu dosyaları doğrudan derler; mantığın kopyasını kullanmaz. UnityEditor referansları yalnızca `Editor` klasöründedir. Harici model, UI paketi, singleton veya Rigidbody eklenmedi.

## Akış

1. Sahne oluşturucu mevcut köyü yeni bir sahne olarak kurar; enerji istasyonu korunur, su/tarım noktaları eklenir.
2. Oturum başlangıç değerleriyle açılır; giriş metni açıkken hareket kilitlidir. Enter/Başla keşfi başlatır.
3. Yalnızca sıradaki noktanın menzilinde E kartları açar. Kart açıkken yatay hız aynı karede sıfırlanır; yerçekimi sürer.
4. Her kart `GameSession.GetPreview` üzerinden gerçek maliyeti, tur sonu net etkilerini okur; ayrı seçim/tur/kuraklık bileşenleri isteğe bağlı ayrıntıdadır. Karşılanamayan seçenek açıklamayla devre dışıdır.
5. `TryChoose` aynı hesapla doğrular; reddedilen seçim hiçbir durum veya geçmişi değiştirmez. Kabul edilen seçim tam bir tur ilerletir.
6. Görsel gruplar ve `DemoWorldView` yeni durumu gösterir. Emisyon bitkiye veya sıcaklığa anlık etki etmez.
7. Üçüncü karardan sonra `EndingResult` hesaplanır; hareket kilitlenir. Kamera aynı açıda uzaklaşıp köyü sonuç panelinin solunda çerçeveler.
8. Yeniden Oyna yeni GameSession açar; oyuncu, hız, kamera, modeller, su seviyesi ve renkler başlangıca döner. Giriş yeniden görünür.

Karar formülleri ve eşikler [DECISIONS](DECISIONS.md), tüm temel diziler [DECISION_MATRIX](DECISION_MATRIX.md) içinde tutulur.

## Görsel ve sahne güvenliği

Ağaçlar bitki sağlığı ≥55'te yeşil, 30–54'te sarı, daha düşükte kurudur. Tarla rengi sağlıkla sürekli değişir. Depo önündeki mavi gösterge suyla ölçeklenir; suyun sıfırında çok ince bir taban kalır. `MaterialPropertyBlock` ile renkler değiştirilir, ortak materyal varlıkları değiştirilmez. Yeniden oynama yeni materyal kopyaları yaratmaz.

Ev/temel, ağaç gövdeleri, depo, kuyu, bank oturağı, çit kirişleri, istasyon kaidesi ve harita sınırları çarpışma içerir. Yaprak taçları ve küçük dekorlar görseldir. Karar sonrasında görünen dekoratif modeller oyuncuyu içine hapsetmemek için çarpışmasızdır. Gösterge ve tarla ürünleri de dekoratiftir.

Araç `SonBirUmutVisualDemo.unity` ve `Generated/VisualDemo...` için benzersiz yollar kullanır. Kirli açık sahne, Prefab düzenleme modu veya New-only input durumunda hiçbir sahne üretmeden durur. Mevcut sahne dosyası silinmez veya kaydedilmez.

## Geriye dönüş

`EnergyStation` sınıfı ve eski serileştirilmiş `player`, `station`, kamera `target`, hareket `speed` alanları korunur. Eski sahnelerde yeni `completeDemo` alanı false olduğundan tek noktalı akış sürer. Üç noktalı akış yalnızca yeni sahne oluşturucunun `ConfigureCompleteDemo` çağrısıyla açılır. Birebir eski denge ve kod `main` dalındadır; mevcut feature dalında tasarruf ücretsizdir.

## Kaynak dalın tarihsel görsel Aşama 2 sınırı

Kaynak polish dalındaki görsel Aşama 2'de Simulation/Decisions kaynakları, kaynak değerleri, final sınıflandırması, DemoGameController, DemoInput ve DemoPlayerMotor birebir korunur. Scene Builder yeni geometri kurar; kamera, dünya görünümü ve HUD yalnız sunum için genişletilir. Yeni modeli görmek için yeni VisualDemo sahnesi üretilir; eski sahneler değiştirilmez. [Aşama 2 raporu](VISUAL_STAGE2.md), önceki mantık/tasarım kaydı [POLISH_DESIGN](POLISH_DESIGN.md) içindedir.

DemoWorldView kaynak hedeflerini okur; kendi `plantView/waterView` değerleri yalnız interpolasyon içindir. Oturum değişince görsel durum sıfırlanır. DemoCharacterVisual root konumuna veya collider'a yazmaz. DemoGuiTheme küçük runtime dokularını OnDestroy'da serbest bırakır. Final sekmeleri/kadraj değişimi yeni GameSession yaratmaz veya kaynak harcamaz.

## Açık doğrulamalar

C# mantığı otomatik test edildi; Unity API tür denetimi, Editor derlemesi, input/fizik, görsel kadraj, eski sahne uyumu, 3–5 dakika hedefi ve Windows build henüz test edilmedi. [Test listesi](../UNITY_TEST_CHECKLIST.md) bu ayrımı korur. Yeni giriş hastalık veya ayrı hikâye görevi içermez.
