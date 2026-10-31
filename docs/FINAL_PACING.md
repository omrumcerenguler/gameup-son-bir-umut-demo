# Final Pacing — bağımsız bakım noktaları ve tempo

Dal `feature/final-pacing`; korunan kaynak `feature/release-candidate / 5eeb35e7dad7343e2591f1d3b03870ad47d84305`. Hakan'ın kullanıcı tarafından aktarılan geri bildirimi görevler arasında hareket ve zaman baskısı eksikliğidir. Yeni kaynaklar bu hedefe yöneliktir; gerçek Unity deneyimi bu Mac'te ölçülmedi.

## Profil ve konum kimliği

Yeni `GameMode.FinalPacing`, mevcut Playtest oynanışı ve ReleaseFlow menü/ses/pause kabuğunu kullanır. Eski RC bayrağı kendi başına hâlâ beş görev/en az dört profilini açar. Yeni sahne ayrıca `finalPacing` bayrağı ve on kalıcı `MaintenanceTaskNode` referansı gerektirir. Eski GameMode sayıları ve sahne yolları korunur.

`FinalPacingCatalog` on immutable tanım içerir. Her tanımın benzersiz TaskId, LocationId, bölge, başlık, kısa açıklama, etkiler ve X/Z konumu vardır. Seçili `MaintenanceTask`, bu verileri ve Locked/Available/Completed durumunu taşır. **Final Pacing bakımında StationId=null**; stratejik istasyonlar ve acil arızanın sabit su istasyonu değişmez. `AtStation` yeni profilde görev döndürmez; eski tamamlama API'si yeni profilde ödül vermez.

| TaskId | Bölge / obje | Dünya X, Z |
| --- | --- | --- |
| solar-link | Enerji / yedek panel bağlantı kutusu | 12, 2 |
| distribution-box | Enerji / elektrik dağıtım kutusu | 3, 3 |
| saving-check | Enerji / tasarruf kontrol anahtarı | 4, -4 |
| tank-check | Su / ana depo çıkış kontrolü | -9.5, 4.8 |
| filter-clean | Su / kanal giriş filtresi | -9.3, 0.5 |
| pipe-joint | Su / köy dağıtım borusu | -3.5, 1 |
| reserve-line | Su / güney yedek hattı | -9.2, -6.5 |
| irrigation-valve | Tarım / güney sulama kontrolü | 4, -9.5 |
| soil-check | Tarım / tarla nem direği | 11.5, -5 |
| tree-care | Doğa / kuzey ağaç koruma işareti | 1, 10 |

Her LocationId `node-` + TaskId'dir. Konumlar 32×28 sınırları içindedir; görevler kanalın doğu tarafındadır. Yeni köprü/büyük yol/harita gerekmedi. Ev temelleri, depo, enerji kaidesi, kuyu, tarla direği, bank ve gövdeler kaynak düzeyinde dikkate alındı. Ayrı küçük objeler çarpışmasızdır; normal ev/tesis collider'ları değişmedi. Fiziksel erişim ve kamera görünürlüğü **Unity'de doğrulanmalıdır**.

## Altı görev, en fazla iki çağrı

Her oturumda enerji grubundan 2/3, su grubundan 2/4, tarım/doğadan 2/3 seçilir: toplam altı, üç bölge, **54 farklı altılı set**. Yerel oturum sayacı setleri döndürür; restart ve yeni oyun yeni seçim üretir. Oyun kaydı veya global Unity Random durumu yoktur. Model Reset aynı seed'i temiz kurar; controller yeni Run üretirken sonraki seed'i kullanır.

Seçilenler tarım kararının yaklaşım konumundan başlayarak mesafe ve bölge değişimiyle sıralanır. Farklı bölge adayı 18 m içinde varsa tercih edilir; yoksa yakın aday kullanılır. Tamamlama sırası oyuncuya açıktır. Merkezi hedef, aktif uygun noktaların en yakınını önerir; tüm görevler ekranda listelenmez.

Çağrılar kriz başlangıcından **0 / 0 / 25 / 55 / 85 / 115 aktif saniye** önce açılamaz. Bunlar son tarihler değildir. Boş aktif yer varsa ve çağrı saati geldiyse sıradaki görev açılır; aynı anda en fazla iki iş vardır. Erken bitiren kısa süre yeni çağrı bekleyebilir; HUD kalan çağrı süresini gösterir. Mevcut acil olay bu sırada da 12. saniyede açılır. Görevler ayrı ayrı zaman aşımına uğramaz; tek genel 240 sn ve tek acil 45 sn penceresi korunur.

En az dört bakım yalnız bir koşuldur: üç karar, arızanın değerlendirilmesi, ≤35 baskı ve 10 aktif sn istikrar da gerekir. Her bakım -6 baskı verir; su işlerinin her biri +2 su, doğa işlerinin her biri +2 bitki verir. Altı işte en fazla +4 su/+4 bitki, toplam -36 baskı; bütçe/enerji/emisyon değişmez. Kaynaklar mevcut ResourceState sınırlarıyla korunur. Kötü stratejiler otomatik iyi finale dönüşmez; dizel emisyonu ve mevcut üç final formülü aynıdır.

## Merkezi etkileşim ve sunum

