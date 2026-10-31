# Güncel kabul — Final Pacing

Dal **feature/final-pacing**; korunan RC commit'i 5eeb35e. **[Hakan'ın güncel kısa Unity listesi](docs/FINAL_PACING_UNITY_TEST.md)** henüz işaretlenmemiştir.

İlk adım çalışan gerçek Unity projesini yedeklemektir. Scripts/Editor birlikte aktarılır; eski .meta/Scenes/Generated korunur. Menü: **Tools > GameUP > Create Final Pacing Scene**.

10 ayrı konum, seçili 6/en fazla 2 aktif/en az 4; gerçek erişim/koşma/E,45/2 arıza, 240 aktif sn ve son 60/20 sn, başarılı/başarısız final, restart/menu/ayar/ses ve Windows EXE test edilmeli.178 saf C# grup geçti; bu gerçek Unity testi değildir. [Tahmini rota/süre raporu](docs/FINAL_PACING_BALANCE.md) · [Dosyalar ve tasarım](docs/FINAL_PACING.md).

---

Aşağıdaki listeler **korunan RC ve önceki profillerin tarihsel planlarıdır**; yeni on node için yukarıdaki liste kullanılmalıdır.

# Güncel kabul testi — Release Candidate

**Dal: feature/release-candidate. Kaynak referans: 391d253.**

Hakan'ın güncel, henüz işaretlenmemiş test planı: **[docs/RELEASE_CANDIDATE_TEST.md](docs/RELEASE_CANDIDATE_TEST.md)**.

İlk adım çalışan Unity projesini yedeklemektir. Scripts ve Editor birlikte aktarılmalı; eski .meta/Scenes/Generated korunmalı. Yeni sahne: **Tools > GameUP > Create Release Candidate Scene**.157 saf C# test grubu geçti; Unity API derlemesi, gerçek menü tıklaması/ses/prefs/sahne ve Windows EXE bu Mac'te denenmedi.

Öncelik ana menü→Yeni Oyun→oynanış→Esc pause→restart/menu→final/replay, gerçek ses/mute ve üç ardışık temiz oturumdur. Build Profiles Scene List'te RC ilk/etkin olmalı; EXE ve bütün yan dosyalar birlikte teslim edilmeli. Tam Unity proje yedeği Assets/Packages/ProjectSettings ve .meta içermeli. Ayrıntılı adımlar güncel plandadır.

---

Aşağıdaki listeler **tarihsel Aşama 3 ve önceki profil testleridir**. Yeni RC menü/ses akışını kapsamazlar.

# Son Bir Umut — Unity test kontrol listesi

**Aşama3 güncel liste:** [PLAYTEST_UNITY_TEST](docs/PLAYTEST_UNITY_TEST.md). Yeni sürüm Unity'de doğrulanmadı; aşağıdaki Aşama2/1/polish kutuları tarihsel regresyon kaydıdır.

- [ ] Yeni feature/playtest-ux / PlaytestDemo sahnesi,37C# dosyası ve tüm serileştirilmiş referanslar Unity6.3LTS'de doğrulandı.
- [ ] İnce HUD, kısa kartlar, yalnız menzilde E ve tek acil/bildirim, iki çözünürlükte okunuyor.
- [ ] 32×28 harita/üç bölge/yol/kanal/ağaçlar;1.9sprint, kamera ve fiziksel erişim denendi.
- [ ] Beş görev/en fazla iki aktif/en az dört başarı; tekrar ödül yok, eski üç görevli mod korunuyor.
- [ ] Arıza45/2; son saniye vurgusu, damla/tamir/solar seçim kontrolü, pause/focus ve iki replay denendi.
- [ ] Yüksek baskıda uzak pus, yakın karakter/yol/işaret görünürlüğü, kaynakla dünya tepkisi ve fog alternatifi doğrulandı.
- [ ] Gerçek aktif/toplam süre ölçüldü; Windows PlaytestDemo build/EXE denendi, ilk hata kaydedildi.


