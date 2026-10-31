# Hakan — Kriz Nöbeti Aşama 2/2 son Unity testi

Hedef: Windows, Unity **6.3 LTS / 6000.3.x**. Dal: **feature/live-village-demo**. Bu belge bir test planıdır; bu Mac'te Unity Editor veya Windows EXE çalıştırılmadı. Kutular yalnız gerçekten denendikten sonra işaretlenmeli. Saf C# sonucu ve [denge simülasyonu](CRISIS_BALANCE.md) fizik/görsel doğrulaması değildir.

## 1. Önce çalışan projeyi yedekle

- [ ] Çalışan Unity projesini kapatıp ayrı konuma yedekledim. Assets, ProjectSettings, Packages ve mevcut .meta dosyalarını korudum; yerel düzenlemelerimi kaybettirmedim.
- [ ] Kaynak depom temizken `git fetch origin`, `git switch feature/live-village-demo`, `git pull --ff-only origin feature/live-village-demo` yaptım. Dal/commit'i aşağıya yazdım. Yerel değişiklik varsa önce korudum; force/reset/merge yapmadım.
- [ ] `UnityAssets/SonBirUmutDemo` içeriğinin tamamını dış Unity projemin `Assets/SonBirUmutDemo` klasörüne, klasörü silmeden aktardım. **Scripts ve Editor birlikte**, yeni CrisisStationMarker ve Simulation dosyaları dahil. Mevcut .meta GUID'lerini korudum; yeni scriptlerin .meta dosyalarını Editor üretti.
- [ ] Tests/docs/kişisel görselleri Assets içine kopyalamadım. Kaynak deposu gerçek Unity projesi değildir.
- [ ] Unity 6.3 LTS Built-in 3D veya URP projesinde import/derleme bitene kadar bekledim. HDRP desteklenmez; yeni paket/asset gerekmez.
- [ ] Active Input Handling = **Input Manager (Old)** veya **Both**. Both yalnız zaten Input System kullanan projede; New-only IMGUI kart tıklamalarını desteklemez. İstenirse Editor'ı yeniden başlattım.
- [ ] Window > General > Console: ilk kırmızı hatayı kontrol ettim, Missing Script veya paket/giriş sembolü hatası yok. Hata varsa devam etmeden tam metin ve adımı kaydettim.

## 2. Yeni tamamlanmış kriz sahnesini üret

- [ ] Play kapalı, Prefab düzenleme dışında, açık sahneler temizken **Tools > GameUP > Create Crisis Demo Scene** seçtim.
- [ ] `Assets/SonBirUmutDemo/Scenes/SonBirUmutCrisisDemo.unity` (doluysa numaralı yol) açıldı. Materyal/mesh'ler `Generated/CrisisDemo...` altında; önceki sahne ve materyaller ezilmedi.
- [ ] GameController, karakter, kamera, üç istasyon, HUD, dünya ve atmosfer referansları dolu. Her ana istasyon kökünde CrisisStationMarker var. Su arıza işareti pompa/vananın altında değil, kalıcı su istasyonunun altında.
- [ ] Sahneyi kapatıp tekrar açtım: referanslar, mesh ve materyaller korunuyor. Pembe nesne yok.
- [ ] Aracı ikinci kez çalıştırınca ayrı numaralı sahne/Generated klasörü oluştu. Kirli sahnede uyarıp duruyor; hiçbir çalışmayı otomatik kaydetmiyor veya atmıyor.

## 3. Giriş ve ilk 20 saniye

- [ ] 1280×720 Game görünümünde Play. Başlık **Son Bir Umut: Kriz Nöbeti**; 04:00, 60 baskı, 0/3 hazırlık görünüyor. Girişte süre/karakter duruyor.
- [ ] Hikâye iklim değişikliği/kuraklık bağlantısını ve “Üç karar ver, köyün altyapısını onar ve kriz baskısını düşürerek kuraklığa hazırlan” hedefini açıklıyor. Deneysel altyapı/sonraki aşamada gelecek metinleri yok.
- [ ] Enter/Başla sonrası ilk hedef enerji istasyonu. WASD, Sol Shift koşma, E karar/bakım ve arızada E basılı tutma anlaşılıyor.
- [ ] Yürüme ve 1.6× Shift koşma yumuşak; dört çapraz ek hız vermiyor. Kamera sabit izometrik, CharacterController korunuyor; çarpışmalar ve sınırlar çalışıyor.
- [ ] İşaret kamera dışındayken ekran kenarındaki Yön etiketiyle hedefi bulabiliyorum. Hedef yolu ev/çit/kuyu kapatmıyor.

