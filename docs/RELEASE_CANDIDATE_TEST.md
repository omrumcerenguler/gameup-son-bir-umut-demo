# Hakan — Release Candidate son doğrulama ve Windows teslimi

**Bütün kutular bekliyor.** Mac'te Unity Editor, GUI veya Windows build çalıştırılmadı. Dal **feature/release-candidate**, kaynak referans 391d253.157 saf C# testinin geçmesi aşağıdaki gerçek testlerin yerine geçmez.

## 1. Çalışanı koru, doğru kaynakları al

- [ ] Önce çalışan Unity projesini kapatıp ayrı konuma yedekledim; Assets/Packages/ProjectSettings ve .meta korunuyor. Yerel düzenlemeleri üzerine yazmadan korudum.
- [ ] Temiz kaynak depoda `git fetch origin`, `git switch feature/release-candidate`, `git pull --ff-only origin feature/release-candidate`. Tam commit aşağıya kaydedildi. Eski dalları reset/merge/force etmedim.
- [ ] UnityAssets/SonBirUmutDemo içeriğinin tamamını dış projemin Assets/SonBirUmutDemo üzerine klasörü silmeden aktardım. **Scripts ve Editor birlikte**, yeni Audio ve Simulation sınıfları dahil. Mevcut .meta GUID'leri korunuyor; yeni scriptlerin .meta'sını Editor üretti.
- [ ] Unity **6.3 LTS/6000.3.x**, Built-in 3D veya URP; Active Input Handling **Old veya Both**. New-only IMGUI için uygun değil; gerekirse yeniden başlatıldı. Yeni paket zorunlu değil.
- [ ] Console derlemesi bitti. İlk kırmızı hata/Missing Script yok; varsa tam hata ve adımı kaydettim, test geçmiş saymadım.

## 2. Yalnız yeni RC sahnesini oluştur

- [ ] Play kapalı, Prefab düzenleme dışında ve açık sahneler temizken **Tools > GameUP > Create Release Candidate Scene**.
- [ ] Yeni `Assets/SonBirUmutDemo/Scenes/SonBirUmutReleaseCandidate.unity` ve `Generated/ReleaseCandidate...`; dolu yollar numaralı benzersiz. Eski sahne, mesh, materyal ve .meta ezilmedi.
- [ ] Controller release anahtarı, Playtest profili, HUD, kamera, WorldView/Feedback/Atmosphere, üç marker ve AudioManager/AudioSource referansları dolu; tek AudioListener kamerada. AudioSource Play On Awake kapalı ve2D.
- [ ] Sahneyi kaydedip kapat/yeniden açtım; referanslar, materyaller/mesh'ler ve gizli release bayrağı korunuyor. İkinci araç çağrısı ayrı dosya üretiyor; kirli sahnede araç duruyor.

## 3. İlk açılış / ana menü

- [ ] Play: köy geniş izometrik arka planda, hafif pus. Başlık **SON BİR UMUT: KURAKLIKTAN ÖNCE**, alt başlık **KRİZ NÖBETİ**. Başka oyunun logo/varlığı yok.
- [ ] Yeni Oyun / Nasıl Oynanır / Ayarlar / Çıkış görünür ve tıklanabilir; normal HUD menü arkasında yok. 30 sn bekleyince süre240/baskı60/görev/arıza/karakter başlamıyor.
- [ ] Nasıl Oynanır: WASD/Shift/E/E held, üç karar, **beş görev/en az dört**, 35baskı/10sn ve üstte kalan süre kısa/anlaşılır. Geri/Esc yalnız ana menüye dönüyor, oturum başlamıyor.
- [ ] Yeni Oyun bir temiz Intro oluşturuyor; Başla/Enter ile oynanıyor. Hızlı çift tıklama/Enter+fare aynı oturumu iki kez başlatmıyor. Başlangıç oyuncu/240/60/kaynaklar/0karar/0görev doğru.
- [ ] İlk kamera geniş görünümü bir kare sonra dar takibe geri kaçmıyor. Yeni Oyun'da normal takip, ana menüye dönüşte geniş merkez görünüm geliyor.
- [ ] Editor'da Çıkış uygulamayı kapatmıyor, açıklama veriyor; bu beklenen durum. Gerçek çıkış EXE adımında denenmeli.

## 4. Ses ve gerçek ayarlar