**Güncel Aşama 2/2:** Hakan önce [FINAL_UNITY_TEST](docs/FINAL_UNITY_TEST.md) planını uygulamalı. Bu yeni sürüm Unity'de henüz test edilmedi; aşağıdaki Aşama 1/polish kutuları da geçmişten aktarılıp otomatik işaretlenmez.

## Aşama 2/2 — yeni kriz demosu (bekliyor)

- [ ] Çalışan proje/.meta yedeklendi, güncel Scripts ve Editor birlikte alındı; Old/Both ve Unity 6.3 LTS derlemesi kontrol edildi.
- [ ] Create Crisis Demo Scene ayrı benzersiz sahne/Generated klasörü üretti; üç kalıcı marker ve referanslar kaydet/yeniden aç sonrasında korunuyor.
- [ ] Üç strateji sonrası üç bakım farklı sırada bir kez uygulanıyor; ikinci E tekrar kaynak/baskı ödülü vermiyor.
- [ ] Pompa seçilmediğinde de su bakım/arıza işareti aktif ve erişilebilir; diğer seçenek görselleri korunuyor.
- [ ] Tek arıza: kırmızı uyarı/işaret; 2 sn E held; bırakma/uzaklaşma/odak kaybı sıfırlar. Onarımda sayaç işler, E aynı karede başka iş yapmaz.
- [ ] Arıza bilerek kaçırıldı: bir kez -6 su/+8 baskı, oyun sürüyor, süre sonunda geçerli final var.
- [ ] Güncel HUD/dünya/final bakım/arıza kaynaklarını kullanıyor; stratejik üç karar geçmişi değişmiyor.
- [ ] En az iki bakım ve arıza değerlendirmesi ile ≤35 baskıda 10 aktif sn erken başarı; tek görev yetmiyor. Kriz başarısı ve uzun vadeli kategori ayrı.
- [ ] İyi/orta/kötü strateji; 0/1/2 karar, eksik bakım, aktif arıza süre sonları; iki replay ve final kilidi denendi.
- [ ] Yönlendirme, bildirim, güncel hedef ve 1280×720/1920×1080 final/scroll okunuyor; atmosfer/Use Basic Fog alternatifi görünür.
- [ ] Eski Demo ve Foundation menüleri kendi eski akışlarını koruyor; sahne/material üzerine yazılmıyor.
- [ ] Build Profiles/Scene List'te yeni CrisisDemo başlangıç; Windows EXE, _Data ve yan dosyalar birlikte; Editor dışında test edildi.
- [ ] Gerçek aktif/duvar saati süresi, commit, patch, pipeline/input, Console/Player.log kaydedildi.

Aşağıdaki liste Aşama 1/polish regresyonudur; güncel kriz demosunda bakım/arıza beklemeyen satırlar yalnız Foundation modu içindir.


**Durum: Kriz Nöbeti Aşama 1/2 ve korunan görsel temel için aşağıdaki Unity kontrolleri henüz yapılmadı.** Mac üzerinde Unity Editor bulunmaz. Kutuları Hakan veya Enes yalnızca testi gerçekten çalıştırınca işaretlemelidir. Saf C# testlerinin geçmesi bu kutuları tamamlamaz.

| Test kaydı | Değer |
| --- | --- |
| Test eden / tarih | |
| Branch / commit | feature/live-village-demo / |
| Unity tam sürümü (6000.3.x) | |
| Windows sürümü / pipeline | |
| Input Handling | Both veya Old / |
| FPS / Game çözünürlüğü | |
| Hız / hızlanma / fren / dönüş | 3.5 / 12 / 16 / 600 başlangıç; test edilen: |
| İlk kez oynanış süresi | 3–5 dk hedef; ölçülen: |
| İlk Console hatası / adım | |

## Kriz Nöbeti Aşama 1/2 — tüm adımlar bekliyor

