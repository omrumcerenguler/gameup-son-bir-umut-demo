# Final Pacing — güncel Windows kurulumu

1. **Önce çalışan Unity projesini ve yerel düzenlemeleri yedekle.** RC kaynak dalı 5eeb35e korunur; yeni çalışma `feature/final-pacing`.
2. Temiz kaynak depoda `git fetch origin`, `git switch feature/final-pacing`, `git pull --ff-only origin feature/final-pacing`; tam commit'i kaydet.
3. UnityAssets/SonBirUmutDemo içeriğinin tamamını gerçek Unity projesinin Assets/SonBirUmutDemo klasörüne aktar. **Scripts ve Editor birlikte**; FinalPacingCatalog, MaintenanceTaskNode ve DemoPacingBuilder dahil. Eski .meta/Scenes/Generated silinmez. Tests/docs/kişisel belgeleri Assets'e kopyalama.
4. Unity 6.3 LTS, Built-in 3D/URP; Input Handling **Old veya Both**. Console derlemesi tamamlanmalı. Hata varsa ilk kırmızı hatayı kaydet; bu Mac'te Unity API derlemesi yapılmadı.
5. Play kapalı/Prefab dışında/açık sahneler temizken **Tools > GameUP > Create Final Pacing Scene**. Benzersiz Scenes/SonBirUmutFinalPacing.unity ve Generated/FinalPacing...; kaydet/kapat/yeniden açıp referansları kontrol et. Görev node'larını katalogdan bağımsız taşımak başlangıç doğrulamasında hata verir.
6. Play→Ana Menü→Yeni Oyun→Başla. WASD/Shift 1.9×; kararlar üç ana istasyonda, bakımlar on ayrı node'da yakın E. Seçili6 görevden en az 4; aynı anda en fazla 2. Acil sabit su noktasında 45 sn / 2 sn held; Esc/pause/ses ayarları korunur.
7. [FINAL_PACING_UNITY_TEST](FINAL_PACING_UNITY_TEST.md) listesini uygula; özellikle on fiziksel erişim, hedefler arasında hareket, gerçek zaman baskısı ve tekrar oynama. [Model rota raporunu](FINAL_PACING_BALANCE.md) gerçek süre ölçümü diye raporlama.
8. Windows Build Profiles Scene List'te **Final Pacing ilk ve etkin**. EXE'yi Editor dışında dene; EXE/_Data/UnityPlayer.dll ve bütün yan dosyaları birlikte tut. Tam Unity proje yedeğinde Assets/Packages/ProjectSettings ve gerekli bütün .meta dosyalarını koru; [ayrıntılı teslim rehberi](RELEASE_CANDIDATE_TEST.md).

Eski Create Release Candidate Scene 5/4 profilini, Playtest ve diğer üç menü kendi davranışını korur. Eski sahne otomatik yeni node almaz. Yeni render/input/audio paketi veya SDK kurulmadı.178 saf C# test grubu geçti; fizik/GUI/ses/Windows kabulü ekip testi bekler.

---

Aşağıdaki talimatlar **korunan RC ve önceki sahnelerin kurulumu içindir**. Yeni Final Pacing için yukarıdaki ayrı menü kullanılmalıdır.

# Release Candidate — Windows Unity entegrasyonu

Dal **feature/release-candidate**; korunan kaynak **feature/playtest-ux / 391d253c0eabbd9283580e7e52fa463d28105d5e**. Hedef Unity 6.3 LTS/6000.3.x, Windows, Built-in 3D veya URP. Bu Mac'te Unity Editor yoktur; Unity API derlemesi/oynanış/ses/EXE testi yapılmadı.

1. **Önce çalışan dış Unity projesini ve yerel düzenlemeleri yedekle.** Kaynak depoyla gerçek Unity projesi ayrı şeylerdir.
2. Temiz kaynak depoda aşağıdaki dalı al; yerel değişiklik varsa üzerine yazmadan koru:

```sh
git fetch origin
git switch feature/release-candidate
git pull --ff-only origin feature/release-candidate
```

