# Mimari — Kriz Nöbeti Aşama 2/2

Unity 6.3 LTS / Windows. Kaynaklar `UnityAssets/SonBirUmutDemo`; dış Windows test projesinin Assets klasörüne aktarılır. Gerçek Unity projesi/sahne/.meta Mac'te üretilmedi. Yeni oynanış `GameMode.CrisisDemo` ile açılır; eski LegacyDecisions ve CrisisFoundation modları korunur.

## Kaynak ve sorumluluk sınırları

| Sınıf | Sorumluluk |
| --- | --- |
| GameSession / DecisionCatalog / SimulationRules | Değişmeyen stratejik sıra, önizleme, kaynak/tur/kuraklık hesabı ve geçmiş |
| ResourceState / EndingResult / EndingSummary | Değişmeyen kaynak sınırları ve uzun vadeli kategori/anlatı değerlendirmesi |
| GameRunState | Mod/faz, bağımsız süre/baskı, görev/arıza sonucu, istikrar ve sonlandırma |
| GameRunSettings / SessionTimer / CrisisPressure | Aşama 1'den korunan ayarlar, aktif süre ve 0–100 gösterge |
| CrisisRules (yeni) | Doğrulanan eşik, istikrar, arıza gecikmesi/penceresi ve basılı tamir süresi |
| MaintenanceTask / MaintenanceEffects / CrisisProgress (yeni) | Üç veri odaklı, tek ödüllü görev; durum, istasyon, baskı ve sınırlı kaynak desteği |
| EmergencyIncident / EmergencyStatus (yeni) | Tek deterministik Waiting → Active → Repaired/Expired olayı; iptal edilebilir basılı tutma |
| InteractionPolicy / InteractionKind (yeni) | Modal → acil → strateji → bakım önceliğinin saf politikası |
| RunOutcome | Güncel kriz kaynakları ve yönetim başarısıyla final/özet; eski mod için eski sonuç |
| DemoGameController | Tek girdi koordinatörü; menzil, hedef önerisi, görsel güncelleme ve yeni modu kurma |
| DemoInput / DemoPlayerMotor / MovementPolicy | Old/Both E pressed/held, WASD/Shift ve korunan CharacterController hareketi |
| EnergyStation / DecisionStation | Korunan menzil ve seçilmiş model grupları; görev ödülü veya ayrı input Update yok |
| CrisisStationMarker (yeni) | Kalıcı istasyon kökünde sarı bakım/kırmızı arıza sunumu; yalnız durum okur |
| DemoHud / DemoStory / DemoGuiTheme | Güncel kaynak, görev/arıza/istikrar, odak kartı, sonuç, yön etiketi ve Türkçe metinler |
| DemoWorldView / DemoCrisisAtmosphere | Kaynaktan su/bitki görünümü ve baskıdan hafif atmosfer; hesap üretmez |
| DemoSceneBuilder / DemoVisualBuilder | Üç ayrı menü, benzersiz sahne/Generated varlıkları, kalıcı serileştirilmiş referanslar |

Simulation ve Decisions UnityEngine'den bağımsızdır. Test projesi bu dosyaları doğrudan derler; hesapların kopyası yoktur. UnityEditor kodu yalnız Editor klasöründedir. Yeni paket/Rigidbody/animasyon motoru eklenmedi.

## Stratejik snapshot ile güncel kriz kaynakları

Üçüncü seçimde GameSession stratejik Ending/Summary üretir. Bunlar geçmişe aittir ve değiştirilmez. `GameRunState.CurrentResources`, kriz başlayana kadar Session.State, sonra kontrollü ayrı immutable ResourceState'tir. Bakım yalnız bir kez su +4 veya bitki +4 verebilir; arıza kaçırma yalnız bir kez su -6 uygular. Bütçe/enerji/emisyon aynı kalır; bütün sınırlar ResourceState tarafından uygulanır.

HUD `game.CurrentResources` okur. Kontrolcü kabul edilen bakım/arıza kaynağı değişince aynı güncel durumu `DemoWorldView.Apply`'a iletir. Baskı artış hızı da güncel enerji/su/bitkiden hesaplanır. Final **yeniden** `EndingResult.Evaluate(CurrentResources)` ve `new EndingSummary(CurrentResources, Session.History)` kullanır; eski cache'i güncelmiş gibi sunmaz.

## Faz, zaman ve tek etkileşim sahibi

- Intro → Preparation; yalnız sıradaki stratejik karar seçilebilir.
- LegacyDecisions: üçüncü karar → Ending. CrisisFoundation: üçüncü karar → CrisisManagement, yalnız süre sonunda Ending.
- CrisisDemo: üçüncü karar → CrisisManagement; üç bakım açılır, arıza planlanır. Görevler bağımsız sırada E basışıyla anında uygulanır. Tekrarın ikinci etkisi yoktur.
- Acil olay aktifken bütün E etkileşiminin sahibi odur; su menzili dışında oyuncu yönlendirilir. E basılı ve menzilde gerçekten geçen aktif süre onarımı ilerletir. Bırakma/uzaklaşma/odak veya native pause/modal ilerlemeyi sıfırlar.
- GameController dışında hiçbir istasyon/görev input tüketmez. Onarım tamamlanan karede bakım çalışmaz; E bırakılıp yeniden basılmalıdır.
- Giriş/kart/odak dışı/native pause sırasında tüm simülasyon saati durur. Onarım ve istikrar aktif oyun süresine dahildir. Odak dönüşündeki ilk delta atılır; Time.timeScale yazılmaz.