## 4. Üç stratejik karar

- [ ] Enerji → su → tarım sırası korunuyor; erken su/tarım E'si kart açmıyor.
- [ ] E üç seçenekli kısa odak panelini açıyor; hareket/sayaç/baskı duruyor. Maliyet/net etkiler ve engelli seçenek gerekçesi okunuyor; Etki ayrıntısı isteğe bağlı.
- [ ] Giriş, kart ve finalde Shift hareket kilidini aşmıyor; Esc/Vazgeç sonrası kontrol geliyor.
- [ ] Üçüncü karardan sonra final değil **KrizManagement / bakım** aşaması; üç altın bakım işareti var. İlk öneri yakındaki tamamlanmamış görev.
- [ ] Güneş, dizel, tasarruf; su pompası, verimlilik, mevcut düzen seçeneklerini farklı turlarda denedim. Her durumda bakım ve su arızası kalıcı istasyonda görünür/erişilebilir.

## 5. Bakımlar ve tek acil olay

- [ ] Yakında yeni E basışıyla enerji bağlantısını denetle (-12 baskı), su kaybını gider (-14, korunan su en çok +4), sulama vanasını ayarla (-14, bitki desteği en çok +4).
- [ ] Üç görev farklı sırada yapılabiliyor. Tamamlanan işaret kayboluyor, kısa bildirim ve sayaç güncelleniyor. Aynı görev tekrar E ile ikinci ödül vermiyor.
- [ ] Kriz başladıktan 12 aktif saniye sonra bir kez su hattı arızası çıkıyor. Çok geç hazırlıkta gecikme kalan sürenin dörtte birine kısalabilir. Tek öncelikli uyarı, kırmızı su işareti ve müdahale süresi görünüyor.
- [ ] Aktif arızada E başka bakım/karar çalıştırmıyor. Depoya Shift ile ulaştım, menzilde **E'yi 2 saniye basılı tutarak** onardım; bar dolmadan başarı olmadı. Onarımda ana sayaç ilerliyor.
- [ ] E'yi bırakınca, menzilden çıkınca, odağı kaybedince onarım barı sıfırlanıyor; döndüğümde önceki ilerlemeyle anında tamamlanmıyor.
- [ ] Onarım bir kez -18 baskı verdi; işaret kapandı. E basılı kalması su bakımını aynı karede ödüllendirmedi; bırakıp yeniden bastım.
- [ ] Ayrı oynayışta arızayı bilerek kaçırdım: 45 aktif saniyede yalnız bir kez su -6 (en az 0), baskı +8 (en çok 100). Bakımlara dönebildim; oyun kilitlenmedi.
- [ ] Su pompasını **seçmeden** aynı iki arıza testini yaptım: işaret/menzil çalışıyor, kapalı pompa görseline bağımlılık yok.

## 6. Sayaç, güncel kaynaklar ve final

