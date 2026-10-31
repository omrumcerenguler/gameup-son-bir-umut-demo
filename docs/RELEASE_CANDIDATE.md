# Son Bir Umut: Kuraklıktan Önce — Release Candidate

Dal: `feature/release-candidate`. Kaynak/yedek: `feature/playtest-ux` / `391d253c0eabbd9283580e7e52fa463d28105d5e`. Bu teslim yeni kaynak entegrasyonudur; Unity Editor ve Windows EXE onayı henüz yoktur. Bu Mac'te Unity kurulu değildir.

## Korunan oyun

32×28 köy, aynı izometrik kamera, WASD/1.9× sprint, üç stratejik karar, beş bakım/en az dört, tek arıza/45 sn pencere/2 sn onarım, 240 aktif sn, 35 baskı altında 10 aktif sn, üç genel final ve kaynak/karar hesapları değiştirilmedi. Yeni bir GameMode eklenmedi; RC kabuğu mevcut PlaytestDemo kullanır. Eski dört sahne menüsü ve çalışan profiller korunur.

## Yeni sahne ve menü

**Tools > GameUP > Create Release Candidate Scene**

`Assets/SonBirUmutDemo/Scenes/SonBirUmutReleaseCandidate.unity`; varlıklar `Generated/ReleaseCandidate...`. Dolu yollar benzersiz numaralanır. Mevcut sahneler, Generated varlıkları veya .meta dosyaları silinmez/ezilmez. Ses kaydı veya ağır paket indirilmez; gerçek sahne bu Mac'te üretilmedi.

İlk açılışta köyün geniş izometrik görünümü üzerinde **SON BİR UMUT: KURAKLIKTAN ÖNCE / KRİZ NÖBETİ**, Yeni Oyun, Nasıl Oynanır, Ayarlar ve Çıkış. Normal HUD menü arkasında çizilmez. Yeni Oyun yeni oturum oluşturur; kısa mevcut Intro yönlendirmesi ve Başla/Enter ile oynanır. Yükle veya oyun kayıt sistemi yoktur.

## Akış ve bağımsız pause

`ReleaseFlow` saf C# UI durumudur; Intro/Preparation/CrisisManagement/Ending motorunu yeniden yazmaz.

| UI durumu | Oyun davranışı |
| --- | --- |
| MainMenu / HowToPlay / ana menü Settings | Başlangıç Intro; saat/baskı/arıza başlamaz, hareket kapalı |
| Playing + Intro | Kısa yönlendirme, hareket/saat kapalı; StartGameplay yalnız bir kez kabul edilir |
| Playing | Mevcut normal oyun; kart açılırsa aynı eski kilit/saat durması |
| Paused / pause Settings | Ayrı Run.UserPaused=true; onarım iptal, sayaç/baskı/arıza ve ödüller durur |
| Ending | Geçerli mevcut sonuç; yeni final sunumu, köyü izleme veya temiz yeni oturum |

Native `SetApplicationPaused`, focus `SetFocus` ve kullanıcı `SetUserPaused` ayrı bayraklardır. Kullanıcı Devam Et diyerek native pause'u kaldıramaz; native resume kullanıcı pause'unu iptal etmez. Dönüşte ilk uygun delta atılır. Time.timeScale veya global AudioListener durumu değiştirilmez.

Esc: açık kartı kapatır; serbest oyunda pause; pause'da devam; HowTo/Settings'te yalnız önceki menü. Intro ve finalde yeni pause açılmaz. Tek klavye koordinatörü, ekran/komut doğrulaması ve Time.frameCount başına bir UI işlemi koruması vardır. IMGUI dalı tıklamadan sonra döner; yeni panel/faz/focus geldiğinde GUI hot/keyboard control temizlenir. Gerçek mouse/keyboard olay sırası Windows testidir.

## Temiz yeniden başlatma

Yeni Oyun, Pause→Yeniden Başlat, Final→Yeniden Oyna ve Ana Menüye Dön yeni GameRunState/GameSession oluşturur. Çift NewGame veya Restart doğru ekrandan gelmiyorsa reddedilir. Ana Menüye Dön oynanışı başlatmaz; temiz Intro arka planı tutar. Dış focus/native pause bayrakları yeni oturuma uygulanır.

`ResetPresentation` tesis seçeneklerini kapatır, oyuncu/velocity/spawn, kaynak görünümü, kamera/offset/zoom, bildirim/kart ve işaretleri sıfırlar. WorldView yeni Session ile anında başlangıca döner. StationFeedback ve Atmosphere `RefreshNow` ile aynı GUI karesinde temizlenir; eski sızıntı/solar/onay/fog beklemez. Sesler durdurulur; ses tercihleri sıfırlanmaz. HUD yeni Session'da ayrıntı/scroll/izleme durumunu temizler.

Kod incelemesinde iki görsel entegrasyon köşesi ele alındı: kamera OnEnable'ın ilk ana menü geniş görünümünü geri alması önlendi; RC'de son karedeki kaynak/arıza değişiminin donmuş eski görselde kalmaması için final görünümü güncel kaynak/baskıyla snap edilir. Bunlar kaynak düzeyindeki düzeltmelerdir; Unity render doğrulaması değildir.