- [ ] Menü seçimi, karar, bakım, acil, Yeniden Doğuş ve olumsuz final efektleri **gerçekte duyuldu**. Kısa/rahatsız etmeyen seviyede; kayıp clip/audio device uyarısı yok.
- [ ] Master0→sessiz,1→yüksek, ara değer→ölçülü; slider aktif sesleri de değiştiriyor. Mute aç/kapat seçilen seviyeyi kaybettirmiyor.
- [ ] Mute ile acil görsel panel/yön/geri sayım ve onarım barı hâlâ anlaşılır; hiçbir oyun şartı sese bağlı değil.
- [ ] Yeni Oyun, Pause→Restart ve Ana Menü sonrası ses/mute tercihi korunuyor. Kaynaklar sıfırlanırken tercihler sıfırlanmıyor.
- [ ] Ayarlardan Geri/Esc, uygulama kapanışı ve tekrar açılış: tercih aynı uygulamada korunuyor. **EXE kapat/yeniden aç** ile ayrıca test edildi; Editor ile EXE PlayerPrefs depolarının aynı olmasını beklemedim.
- [ ] Pause/focus kaybında eski oyun sesi sürmüyor; menü tıklama sesi çalışabiliyor. Ses oluşturma hatası olursa yalnız sesin kapandığı açıkça görülüyor, temel oyun devam edebiliyor; hata kaydedildi.

## 5. Mevcut oynanış

- [ ]32×28 üç bölge/izometrik köy korunuyor. WASD ve1.9×Shift, normalize çapraz, dönüş/kamera/çarpışma/sınır doğru; yeni harita veya denge değişikliği yok.
- [ ] Enerji→su→tarım kartları; kısa metin/gerçek etkiler/yetersiz seçenek. Kart açıkken saat/karakter duruyor, Esc yalnız kartı kapatıyor ve aynı basış pause açmıyor.
- [ ] Beş bakım varyasyonu, en fazla iki açık, en az dört şartı; aynı ID tekrar ödül vermiyor. İstasyonlara dönüş ve pasif süzgeç/ortak vana seçilmeyen tesisi çalışır göstermiyor.
- [ ] Tek su arızası:45sn pencere,2sn E held. Bırakma/uzaklaşma/focus iptal; normal onarımda saat işler. Ayrı turda bilerek kaçırdım: tek ceza ve geçerli devam/final.
- [ ] Solar/dizel/tasarruf, su ve bitki değişimleri, sızıntı/tamir izi, baskı/fog ve yakın yol/karakter/işaret görünürlüğü korunuyor. Kaynak değişimi son kareye denk gelse de final görünümü güncel değerle uyuşuyor.

## 6. Gerçek Esc pause ve bütün geçişler

- [ ] Serbest Preparation ve CrisisManagement'ta Esc → Duraklatıldı; dört buton.30sn bekle: karakter, ana sayaç, baskı, arıza geri sayımı, görev/kaynak değişmiyor.
- [ ] Onarım1sn ilerlemişken pause: bar sıfırlanıyor. Devam sonrası eski1sn'yi kullanıp anında tamamlamıyor; yeniden2aktif sn gerekiyor.
- [ ] Pause→Devam ve pause'da Esc; Ayarlar→Esc yalnız pause'a dönüyor, ikinci Esc devam ediyor. Aynı Esc/tıklama iki işlemi tetiklemiyor.
- [ ] Pause+AltTab/native suspend+kart kombinasyonları; resume native pause'u, native resume kullanıcı pause'unu kaldırmıyor; dönüşte delta sıçraması yok.
- [ ] Pause→Yeniden Başlat: yeni Intro; kaynaklar/240/60/0karar/0görev/arıza/onarım/istikrar sıfır. Pause→Ana Menü: temiz başlangıç arka planı; oynanış kendiliğinden başlamıyor.
- [ ] Normal kart, pause, alt menü ve focus dönüşünde eski slider/button keyboard focus'u yeni panelde işlem başlatmıyor. Fare sürüklemesi/E/Enter/Esc/Space hızlı ardışık denendi.

## 7. Final ve temiz tekrar

- [ ] Başarılı/kısmi/başarısız üç genel final denendi. Dizel kriz başarısı otomatik Yeniden Doğuş vermiyor. Başlık/kısa açıklama/güncel kaynaklar hemen anlaşılır; iklim değişikliğinin bittiği söylenmiyor.
- [ ] Köyü İzle / Yeniden Oyna / Ana Menü hem sonuç panelinde hem köyü izleme şeridinde çalışıyor. Ayrıntılar dikey kayıyor; normal sonuç teknik rapor gibi görünmüyor.
- [ ] Final→Yeniden Oyna ve Final→Ana Menü→Yeni Oyun; en az üç ardışık tur. Tesis modelleri, sızıntı/onarım izi, onay, su/bitki/toprak/fog, kamera/offset/zoom, HUD/scroll/ayrıntı, onarım, görevler ve sayaç eski turdan kalmıyor.
- [ ]0/1/2karar, eksik bakım veya aktif arızayla süre sonu da geçerli; boş sonuç/null hata veya sonsuz bekleme yok. Kısa süre testinden sonra240'a döndüm.
- [ ] Eski Playtest ve diğer üç Editor menüsü/sahnesi kendi akışını koruyor; yeni ana menü/ses yalnız RC sahnesinde. Birebir kaynak yedeği391d253 korunuyor.