- [ ] Önce Create Demo Scene: varsayılan üç karar → final, sayaç/baskı yok. Eski tek istasyon da test edildi.
- [ ] Create Crisis Foundation Scene ayrı benzersiz sahne/Generated klasörü üretiyor; varsayılan veya eski sahneyi ezmiyor.
- [ ] Yeni sahne: Intro, 04:00, 60 baskı, 0/3; beklemek hareket/süre/baskıyı ilerletmiyor.
- [ ] Sol Shift hedef hızı 1.6×; bırakınca yumuşak yürüyüş; dört çaprazda ek hız yok.
- [ ] Shift+W tutulurken kart/intro/final yatay hareketi hemen kesiyor; kamera/CharacterController/sınırlar korunuyor.
- [ ] 1280×720 ve 1920×1080: tek odak paneli, kısa olay metni ve üç seçenek kırpılmıyor; dünya kısmen görünüyor.
- [ ] Kazanç, bedel, maliyet ve net +/- çipleri okunuyor; emisyon artışı kırmızı, azalışı yeşil, sıfır nötr.
- [ ] Engelli seçeneğin gerçek gerekçesi okunuyor; tıklama durum/geçmiş/sayaç değiştirmiyor.
- [ ] Etki ayrıntısı seçeneği teknik bileşenleri açıyor; varsayılan görünüm uzun döküm içermiyor. Yeni kart ve replay ayrıntıyı kapatıyor.
- [ ] Kart ve ayrıntı açıkken süre/baskı duruyor; Esc/Vazgeç sonrası devam ediyor.
- [ ] Alt+Tab, arka plan Update açık/kapalı ve kart açık/kapalı: süre dönüşte sıçramıyor; Windows EXE'de de denendi.
- [ ] Üç karar: CrisisManagement, 3/3; final açılmıyor, kamera uzaklaşmıyor; serbest dolaşım/sayaç sürüyor.
- [ ] 5 saniyelik test ayarıyla 0/1/2 kararda son: sonuç/eksik hazırlık, iki final sekmesi/scroll/boş geçmiş null hatasız.
- [ ] Üç karar sonrası süre sonu: geçerli mevcut stratejik kategori ve final görünümü; dördüncü karar yok.
- [ ] Yeniden Oyna: Intro/tam süre/60 baskı/kaynaklar/geçmiş/UI/kamera/modeller ve hareket hızı sıfırlandı.
- [ ] Built-in ve URP: hafif sis geçişi, karakter/yol/işaretler okunuyor. Use Basic Fog kapalıyken ışık/ambient alternatifi çalışıyor.
- [ ] Ayrı başlangıç baskısı 20/80 denemeleri açık/sıcak ve kasvetli yönü gösteriyor; varsayılan ayarlara geri dönüldü.
- [ ] Atmosfer bileşeni kapatılıp açılınca render ayarları geri yükleniyor; eski sahnede yeni sis bileşeni yok.
- [ ] Duration Seconds tekrar 240. Bakım/acil arıza/yeni kazanma sistemi olmadığı test notuna yazıldı.
- [ ] Console ilk hatası, Unity patch/pipeline/input/commit ve gerçek deneyim süresi kaydedildi; Windows test build'i denendi.

[Windows test adımları](docs/INTEGRATION.md) · [Modlar ve ayarlar](docs/LIVE_VILLAGE_STAGE1.md)

## Import ve sahne güvenliği

