# Hakan — Final Polish / Windows son kabul testi

**Bütün kutular bekliyor.** Mac'te Unity Editor, gerçek dinleme/GUI/fizik/Windows build yapılmadı.192 saf C# test grubu ve WAV sayısal kontrolleri aşağıdakilerin yerine geçmez. Testin gerçek tarihini aşağıya yaz; bu belge test yapılmış gibi tarih atmaz.

1. [ ] **Çalışan gerçek Unity projesini önce yedekle.** Yerel değişiklikleri koru; temiz kaynak depoda `git fetch origin`, `git switch feature/final-polish`, `git pull --ff-only origin feature/final-polish`. Tam commit'i kaydet. Korunan kaynak 747c08a; eski dallara reset/merge/force yok.
2. [ ] UnityAssets/SonBirUmutDemo **tamamını** Assets/SonBirUmutDemo içine aktar: Scripts + Editor + **Audio'daki üç WAV**. Eski .meta GUID'leri, Scenes/Generated silinmez. Tests/tools/docs/kişisel dosyaları Assets'e kopyalama. Yeni dosyaların .meta'sını Editor üretir.
3. [ ] Unity **6.3 LTS/6000.3.x**, Built-in veya desteklenen URP; Input Handling **Old veya Both**, gerekirse yeniden başlat. Console derlemesi bitti; ilk kırmızı hata/Missing Script yok. Unity API derlemesi burada ilk kez doğrulanıyor.
4. [ ] Play kapalı/Prefab dışında/açık sahneler temizken **Tools > GameUP > Create Final Polish Scene**. Yeni FinalPolish sahne/Generated yolu benzersiz; tekrar çalıştırma eskileri ezmiyor. Kaydet/kapat/yeniden aç:10 node, oyun/görsel/ses referansları dolu. Tam sesli sahnede **4 AudioSource (1 efekt+3 loop),1 AudioListener**; tekrar Play/restart sonrası sayı artmıyor. WAV importu mono/PCM/DecompressOnLoad/preload/22050. Eksik ses halinde uyarı ve temel oyunun devamı kontrol edildi.
5. [ ] Ana menü başlık/alt başlık/Nasıl Oynanır/Ayarlar/Yeni Oyun/Çıkış doğru.30sn beklerken sayaç/baskı/oyuncu başlamıyor. HowTo10nokta/6görev/4asgari/240sn/45-2 ve ilk enerji hedefini anlaşılır gösteriyor. Editor Çıkış açıklaması normal; gerçek çıkış EXE'de test edilecek.
6. [ ] Menü müziğini **gerçekte dinle**: sakin, vokalsiz, rahatsız edici sabit ton yok. En az üç 12 sn döngüde birleşim/click/kesilme, rüzgâr/uğultuda üç 8 sn döngü denendi. Master/müzik/efekt-ortam0/ara/1 değerleri yalnız ilgili kaynakları etkiliyor. Mute hepsini kapatıyor; unmute düzeyleri koruyor. Kısa altı cue doğru anda, tek kez ve taşmadan duyuluyor. Gerekirse müzik0 ile çalış; sonucu kaydet.
7. [ ] Yeni Oyun→Intro→Başla/Enter: müzik geçişi yumuşak, loop/source çoğalmıyor. Temiz240/60/kaynak/0karar/0görev başlangıcı ve ilk enerji hedefi. Çift tıklama/Enter+fare iki oturum başlatmıyor.
8. [ ] WASD, normalize çapraz,1.9×Shift, kamera/karakter/sınır ve yakın E. Kartta hareket ve saat duruyor; Esc yalnız kartı kapatıyor. Fare/klavye focus ve hızlı ardışık giriş güvenli.
9. [ ] Enerji→Su→Tarım üç karar, yetersiz seçenek/önizleme/görsel/karar sesi doğru. Her enerji seçimi için ayrı rota; dizel emisyonu bakım ile silinmiyor.
10. [ ] Oturumda6 seçili bakım (her bölgeden2), en fazla2aktif/en az4. Çağrı zamanları0/0/25/55/85/115; yanlış konum/TaskId/uzak E ve tekrar ödül engelleniyor. Yeni hedef ve bakım tamamlandı bildirimi/yeşil onay/ses anlaşılır. Pasif objeler ekranı doldurmuyor.
11. [ ] On fiziksel noktanın tamamına farklı turlarda ulaştım; bina/kanal/kuyu/depo/gövde/işaret kapanması yok. Sis/baskı20/60/100 denemelerinde karakter, yol ve aktif hedef anlaşılır. Gerçek arama/koşma/aktif-toplam süre kaydedildi; [rota tahmini](FINAL_PACING_BALANCE.md) ölçüm sayılmadı. Değiştirilen Inspector değerlerini varsayılana döndür.
12. [ ] Acil su hattı45sn; menzil içinde E'yi2aktif sn tutarak tamir. Bırakma/uzaklaşma/Esc/AltTab iptal ediyor. Aynı E bakım da yapmıyor. Acil ses eski cue'lar arasında kaybolmuyor; **mute ile** kırmızı işaret/panel/yön/bar/geri sayım yeterli.
13. [ ] Ayrı turda arızayı bilerek kaçır: tek ceza, geçerli devam/final; tekrar tetiklenme/kilit yok. Onarım ve olay penceresi gerçek zamanla karşılaştırıldı.
14. [ ] Pause30sn: oyuncu, toplam süre/baskı/arıza/çağrı saati duruyor, onarım iptal. Resume/delta sıçraması yok. Ayarlar→Esc yalnız pause'a, ikinci Esc devam. AltTab/native suspend sesleri durduruyor; geri dönüşte tek loop başlatma, kullanıcı pause'u korunuyor. Son 60/20 uyarıları metin+`!`+renk, acille çakışmıyor; mute ile de anlaşılır.
15. [ ] Üç genel final ve eksik hazırlıkla timeout denendi. Başlık/kısa neden güncel kaynaklarla uyumlu; rastgele başarı yok. Yeniden Doğuş olumlu cue/atmosfer, diğerleri olumsuz cue; kötü kaynaklı köy görseli korunuyor. Kriz başarısı ile sürdürülebilirlik ayrımı açık. Köyü İzle/ayrıntı-scroll çalışıyor.
16. [ ] Pause→Restart/Ana Menü, Final→Yeniden Oyna ve Final→Ana Menü→Yeni Oyun; en az üç tur. Kaynak/süre/görev/onarım/10 işaret/tesis/kamera/su/bitki/fog/HUD temiz. Eski kısa cue/gerilim kalmıyor; müzik/ortam geçişi tutarlı, source/Listener sayısı sabit. Eski Final Pacing ve RC 5/4 dahil diğer menüler de kendi profillerini koruyor; arka plan müzik eklemiyor.
17. [ ] Master/müzik/efekt/mute restart ve menü dönüşünde korunuyor. Aynı Editor oturumundan çıkıp yeniden açma, ayrıca **aynı EXE'yi kapatıp yeniden açma** ile PlayerPrefs kalıcılığı test edildi; Editor/EXE depolarının aynı olması beklenmedi. Eski master/mute tercihi sıfırlanmadı.
18. [ ] **1280×720 ve1920×1080 /16:9**: Türkçe karakter/font, menü butonları, üç slider, kartlar, uzun hedef/bildirim, son süre uyarısı, acil ve final açıklaması taşmıyor. Kontrast yeterli; kritik bilgi yalnız renk/ses değil. Kaydırma ve mouse hit alanları doğru.
19. [ ] Windows Build Profiles hedefi Windows; **Scene List'te FinalPolish sahnesi etkin ve ilk**. Player Settings ürün adı/pencere başlığı **Son Bir Umut: Kuraklıktan Önce**; uygulama simgesi doğru/başka oyunla karışmıyor. Araç bu global ayarları değiştirmedi; manuel doğruladım.
20. [ ] EXE Unity dışında baştan sona açılış→menü→oyun→pause→final→replay→çıkış. **EXE + aynı isimli _Data + UnityPlayer.dll + Unity'nin ürettiği bütün yan dosya/dizinler** birlikte paketlendi; başka klasöre çıkarılıp çalıştırıldı. Tek EXE gönderilmedi. İlk Player.log/Console hatası ve tekrar adımı kaydedildi.
21. [ ] Son oynanış videosu: başlık/menü müziği, hareket/karar, görevler, acil, pause ve final/yeniden oynama görünür; oyun sesi kayıt seviyesinde temiz. Kişisel masaüstü/bildirimler veya başka oyun varlıkları videoya girmedi. Video bu Mac'te üretilmiş sayılmadı.

| Kabul kaydı | Değer |
| --- | --- |
| Test eden / gerçek tarih / branch / tam commit | |
| Unity patch / Windows / pipeline / input / build backend | |
| Sahne yolu / çözünürlük / FPS | |
| Müzik kalite/seviye/döngü / altı cue / prefs / cihaz | |
| Source/Listener sayıları ilk açılış–üçüncü tur | |
| Görev seti/rota/aktif-toplam süre/arıza/final | |
| İlk Console/Player.log hatası / tekrar adımı | |
| EXE/çıkış / teslim ZIP / video yolu | |

Tam test edilmiş Unity proje yedeği için Editor kapalıyken **Assets/Packages/ProjectSettings ve bütün gerekli .meta** dosyalarını koru. Library/Temp/Logs/bin/obj/cache gerekmez; gerçek Assets/Generated ve Audio dosyaları önbellek değildir. Kopyayı başka klasöre çıkarıp yeniden aç. [Tam paketleme ayrıntısı](RELEASE_CANDIDATE_TEST.md). Bu kaynak deposu gerçek Unity projesi veya Windows build değildir.