GameRunState.Tick, arıza tetiklenmesi, tamir/deadline, eşik geçişi ve istikrar bitişi sınırlarında deltasını böler. Onarım başlamadan önceki süre ödünç alınmaz; 45 saniyelik olay penceresinden sonraki delta onarımı tamamlayamaz. Tam deadline anında yeterli onarım tamamlanırsa başarılıdır. Son global süre kadar zaman tüketilir. Süre sonu ile başarı aynı anı bulursa gerçekten sağlanmış başarı önceliklidir.

## Denge ve final ayrımı

Varsayılanlar: 240 aktif sn, başlangıç baskısı 60; ≤35 baskı, 10 aktif sn. Bakım rahatlamaları 12/14/14; su/bitki desteği birer +4. Tek arıza, krizden 12 aktif sn sonra (çok geç hazırlıkta kalan sürenin dörtte biriyle sınırlı); 45 sn müdahale penceresi, 2 sn onarım. Tamir -18 baskı; kaçırma +8 baskı ve su -6. Değerler oyun dengelemesidir.

Erken başarı yalnız strateji tamamlanmış + en az iki bakım + arıza Repaired veya Expired + düşük baskıda kesintisiz 10 aktif saniyedir. Üçüncü bakım ek destek sağlar; bir görevle kazanılamaz. Tekrarlı bakım/olay veya yeni demo modunda genel TryReducePressure çağrısıyla ödül döngüsü açılmaz. Eski Foundation azaltma API'si uyumluluk için korunur.

Kriz başarısı, köyün uzun vadeli kalitesi değildir. Mevcut kaynak/emisyon/bütçe eşikleri kategori üretir; kriz başarılamamışsa Yeniden Doğuş en çok Eksik Dönüşüm'e iner. Kaybedilen Gelecek kaynak koşulları korunur; kriz başarılı olsa bile yetersiz stratejik stoklar bu genel sonucu verebilir. Dizel ile kriz başarısı mümkündür, en iyi emisyon finali otomatik verilmez. Süre sonunda 0/1/2 karar, eksik bakım veya aktif arıza geçerli sonuç üretir.

[27 tam oturum ve üst sınır hesabı](CRISIS_BALANCE.md). Üç enerji yolunda da başarı; kötü karar/eksik müdahale örneği de rapordadır. Gerçek 3–4 dakika süresi ölçülmedi.

## Görsel ve eski sahne güvenliği

CrisisStationMarker, ana istasyonun üzerinde; işaret grupları seçenek görsellerinin kardeşidir. DecisionStation.ShowChoice kapalı pompa/vanayı gizlese de görev bileşeni ve menzil kaybolmaz. İşaretler çarpışmasızdır. Ekran dışı hedef etiketi görünür alana sıkıştırılır. Kriz azalınca mevcut atmosfer açılır; fiziksel iklim temizliği iddiası yoktur.

Create Demo Scene ve Create Crisis Foundation Scene korunur. Create Crisis Demo Scene yalnız yeni CrisisDemo sahnesini/Generated klasörünü benzersiz yolda üretir; mevcut sahne/material/meta'yı ezmez. Kirli sahne/Prefab/Play/input/pipeline korumaları sürer. Script referansları SerializeField alanlarıdır; kaydet/yeniden aç doğrulaması Unity'de bekler.

[Tam Windows/EXE test planı](FINAL_UNITY_TEST.md) · [Gerçek test kaydı](TEST_RESULTS.md). API derlemesi, fiziksel erişim, gerçek tuş/menzil, render/okunabilirlik ve Windows build bu Mac'te doğrulanmadı.

## Aşama 2 dosya listesi

Başlangıç `f60b0b1` karşısında toplam **22 dosya**:

| Alan | Dosyalar |
| --- | --- |
| Scripts/Simulation | GameRunState.cs, RunOutcome.cs; yeni CrisisRules.cs, MaintenanceTask.cs (CrisisProgress/Effects dahil), EmergencyIncident.cs, InteractionPolicy.cs |
| Scripts/Core | DemoGameController.cs, DemoInput.cs, DemoStory.cs |
| Scripts/UI | DemoHud.cs |
| Scripts/Interaction | Yeni CrisisStationMarker.cs |
| Editor | DemoSceneBuilder.cs |
| Tests/LogicTests | Program.cs, yeni CrisisDemoTests.cs |
| Belgeler | README.md, UNITY_TEST_CHECKLIST.md, docs/ARCHITECTURE.md, docs/INTEGRATION.md, docs/TEST_RESULTS.md, docs/LIVE_VILLAGE_STAGE1.md (tarihsel kayıt notu), yeni docs/CRISIS_BALANCE.md, docs/FINAL_UNITY_TEST.md |

Scripts/Editor yolları UnityAssets/SonBirUmutDemo altındadır. DemoWorldView ve DemoCrisisAtmosphere dosyaları korunmuştur: denetleyici güncel kaynağı mevcut Apply metoduna iletir; atmosfer mevcut baskıyı okumaya devam eder. Sahne/mesh/materyaller burada üretilmemiştir.