- [ ] HUD ve su/bitki görselleri bakım/arıza sonrası güncel kaynakları gösteriyor. Stratejik üç karar geçmişi değişmiyor; bütçe/enerji/emisyon bakımda bedelsiz çoğalmıyor.
- [ ] Baskı düşerken atmosfer yumuşakça açılıyor/ısınıyor; artarken hafif kasvetli. Karakter/yollar/işaretler daima okunuyor. Use Basic Fog kapalıyken ışık/ambient alternatifi de kullanılabilir.
- [ ] Üç karar + en az iki bakım + arızanın onarılması veya zaman aşımıyla değerlendirilmesi + baskı ≤35 koşulları **10 aktif saniye** korununca süre dolmadan final açılıyor. Tek görevle kazanılmıyor; üçüncü bakım zorunlu olmayan ek destektir ve erken finalden önce tamamlanabilir.
- [ ] Güneş → verimlilik → koruma, bakımlar ve onarım: kriz başarısı / Yeniden Doğuş mümkün. Dizel → pompa → sulama + aynı başarılı müdahaleler: kriz başarısı mümkün, yüksek emisyon nedeniyle genel final Eksik Dönüşüm.
- [ ] Kötü dizel → mevcut su düzeni → depola, yalnız bir bakım ve kaçırılan arıza: süre sonunda Kaybedilen Gelecek mümkün. [Sayısal örnekleri](CRISIS_BALANCE.md) gerçek kaynaklarla karşılaştırdım.
- [ ] Ayrı testte kararlar tamam ama bakım eksik: süre sonunda geçerli kısmi/başarısız sonuç. Sağlıklı stratejik kaynaklar tek başına kriz başarısı mesajı vermiyor.
- [ ] Play dışında Duration Seconds=5 ile 0, 1 ve 2 kararda süre sonunu test ettim. Eksik hazırlık, geçerli özet, boş geçmiş sekmesi; NullReferenceException/boş final yok.
- [ ] Play dışında Duration Seconds=15 ile üç kararı ve bakımları hemen tamamlayıp arızayı çözmeden bekledim: güvenli süre sonu, “çözülmedi” sonucu; sonsuz bekleme yok.
- [ ] Finalde **kriz nöbeti başarısı ile genel sürdürülebilirlik kategorisi ayrı**; bakım/arıza sonrası güncel su ve bitki özeti kullanılıyor. Stratejik kart geçmişi eski tur etkileri olarak açıklanıyor.
- [ ] İki final sekmesi, kaydırma, Köyü izle / rapora dön ve tüm uzun metinler okunuyor. Finalde hareket ve E ödülleri kilitli. İklim değişikliğinin bittiği iddiası yok.
- [ ] İki ardışık Yeniden Oyna: Intro, 240 süre, 60 baskı, 0 karar/0 bakım, tek yeni arıza planı, onarım/istikrar sıfır, ilk kaynaklar, modeller/marker/kamera/oyuncu/UI sıfırlandı.
- [ ] Alt+Tab: giriş/kart/serbest/onarım sırasında, arka plan Update açık ve kapalı; döndüğümde sayaç/arıza sıçramıyor. Kart ve odak dışı süre ilerlemiyor. 1280×720 ve 1920×1080'de tüm arayüzü denedim.
- [ ] Test ayarlarını **240 / 60 / 35 / 10 / 12 / 45 / 2** (süre/baskı/eşik/istikrar/gecikme/arıza süresi/onarım) ve varsayılan artış katsayılarına döndürdüm.

## 7. Eski mod regresyonu

- [ ] **Create Demo Scene** ayrı eski üç karar → final sahnesini üretiyor; görev/arıza ve yeni sayaç yok. Önceki sayısal üç final ve eski tek enerji sahnesi de doğrulandı.
- [ ] **Create Crisis Foundation Scene** hâlâ ayrı temel sahne; süre/baskı var, görev/arıza/erken kriz başarısı yok. Üç kararla hemen bitmiyor, süre sonu geçerli.
- [ ] İki eski menü ve yeni menü aynı mevcut sahneyi/metaryali ezmiyor. Her sahneyi kendi .meta ve Generated klasörüyle korudum.

## 8. Windows build ve teslim

- [ ] Editor kontrolleri sonrası **File > Build Profiles** Windows profili. Scene List'te **yeni CrisisDemo sahnesi** etkin başlangıç sahnesi; VisualDemo veya Foundation yanlışlıkla ilk değil.
- [ ] Mevcut Windows build modülleriyle build aldım. Çıktıyı kaynak deposuna eklemedim.
- [ ] EXE'yi Editor dışında çalıştırdım: yürüyüş/Shift, üç karar, bakımlar, basılı tamir, süre sonu/erken final, Alt+Tab ve replay çalışıyor. Console/Player.log hatasını kaydettim.
- [ ] EXE ile aynı isimli **_Data** klasörünü, UnityPlayer.dll ve Unity'nin ürettiği diğer gerekli yan dosyaları birlikte korudum/ilettim; yalnız EXE göndermedim.

## Test kaydı

| Alan | Değer |
| --- | --- |
| Test eden / tarih | |
| Dal / tam commit | feature/live-village-demo / |
| Unity patch / Windows | |
| Pipeline / input | |
| Oluşturulan sahne yolu | |
| FPS / çözünürlük | |
| Strateji / görev sırası / arıza sonucu | |
| Aktif oyun süresi / kartlar dahil toplam süre | |
| Son baskı / bakım sayısı / kriz başarısı / genel final | |
| İlk hata, adım ve Console/Player.log metni | |
| Windows build / EXE sonucu | |

3–4 dakika hedefi henüz gerçek oyuncuyla ölçülmedi; hızlı rota daha erken bitebilir. Simülasyonun 178 aktif saniyesi gerçek hareket/okuma süresinin ölçümü değildir. Kaynak ve strateji eşiği testleri Unity API derlemesi, fizik, görünürlük veya referans kalıcılığının yerine geçmez.