- [ ] Mevcut Assets/SonBirUmutDemo silinmeden kaynaklar güncellendi; eski .meta, Scenes ve Generated korundu.
- [ ] Unity 6.3 LTS script derlemesi tamamlandı; Console'da hata / Missing Script yok.
- [ ] UnityEditor kodları build'e sızmıyor; Editor klasöründe.
- [ ] Tools > GameUP > Create Demo Scene üç noktayı ve bağlı referansları kuruyor.
- [ ] SonBirUmutVisualDemo.unity yeni dosya; eski enerji sahnesi değiştirilmedi.
- [ ] Aracı ikinci kez çalıştırınca ayrı numaralı sahne ve materyal klasörü oluşuyor.
- [ ] Kirli açık sahnede araç uyarıp duruyor; kaydetme, silme veya değişiklik atma yok.
- [ ] Prefab düzenleme / Play modunda araç mevcut içeriğe müdahale etmiyor.
- [ ] Sahneyi kapatıp tekrar açınca materyaller ve referanslar korunuyor; pembe model yok.
- [ ] Eski tek istasyonlu sahnenin referansları ve tek noktalı akışı yeni kaynaklarla ayrıca denendi.

## Aşama 2: gerçekten yeni görsel sahne

- [ ] Güncel Tools menüsü yeni SonBirUmutVisualDemo sahnesini ve Generated/VisualDemo klasörünü oluşturdu; eski CompleteDemo ezilmedi.
- [ ] Üç gerçek mesh varlığı (Canopy, GableRoof, Icosahedron) ve materyaller kaydedildi; sahne tekrar açılınca eksik/pembe model yok.
- [ ] Eğimli çatılar, pencereler/kapılar, taş temeller, patika, çit, kuyu, bank, kaya ve ot ayrıntıları görülüyor.
- [ ] Kapsül collider üzerinde köylü baş/yüz/şapka/ceket/çanta/kol/bacak modeli var; görsel kapsül kaldırıldı.
- [ ] Yüz ileri yöne bakıyor; yürüyüşte görsel eklemler hareket ediyor, root hareketi değişmiyor.
- [ ] Ayaklar zemine oturuyor; duvar, kart, giriş ve finalde yerinde yürüyüş sürmüyor.
- [ ] Yeni ev temel/kuyu/bank/çit çarpışmaları erişim yollarını kapatmıyor; üç noktaya ulaşılabiliyor.
- [ ] Yeşil/sarı taç ve kuru dal görünümü sağlığa uyuyor; taç/dal aynı anda yanlış açık kalmıyor.
- [ ] Ürün boyu ve toprak rengi değişiyor; su havuzu çekiliyor, <20 suda çatlaklar beliriyor; 0 suda yüzey kapalı.
- [ ] Görsel geçiş sırasında kaynaklar fazladan değişmiyor; dünya kategoriden değil gerçek durumdan türetiliyor.
- [ ] Opak tema, kaynak çubukları ve sıradaki nokta etiketi 1280×720 ve 1920×1080'de okunuyor.
- [ ] Kartlarda kazanç/bedel/maliyet/net etkiler; isteğe bağlı ayrıntıda seçim/tur/kuraklık değerleri kırpılmıyor; engelli düğme seçilemiyor.
- [ ] Finalin Köyün durumu / Kararlarının etkisi sekmeleri ve fareyle kaydırma çalışıyor; tüm metinlere erişiliyor.
- [ ] Köyü izle paneli gizliyor, kamera köyü ortalıyor; Sonuç raporuna dön sağ panele yer açıyor.
- [ ] Köyü izleme sırasında hareket/karar kilidi korunuyor; rapora dönüş kaynak veya seçim değiştirmiyor.
- [ ] Üç kez yeniden oynamada görsel poz, taç, ürün, havuz, tema dokuları, sekme/kaydırma ve kadraj sızıntısı yok.
- [ ] Windows EXE'de yeni sahne/mesh/materyaller ve finali izleme çalışıyor; FPS / ilk hata kaydedildi.

## Öncelik: akıcı hareket

