# Hakan — Final Pacing / Unity 6.3 LTS

**Bütün kutular bekliyor.** Mac'te Unity Editor, gerçek fizik, GUI, ses veya Windows build testi yapılmadı. Dal `feature/final-pacing`; korunan RC kaynak commit'i `5eeb35e7dad7343e2591f1d3b03870ad47d84305`.

- [ ] **Önce çalışan Unity projesini yedekle.** Kaynak depoda yerel değişiklikleri koru, `git fetch origin`, `git switch feature/final-pacing`, `git pull --ff-only origin feature/final-pacing`. Tam commit'i kaydet.
- [ ] UnityAssets/SonBirUmutDemo içeriğinin tamamını gerçek projenin Assets/SonBirUmutDemo klasörüne aktar. Yeni FinalPacingCatalog, MaintenanceTaskNode ve DemoPacingBuilder dahil **Scripts ve Editor birlikte**; eski .meta/Scenes/Generated silinmez.
- [ ] Unity 6.3 LTS, Built-in/URP, Active Input Handling **Old veya Both**. Console derlemesi bitti, ilk kırmızı hata/Missing Script yok.
- [ ] Play kapalı, Prefab dışında ve açık sahneler temizken **Tools > GameUP > Create Final Pacing Scene**. Yeni FinalPacing sahne/Generated yolu benzersiz; eski RC/Playtest ve diğer menüler korunuyor.
- [ ] Sahneyi kaydet/kapat/yeniden aç: controller Final Pacing bayrağı, on node kimlik/konum referansı ve mevcut ses/kamera/dünya/arıza bağlantıları korunuyor. Node elle taşınmadı; katalog uyumsuzluğu Console'da açık hata vermeli.
- [ ] On **farklı fiziksel nokta** var: üç enerji, dört su, üç tarım/doğa. Konumları [dağılım tablosuyla](FINAL_PACING.md) karşılaştır. Aynı üç istasyon üst üste on görev gibi sunulmuyor.
- [ ] Her noktaya WASD/1.9×Shift ile gerçekten ulaştım; ev, kuyu, tarla direği, depo ve gövdeler engellemiyor. Kanal/havuz üzerinden geçmek gerekmiyor; kamera/işaretler okunuyor. Sorunlu koordinatı kaydettim.
- [ ] Ana Menü→Yeni Oyun→Intro→Başla: 240 sn, başlangıç60 baskı, üç karar aynı. Menü/Intro/kart/pause sırasında zaman duruyor. Ayarlar, mute, ses düzeyi ve kalıcılık çalışıyor.
- [ ] Krizde **altı seçili görev**, her üç bölgeden görev ve en fazla iki sarı aktif işaret var. Pasif dört nokta normal objeler; on simge ekranı kaplamıyor.
- [ ] Çağrı saatleri kriz başından 0/0/25/55/85/115 sn; boş aktif yer gerektiriyor. İki açık işten herhangi biri tamamlanabilir. Yeni çağrı bildirimi ve bekleme hedefi anlaşılır; bakımın ayrı sert son tarihi yok.
- [ ] Her noktada yakın E doğru TaskId/LocationId'yi tamamlıyor; uzaktan veya komşu istasyondan tamamlanmıyor. Tekrar E ikinci ödül vermiyor; yeşil onay ve mevcut bakım sesi geliyor. Tesis seçimini değiştirmenin/yeniden oynamanın node köklerini kapatmadığını kontrol ettim.
- [ ] Görevden göreve koşmak gerekiyor; iki aktif hedef arasında sürekli anlamsız uzak uç zorlaması yok. En yakın hedef etiketi/kenar yönlendirmesi ve açık kalma süresi anlaşılır; on görevlik liste yok.
- [ ] Enerji→su→tarım kararları değişmedi. Solar/dizel/tasarruf için ayrı tur; dizel emisyonu bakım ile silinmiyor. En az dört bakım tek başına yetmiyor: arıza sonucu, düşük baskı ve 10 aktif sn istikrar da gerekiyor.
- [ ] Acil çağrı 12 aktif kriz sn sonra: sabit su istasyonu erişilebilir, **45 sn** belirgin. E'yi **2 aktif sn** tutarak onardım. Ayrı turda kaçırdım; tek ceza ve geçerli devam/final. Acilde bakım E ile yapılamıyor; aynı E iki ödül üretmiyor.
- [ ] Onarımın ortasında E bırakma/uzaklaşma/Esc/AltTab: ilerleme iptal. Pause'da ana sayaç/baskı/arıza/çağrı saatleri 30 sn değişmiyor. Ayarlar→Esc yalnız pause'a; ikinci Esc devam. Odak dönüşünde süre sıçramıyor.
- [ ] Son60 sn sayaç rengi; son20 sn kırmızı kısa uyarı okunuyor, acil paneliyle çakışmıyor. Gerekirse ayrı turda bekleyerek dene; otomatik görev tamamlama kullanma.
- [ ] Üç genel final (Yeniden Doğuş/Eksik Dönüşüm/Kaybedilen Gelecek), geç hazırlık/eksik bakım/aktif arızayla süre sonu geçerli. Kriz başarısı kötü kaynak stratejisini otomatik iyi finale çevirmiyor.
- [ ] Pause→Restart, Pause→Ana Menü, Final→Yeniden Oyna ve Final→Ana Menü→Yeni Oyun; en az üç tur. Görev seti değişebiliyor; bütün on işaret, kaynak/süre/arıza/baskı/karakter/kamera/atmosfer/HUD temizleniyor. Ses tercihleri korunuyor.
- [ ] Eski **Create Release Candidate Scene** hâlâ beş görev/en az dört ve yeni node olmadan çalışıyor. Eski Playtest ve diğer üç sahne menüsünün akışı korundu.
- [ ] 1280×720 ve1920×1080: Türkçe metinler, uzun hedef/başarı bildirimi, yeni çağrı/açık süre, son20 sn ve final/scroll taşmıyor. Fare/Esc/Enter odak davranışı doğru.
- [ ] En az bir yeni oyuncuyla gerçek dolaşma/arama/etkileşim/aktif ve toplam süre ölçüldü; [model tahmini](FINAL_PACING_BALANCE.md) fizik doğrulaması sayılmadı. Kolay/normal/geciken rota ve başarısızlık kaydı alındı.
- [ ] Windows Build Profiles Scene List'te **Final Pacing sahnesi etkin ve ilk**. EXE Editor dışında ana menü→oyun→pause→final→replay→çıkış akışında test edildi. EXE, _Data, UnityPlayer.dll ve bütün gerekli yan dosyalar birlikte korundu.

| Kayıt | Değer |
| --- | --- |
| Test eden/tarih/tam commit/Unity patch | |
| Pipeline/input/çözünürlük/FPS | |
| Sorunlu node/koordinat, Console/Player.log ilk hatası | |
| Seçili görevler/rota/enerji/arıza sonucu/final | |
| Aktif süre/toplam süre/oyuncu deneyimi | |
| Restart/ses/EXE sonucu | |

Tam proje ve Windows teslim paketleme kuralları: [RC teslim rehberi](RELEASE_CANDIDATE_TEST.md). Assets/Packages/ProjectSettings ve gerekli bütün .meta dosyalarını koru; Library/Temp/Logs/build önbelleği gerekmez. Bu kaynak deposu gerçek Unity proje yedeği değildir.