Menü/kart → acil arıza → stratejik karar → bakım önceliği korunur. Controller tek E sahibidir; node scripti input veya ödül üretmez. Tamamlama TaskId+LocationId eşleşmesini, node'un gerçek X/Z menzilini (1.65 m), modal/pause/focus durumunu, aktif görevi ve tek ödül kuralını denetler. Yan istasyondan veya yanlış node üzerinden tamamlanamaz. Acil olay bakım ödüllerini engeller; eski su istasyonu menzili ve 2 sn basılı onarım kullanılır.

Aktif noktada sarı işaret, tamamlananda yeşil onay; seçilmeyen/kilitli noktada yalnız küçük normal obje görünür. Acilde sarı bakım işaretleri gizlenir; kırmızı eski acil işareti öne çıkar. Kalıcı bakım kökü karar modellerinin kardeşidir; Solar/Pump/Protect seçimi onu kapatamaz. Node kimlikleri ve katalog konumları başlangıçta doğrulanır; elle yer değiştirildiğinde kaynak rotalarıyla sessiz uyumsuzluk yerine Console hatası oluşur.

HUD kısa hedef/bakım ilerlemesi, aktif hedefin açık kaldığı süre, ekran kenarına sıkıştırılan hedef etiketi ve kısa yeni çağrı/tamamlama bildirimi kullanır. Toplam süre son 60 sn'de altın, son 20 sn'de kırmızı ve kısa uyarıdır. Yeni ses/müzik paketi yok; mevcut bakım sesi kullanılır. Menü, ses tercihleri, kart/Esc/focus/native pause davranışı korunur. Restart/menu/replay; on node işaretini, seçili görevleri ve mevcut dünya/kamera/HUD durumunu sıfırlar.

## Gerçek doğrulama ve sınırlar

**178 test grubu geçti, 0 başarısız**: önceki 157 beklentileriyle korundu, 21 yeni grup eklendi. 54 setin çeşitliliği/bölgeleri, menzil/kimlik/modal/tek ödül, çağrı zamanları, öncelik, saatler, reset/replay ve eski RC profili test edildi. 27 strateji üç genel finale ulaştı; iki engelli sulamada mevcut depolama alternatifi uygulandı. **44 Unity C# dosyası ×5 sembol birleşimi**, C#9 sözdizimi kontrolü: 0 hata; Unity API/tür derlemesi değildir.

[Rota raporu](FINAL_PACING_BALANCE.md): 0.5 m düzlem grafiği, kaynaklardan tahmin edilen şişirilmiş engeller, yürüyüş/koşma, yön bulma/etkileşim/tepki payları ve gerçek olay saatleriyle ReleaseFlow/GameRunState birlikte yürütülür. Bütün işleri aynı anda bitmiş varsaymaz. Hızlı 87–90 sn, yeni oyuncu 155–175 sn; üç enerji yolu da 54/54 sette kriz başarısı sağlar. Geciken örnekler 240 sn'de geçerli kısmi sonuç üretir. Diğer stratejilerde kötü genel final de korunur. Bunlar garanti veya gerçek oyuncu ölçümü değildir.

Bilinen sınırlar: düzlem engel modeli gerçek collider'ların tamamını veya kamera örtüşmesini temsil etmez; hedef değişince yön bulma maliyeti tekrar uygulanabilir. İlk oyun, restart ve ana menü arka planı oturum sayacını tükettiğinden Unity'deki set sırası raporun seed 0 sırasıyla aynı olmak zorunda değildir; tüm 54 set taranmıştır. GUI taşıması, yakın hedefin oyuncu hareketiyle değişmesi, son süre uyarısı, ses, pause/focus, prefab/serileştirme ve Windows build gerçek test bekler. Gerçek 3–4 dakika hedefi iddia edilmez. P2 ek cilalar yerine kapsamda istenen basit işaretlerle yetinildi.

Yeni menü: **Tools > GameUP > Create Final Pacing Scene**. Benzersiz `Assets/SonBirUmutDemo/Scenes/SonBirUmutFinalPacing.unity` ve `Generated/FinalPacing...`; eski beş menü ve varlıklar korunur. [Hakan testi](FINAL_PACING_UNITY_TEST.md).

## Dosyalar

Yeni kaynaklar: `Scripts/Simulation/FinalPacingCatalog.cs`, `Scripts/Interaction/MaintenanceTaskNode.cs`, `Editor/DemoPacingBuilder.cs`, `Tests/LogicTests/FinalPacingTests.cs`, `Tests/LogicTests/PacingRouteSimulation.cs`.

Güncellenen kaynaklar: `MaintenanceTask.cs` (CrisisProgress dahil), `GameRunState.cs`, `ReleaseFlow.cs`, `DemoGameController.cs`, `DemoStory.cs`, `CrisisStationMarker.cs`, `DemoHud.cs`, `DemoSceneBuilder.cs`, test `Program.cs`.

Belgeler: README.md, UNITY_TEST_CHECKLIST.md, docs/ARCHITECTURE.md, INTEGRATION.md, TEST_RESULTS.md ve yeni FINAL_PACING.md, FINAL_PACING_BALANCE.md, FINAL_PACING_UNITY_TEST.md. Scripts/Editor yolları UnityAssets/SonBirUmutDemo altındadır. Karar kuralları, motor, arıza kuralları, ses yöneticisi ve önceki test dosyaları değiştirilmedi.