## 8. UI ölçüleri / build

- [ ] **1280×720 ve1920×1080**: ana menü butonları, merkez pause/alt menüler, Türkçe karakterler, kartlar, slider, final/kaydırma ve mouse hit alanları taşmıyor. Menülerin arkasında HUD yok; normal sade HUD korunuyor.
- [ ] İlk kez oynayan bir kişiyle gerçek aktif/toplam süre ölçüldü; eski195sn simülasyon gerçek oyuncu süresi diye raporlanmadı. Commit/patch/pipeline/input/FPS ve ilk Console/Player.log hatası kaydedildi.
- [ ] Player Settings Product Name doğru Son Bir Umut kimliğinde; pencere başlığı ve teslim klasörleri başka oyunla karışmıyor. Bu ayar kaynak aracı tarafından otomatik değiştirilmedi.
- [ ] **File > Build Profiles / Windows**, **Scene List** içinde yeni **ReleaseCandidate** sahnesi etkin ve **ilk**; yanlış Playtest/Foundation ilk sahne değil. Mevcut Windows build modülleriyle build alındı.
- [ ] EXE Unity Editor dışında açıldı: ana menü→ayarlar/how-to→oyun→pause→üç karar/bakım/arıza→final→replay/main→çıkış baştan sona test edildi. Çıkış pencereyi gerçekten kapatıyor.
- [ ] EXE'yi tek göndermedim: **aynı isimli _Data klasörü, UnityPlayer.dll ve Unity'nin ürettiği diğer bütün gerekli yan dosyaları** içeren build çıktı klasörünü birlikte korudum. Backend'e ait MonoBleedingEdge/diğer dizinler varsa silmedim. Teslim ZIP'i çıkarılıp ayrı klasörde EXE tekrar çalıştırıldı.

## 9. Tam Unity projesini güvenle arşivle

Bu Git deposu yalnız kaynak teslimidir; gerçek Unity projesi değildir. Test edilmiş dış projeyi Editor kapalıyken arşivle:

- [ ] Proje kökünden **Assets, Packages, ProjectSettings** klasörlerinin tamamı alındı. Assets içindeki bütün sahne/Generated mesh/materyal/audio script ve **dosya/klasör .meta** dosyaları korundu. Yalnız Scripts kopyasıyla tam proje yedeği yapılmadı.
- [ ] Packages/manifest.json, packages-lock.json ve gerekli embedded package kaynakları korundu. Private erişim tokenları veya kişisel belgeler paylaşılmadan önce denetlendi.
- [ ] **Library, Temp, Logs, obj/bin, .git, UserSettings ve geçici build/cache** proje arşivine gerekmez. Gerçek Assets/Generated içeriğini build önbelleği sanıp çıkarmadım.
- [ ] Çalışan asıl projeyi silmeden arşivin bir kopyasını başka klasöre çıkarıp Unity 6.3 LTS ile yeniden açtım; import, RC sahnesi ve referanslar doğrulandı. Bu denetim Mac'te yapılmadı.

Windows PowerShell'de **doğru Unity proje kökünde**, hedef dosya mevcut değilken örnek:

```powershell
Compress-Archive -Path .\Assets,.\Packages,.\ProjectSettings -DestinationPath ..\SonBirUmut_UnityProject_RC.zip
```

Build teslim ZIP'i ile bu kaynak/proje ZIP'i ayrı tutulmalıdır. Hangi commit, Unity patch ve build backend'in test edildiğini yanına yaz.

| Test kaydı | Değer |
| --- | --- |
| Test eden / tarih / branch / tam commit | |
| Unity patch / Windows / pipeline / input | |
| RC sahne yolu / çözünürlük / FPS | |
| Ses/mute/PlayerPrefs/cihaz sonucu | |
| Oynanış / pause / ardışık oturum / final sonucu | |
| Aktif süre / toplam süre | |
| İlk Console/Player.log hatası ve adımı | |
| EXE açılış/çıkış / teslim ZIP / tam proje yeniden açma | |