3. `UnityAssets/SonBirUmutDemo` içeriğinin tamamını gerçek projenin `Assets/SonBirUmutDemo` klasörüne kopyala. **Scripts ve Editor birlikte**, yeni Audio/DemoAudioManager ile Simulation/ReleaseFlow, SoundPreferences, SoundSynthesis dahil. Klasörü, eski `.meta` GUID'lerini, Scenes veya Generated varlıklarını silme. Yeni dosyaların .meta'sını Editor üretir. Tests/docs/kişisel dosyaları Assets'e aktarma.
4. **Active Input Handling: Input Manager (Old) veya Both**; gerekirse Editor yeniden başlatılır. New-only mevcut IMGUI fare akışı için uygun değildir. Yeni UI/audio paketi gerekmez; HDRP desteklenmez.
5. Console import/derlemesi bitmeli; ilk kırmızı hatayı çözmeden ilerleme. Tools görünmüyorsa Editor klasörü kopyasını ve ilk derleme hatasını kontrol et.
6. Play kapalı, Prefab düzenleme dışında ve açık sahneler temizken **Tools > GameUP > Create Release Candidate Scene**. Yeni yol `Assets/SonBirUmutDemo/Scenes/SonBirUmutReleaseCandidate.unity`, varlıklar `Generated/ReleaseCandidate...`; dolu yollar numaralanır. Eski sahneler/varlıklar ezilmez. Sahneyi kaydet/kapat/yeniden aç ve referansları kontrol et.
7. Play → Ana Menü → Yeni Oyun → kısa yönlendirme → Başla/Enter. WASD,1.9×Shift, yakın E, arızada2sn E held. Esc önce kartı kapatır, serbest oyunda pause açar; alt menüde önce geri döner.
8. **[RELEASE_CANDIDATE_TEST](RELEASE_CANDIDATE_TEST.md)** listesini sırayla tamamla: gerçek ses/mute/kalıcılık, pause saatleri, üç karar/beş bakım/en az dört,45/2arıza, üç final/üç ardışık oturum ve1280×720/1920×1080.
9. Windows Build Profiles Scene List'te yeni **RC sahnesi ilk ve etkin** olsun. EXE'yi Editor dışında baştan sona dene; gerçek Çıkış yalnız standalone'da kapanır. **EXE + aynı isimli _Data + UnityPlayer.dll + bütün üretilen yan dosya/dizinleri** birlikte paketle.
10. Tam test edilmiş Unity proje yedeğinde **Assets/Packages/ProjectSettings ve bütün gerekli dosya/klasör .meta** dosyalarını koru. Library/Temp/Logs/cache/build çıktıları gerekmez. Kabul belgesindeki ayrı klasöre çıkarıp yeniden açma kontrolünü uygula.

Create Playtest Demo Scene ve önceki üç menü korunur; kaydedilmiş eski sahne otomatik RC olmaz. Yeni menü/pause/ses yalnız RC bayraklı sahnede çalışır. Karar ekonomisi, 240aktif sn, harita ve görev/arıza dengesi değişmedi.157 saf C# test grubu geçti; bu, yukarıdaki Unity/Windows kabulü değildir.

[RC uygulama ayrıntıları](RELEASE_CANDIDATE.md) · [Test kaydı](TEST_RESULTS.md)

---

Aşağıdaki metin **korunan Aşama 3 kurulumu içindir**; oradaki Playtest menüsü yeni RC menü/ses katmanını etkinleştirmez. Güncel RC kurulumu yukarıdadır.

# Aşama 3 — Windows Unity entegrasyonu

Yeni dal **feature/playtest-ux**; korunan Aşama 2 yedeği **feature/live-village-demo / 1912d94**. Hedef Unity 6.3 LTS (6000.3.x), Windows, Built-in 3D veya URP. Bu Mac'te Unity Editor yoktur; yeni sürüm derlenmedi/oynanmadı.

## Kaynakları al

Önce çalışan dış Unity projesini ve yerel değişiklikleri yedekle. Kaynak depon temizken:

```sh
git fetch origin
git switch feature/playtest-ux
git pull --ff-only origin feature/playtest-ux
```

1. Mevcut Unity 6.3 LTS projesini kullan. Depo doğrudan Unity projesi değildir; başka proje gerekiyorsa depodan ayrı Built-in/URP proje aç. HDRP desteklenmez.
2. `UnityAssets/SonBirUmutDemo` içeriğini `Assets/SonBirUmutDemo` üzerine klasörü silmeden aktar. Yeni **DemoPlaytestBuilder, DemoStationFeedback, AtmosphereProfile** dahil tüm Scripts ve Editor birlikte olmalı.
3. Mevcut `.meta`, Scenes/Generated ve yerel düzenlemeleri koru; yeni scriptlerin .meta'sını Editor üretsin. Tests/docs/kişisel belgeleri Assets içine kopyalama.
4. **Active Input Handling: Input Manager (Old) veya Both.** Both mevcut Input System paketiyle kullanılabilir; yeni paket gerekmez. New-only IMGUI fare tıklamaları için uygun değildir. İstenirse Editor yeniden başlatılmalı; input sembollerini elle ekleme.
5. Console import/derlemesi bitmeli. Tools görünmüyorsa önce ilk kırmızı hatayı ve Editor kopyasını kontrol et; bu Mac'te Unity API derlemesi yapılmadı.