- [ ] CharacterController korunuyor; Rigidbody yok. Kapsül zeminden düşmüyor.
- [ ] W, A, S, D tek başına kamera düzlemine göre doğru yönlere götürüyor.
- [ ] W+A, W+D, S+A, S+D çaprazlarının hepsi çalışıyor.
- [ ] Çapraz hız düz hareketten yüksek değil.
- [ ] W+A basılıyken A bırakılması düzgün yön değiştiriyor.
- [ ] W'den S'ye doğrudan geçişte anlık 180° sıçrama yok.
- [ ] Hızlanma kısa ve doğal; tüm tuşlar bırakılınca kontrollü fren var.
- [ ] Karakter anlamlı hız yönüne Y ekseninde yumuşak dönüyor; yön işareti anlaşılır.
- [ ] 30/60/120 FPS karşılaştırmasında gereksiz kayma, titreşim veya yön tersliği yok.
- [ ] Evler, ağaçlar, depo, kaide ve harita sınırlarının içinden geçilemiyor.
- [ ] Dekoratif karar modellerinin çarpışmasız olması oyuncuyu hapsetmiyor.
- [ ] Alt-tab sonrası geri dönünce kontrol düzgün; eski hız birikmiyor.

## Giriş, kamera ve kartlar

- [ ] Kısa Türkçe giriş görünüyor; Enter veya Başla tek adımda oyunu başlatıyor.
- [ ] Giriş açıkken karakter yürümüyor.
- [ ] Kamera yumuşak takip ediyor; karakter dönünce kamera kendi açısını döndürmüyor.
- [ ] Sıradaki noktaya yaklaşınca E ipucu, uzaklaşınca normal keşif bilgisi görünüyor.
- [ ] E üç kartı açıyor; yatay hareket aynı karede duruyor.
- [ ] Esc/Vazgeç hiçbir kaynağı değiştirmeden kapatıyor; hareket yeniden hızlanıyor.
- [ ] Both modunda klavye ve fare kartları çalışıyor (paket varsa).
- [ ] Old modunda yeni Input System paketi olmadan klavye ve kartlar çalışıyor.
- [ ] New-only modunda araç açıklama gösterip duruyor; eski sahnede açık Console uyarısı var.
- [ ] Kart metinleri 1280×720 ve 1920×1080'de okunaklı; kırpılma yok.

## Polish: hikâye ve kararların anlaşılabilirliği

- [ ] Varsayılan sahnede başlık Son Bir Umut: Kuraklıktan Önce; ana hedef tüm ekranlarda açık.
- [ ] İlerleme, sıradaki nokta ve kritik kaynak durumu sürekli görülüyor.
- [ ] Kısa olay/problem ve Kazanç / Bedeli / Maliyet alanları okunuyor; ayrıntı isteğe bağlı.
- [ ] Güneş sonrası 40 / 55 / 32 / 41 / 0; doğrudan bitki ve emisyon etkisi sıfır.
- [ ] Tur su -8, su koşulu bitki -8, kuraklık bitki -6 ayrı; net bitki -14 açık.
- [ ] 0/100 sınırındaki net değişim bileşenlerle karıştırılmıyor.
- [ ] Seçim bildirimi değişen nesneyi kapatmıyor; ×/Esc ile hemen, 10 saniyede otomatik kapanıyor.
- [ ] Bildirim açıkken hareket ve sonraki E etkileşimi çalışıyor; final düğmesini örtmüyor.
- [ ] Güneş → pompa → sulama, 10 / 55 / 46 / 73 / 0 ile Yeniden Doğuş; koruma yolu farklı kaynak dağılımıyla da başarılı.
- [ ] Final stok, tarım, doğal yaşam, iki karar ve süren risk bakımından anlaşılır.
- [ ] Emisyon/enerji riski olan final enerji + su kararını; diğer finaller su + tarım kararını öne çıkarıyor.
- [ ] Final açıklamaları su kalitesi veya bağımsız ürün verimi ölçümü iddiası taşımıyor.
- [ ] Yeni arayüz 1280×720 ve 1920×1080'de kırpılmıyor; yeniden oynama bildirim ve özeti de sıfırlıyor.

## Üç karar ve bağımlılıklar