## Ses ve gerçek ayarlar

`SoundSynthesis` altı özgün kısa mono PCM efekti üretir: menü, karar, bakım, acil, iyi final, olumsuz final. 22050 Hz, kısa yumuşak zarf, sinüs/akorlar; başlatmada bir kez AudioClip.Create/SetData kullanılır. Tek 2D AudioSource, mevcut kamerada tek AudioListener; kaynak ses düzeyi/mute değiştirilir, ağır müzik/middleware veya lisansı belirsiz kayıt yoktur.

`SoundPreferences` sonlu ses düzeyini 0–1'e sıkıştırır; mute seçilen düzeyi kaybetmez. DemoAudioManager mevcut efektlere uygular, sliderda kısa preview verir. İki PlayerPrefs anahtarı yalnız ses/mute tutar. Toggle'da, ayarlardan çıkışta, native pause/uygulama kapanışında kirli tercihler kaydedilir; slider hareketinde her Layout/Repaint'te disk Save yapılmaz. Yeni oturum ses tercihlerini korur. Editor ve standalone tercih depoları farklı olabilir; kalıcılık aynı EXE'yi kapatıp yeniden açarak doğrulanmalı.

Clip oluşturma başarısızsa yalnız ses katmanı uyarı verir, oyun sessiz sürer ve ses kontrolleri açıklamayla pasiftir. `Available` clip hazırlığını gösterir; fiziksel hoparlör/aygıt doğrulaması değildir. Ses kapalıyken görsel acil panel/yön/bar ve bütün oyun mekanikleri bağımsız çalışır.

[AudioClip.Create](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/audioclip/create) · [PlayOneShot](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/audiosource/playoneshot) · [PlayerPrefs.Save](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/playerprefs/save). Bu resmi API kontrolü gerçek Unity derlemesi veya sesin duyulması değildir.

## Final ve çıkış

RC finalinde büyük sonuç başlığı, kısa doğal açıklama, güncel beş kaynak ve bakım/baskı özeti; uzun özet isteğe bağlı Ayrıntılar/kaydırmada. Köyü İzle / Yeniden Oyna / Ana Menü her iki final görünümünde bulunur. Kriz başarısı ile sürdürülebilirlik kategorisi ayrı kalır; köyün mevcut kaynak görünümü korunur. İklim değişikliğinin tamamen çözüldüğü söylenmez, büyük sinematik eklenmedi.

Çıkış standalone'da Application.Quit çağırır. Editor'da kapatılmak yerine Windows uygulaması davranışını belirten kısa bilgi gösterilir; [Unity bu çağrıyı Editor'da yok sayar](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/application/quit). Gerçek çıkış, EXE testinde onaylanmalıdır.

## Gerçekte doğrulananlar / bekleyenler

**157 test grubu geçti, 0 başarısız**: 136 korundu, 21 yeni. Menü/fresh start/tek başlatma, pause sayaç/baskı/arıza/ödül kilidi ve tamir iptali, Esc önceliği, focus/native pause birleşimi, menü dönüşü/restart/final/replay/ardışık oturumlar, beş/dört ve 45/2 sabitleri, ses sınırı/mute/PCM veri kalitesi. Menü katmanı üzerinden 27 strateji ve üç genel final de geçti; korunan eski/new Playtest 27 dizileri de çalışır. 157 sayısı test gruplarıdır; gerçek GUI/ses testi değildir.

**41 C# ×5 sembol** Roslyn C#9 parser taraması hatasız. Unity API/tür derlemesi, gerçek GUI tıklaması/focus, sahne referansları, kamera/fog/son-kare görselleri, sesin duyulması, PlayerPrefs disk kalıcılığı ve Windows çıkışı/build testi **yapılmadı**. Üç-dört dakika hedefi yeniden ölçülmedi; mevcut 195 aktif sn simüle rota korunur, menü/okuma/pause duvar saati daha uzundur. P0/P1 kaynak özellikleri uygulandı; doğrulanmış Windows teslimi için [Hakan listesi](RELEASE_CANDIDATE_TEST.md) tamamlanmalı.

## Değişen dosyalar

Yeni: Simulation/ReleaseFlow.cs, SoundPreferences.cs, SoundSynthesis.cs; Audio/DemoAudioManager.cs; Tests/LogicTests/ReleaseFlowTests.cs; bu belge ve RELEASE_CANDIDATE_TEST.md.

Güncellenen: GameRunState.cs, DemoGameController.cs, DemoStory.cs, CrisisStationMarker.cs, IsometricFollowCamera.cs, DemoHud.cs, DemoGuiTheme.cs, DemoWorldView.cs, DemoStationFeedback.cs, DemoCrisisAtmosphere.cs, Editor/DemoSceneBuilder.cs, test Program.cs ve README/ARCHITECTURE/INTEGRATION/TEST_RESULTS/UNITY_TEST_CHECKLIST. Scripts ve Editor, UnityAssets/SonBirUmutDemo altındadır. Eski karar kuralları, görev/arıza verisi, motor ve eski test dosyaları değiştirilmedi.