## Yeni sahneyi oluştur

Play kapalı, Prefab düzenleme dışında ve açık sahneler temizken:

**Tools > GameUP > Create Playtest Demo Scene**

Yeni yol `Assets/SonBirUmutDemo/Scenes/SonBirUmutPlaytestDemo.unity`; varlıklar `Generated/PlaytestDemo...`. Dolu yollar benzersiz numaralanır. Eski sahne/materialler ezilmez. Sahneyi kaydet/kapat/yeniden aç ve marker/feedback/terrain/kanal/solar referanslarını kontrol et.

Eski Create Demo Scene / Create Crisis Foundation Scene / Create Crisis Demo Scene menüleri korunur. Aşama 3 beş görevli profil ve geniş harita için **Playtest** menüsü gereklidir; eski CrisisDemo sahnesini açmak yeni düzeni eklemez. Bu daldaki ortak HUD/atmosfer değişimleri eski sahnelerde de görülür; birebir yedek 1912d94 kaynakları ayrı test projesinde denenmelidir.

## Oynanış ve değerler

1280×720, Play, Enter. WASD, Sol Shift1.9×, yakınken E; arızada2sn E held. Enerji kökü (8,0,1), su (−8,0,8), tarım (8,0,−7). Üç kararı sırayla ver; ardından en fazla iki açık bakım arasında hareket et. Beş varyasyonun her biri yalnız bir kez ödül verir. Dört bakım + arızanın onarım/zaman aşımıyla değerlendirilmesi + ≤35 baskıda10 aktif sn erken başarıdır; beşinci bakım isteğe bağlı.

Sayaç240, başlangıç baskısı60; arıza gecikmesi12, penceresi45, tamir2; hız3.5/sprint1.9/hızlanma12/fren16/dönüş600. Ayarlar Play başında veya Yeniden Oyna'da okunur. Giriş/kart/odak dışı/native pause saatleri durur; onarımda ilerler. Yeni duraklatma menüsü yoktur.

- Solar seçildiğinde ışık; dizel/tasarrufta solar çalışma işareti yok. Depo süzgeci ve ortak tasarruf vanası kalıcı altyapı; seçilmemiş pompa/sulama tesisinin çalıştığını göstermez.
- Arızada su damlaları, onarımda durmuş sızıntı/yeşil hat, kaçırmada kırmızı kayıp izi. Temsilî görseldir; ek süreklilik su hesabı yaratmaz.
- Baskı yüksekken uzaktaki pus/solgunluk artar; oyuncu/yan yol/işaretler okunmalı. Use Basic Fog kapalıysa yalnız ışık/ambient alternatifi kalır; uzak nesneyi örtmez.
- Bilimsel emisyon/sıcaklık veya fiziksel iklim temizliği iddiası yoktur. Stratejik hesaplar ve emisyonun genel finale etkisi korunur.

## Teslim öncesi kontroller

[PLAYTEST_UNITY_TEST](PLAYTEST_UNITY_TEST.md) tüm adımları içerir. Özellikle 30/60/120 FPS, diagonal/sprint/follow, üç bölgenin fiziksel erişimi, arıza45/2, bırakma/menzil/focus, aktif en fazla iki görev ve tekrar ödül engeli, beş görevli final scroll'u, iki replay, iki çözünürlük ve eski menü/sahne regresyonunu dene. Console/Player.log'un ilk hatasını, commit/patch/pipeline/input ve gerçek süreleri kaydet.

Görüş için Initial Pressure20/60/100 ayrı Play denemeleri yapılabilir; bu ayarlar dengeyi değiştirir. Sonunda60 ve varsayılan değerleri geri yükle. 5 sn süreyle 0/1/2 kararda güvenli sonucu kontrol et; sonra240'a dön. Saf simülasyonda195 aktif sn, gerçek oyuncu süresi değildir.

Build Profiles / Windows / Scene List içinde **yeni PlaytestDemo** başlangıç olmalı. EXE'yi Editor dışında çalıştır; EXE, _Data, UnityPlayer.dll ve üretilen gerekli yan dosyaları birlikte koru. Build/cache'i depoya ekleme. Bu Mac'e Unity veya SDK kurulmadı.

[UX ve harita tasarımı](PLAYTEST_UX.md) · [Denge](PLAYTEST_BALANCE.md) · [Test kaydı](TEST_RESULTS.md)