- [ ] Enerji → su → tarım sırası korunuyor; erken su/tarım kartı açılamıyor.
- [ ] Her noktada üç seçenek ve kısa açıklama var.
- [ ] Maliyetler ve net enerji/su/bitki/emisyon etkileri seçimle birebir eşleşiyor.
- [ ] Yetersiz bütçe, enerji veya su nedeni görünüyor; seçenek uygulanamıyor.
- [ ] Her aşamada en az bir seçenek seçilebilir kalıyor.
- [ ] Güneş sonrası pompa maliyeti 25; tasarruf sonrası 30 ve su üretimi daha düşük.
- [ ] Verimlilik tarım turunun su kullanımını da azaltıyor.
- [ ] Güneş/dizel → mevcut su düzeninde sulama engelli; depolamayla tamamlanıyor.
- [ ] Aynı noktada ikinci seçim ve ikinci maliyet uygulanamıyor.
- [ ] Beklerken kaynaklar değişmiyor; her kabul edilen karar yalnız bir tur ilerletiyor.

## Dünya görünümü

- [ ] Panel / jeneratör / tasarruf işareti yalnız seçilen enerji modelini gösteriyor.
- [ ] Su seviyesi, pompa / vana / mevcut düzen uyarısı seçime ve duruma uyuyor.
- [ ] Doğal alan koruması / sulama kanalları / depolama varilleri doğru açılıyor.
- [ ] Bitki ≥55 yeşil, 30–54 sarı, <30 kuru; tarla rengi de değişiyor.
- [ ] Görseller tek haritada değişiyor; üç ayrı harita veya sıcaklık simülasyonu yok.

## Final ve yeniden oynama

- [ ] Güneş → verimlilik → koruma: 20 / 55 / 42 / 59 / 0, Yeniden Doğuş.
- [ ] Dizel → pompa → sulama: 45 / 70 / 43 / 73 / 18, Eksik Dönüşüm.
- [ ] Güneş → mevcut düzen → depola: 40 / 55 / 17 / 8 / 0, Kaybedilen Gelecek.
- [ ] Varsayılan eski modda final yalnız üçüncü karardan sonra açılıyor; su/tarım/doğa, öne çıkan iki karar ve süren risk okunuyor.
- [ ] Kamera uzaklaşıp yükseliyor; köy sonuç panelinin solunda izlenebiliyor, açı dönmüyor.
- [ ] Final açıkken karakter kontrolü kapalı; tekrar karar uygulanamıyor.
- [ ] Yeniden Oyna; kaynakları 100 / 20 / 40 / 55 / 0, ilerlemeyi 0/3 ve karakteri başlangıca döndürüyor.
- [ ] Panel/jeneratör ve diğer seçim modelleri kapanıyor; su, tarla ve ağaçlar ilk duruma dönüyor.
- [ ] Kamera normal takip ve zoom değerlerine, hız sıfıra dönüyor; giriş yeniden başlatılabiliyor.
- [ ] En az üç ardışık farklı oynanışta önceki seçim veya görsel sızıntısı yok.
- [ ] Giriş kısa köye dönüş ve yaklaşan kuraklık anlatısı; hastalık veya NPC görevi yok.

## Windows teslimi

- [ ] Build Profiles sahne listesinde yeni VisualDemo başlangıç sahnesi.
- [ ] Windows build hatasız oluşuyor ve EXE açılıyor.
- [ ] EXE içinde WASD/çapraz, E, kart tıklama, üç karar, final ve yeniden oynama tekrar denendi.
- [ ] Oynanış süresi ölçüldü; 3–5 dakika hedefine ilişkin geri bildirim yazıldı.
- [ ] Son Console/oyuncu log kontrolü temiz; başarısız adımlar kayda işlendi.

Başarısız adımda commit, adım, beklenen/görülen davranış ve ilk hata metnini yazın. Bu dosyaya kişisel ekran görüntülerini eklemek yerine gerekirse ekip içinde paylaşın.
