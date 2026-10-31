# Hakan / Enes — Aşama 2 görsel sürüm, Unity 6.3 LTS

## Durum ve dal

Bu sürüm **feature/polish-backup-demo** dalındadır. `main` ve kaynak `feature/complete-demo` değiştirilmez; otomatik merge yapılmaz. Mac üzerinde Unity yoktur. Kullanıcının aktarımına göre Hakan kaynak complete-demo dalını üç kararla Eksik Dönüşüm finaline kadar oynadı. Aşama 1 ve yeni Aşama 2 görsel sürümü bu Mac'te Unity'de doğrulanmadı; saf C# regresyon testleri yeniden **54/54** geçti.

## 1. Kaynakları güvenle al

Çalışma ağacındaki kendi değişikliklerini önce koru; yerel değişikliklerin üzerine yazma. Depo temizken:

```sh
git fetch origin
git switch feature/polish-backup-demo
git pull --ff-only origin feature/polish-backup-demo
```

İlk klonlamada `git clone --branch feature/polish-backup-demo git@github.com:omrumcerenguler/gameup-son-bir-umut-demo.git` kullanılabilir. Bu Windows'taki ekip kurulumu içindir; mevcut Mac çalışma klasöründe yeni klon oluşturulmaz.

1. Unity Hub'da **Unity 6.3 LTS (6000.3.x)** kullan; tam patch sürümünü test kaydına yaz.
2. Mevcut **Universal 3D (URP)** veya **3D Built-in** test projesini kullan. Yeni proje gerekiyorsa bu iki şablondan biriyle depodan ayrı konumda oluştur. HDRP desteklenmez. UnityAssets klasörü doğrudan proje değildir.
3. Projedeki kendi düzenlemelerini koru. `UnityAssets/SonBirUmutDemo` içeriğinin tamamını **`Assets/SonBirUmutDemo` üzerine, klasörü silmeden** kopyala. `Editor` dahil olmalı; yeni scriptleri de al.
4. Mevcut `.meta`, `Scenes` ve `Generated` dosyalarını silme veya yeniden oluşturma. Kaynakta `.meta` bulunmadığı için eski scriptlerin GUID'leri korunabilir; yeni scriptlerin `.meta` dosyalarını Editor üretsin.
5. Depodaki `Tests`, `docs` veya kişisel görselleri Assets içine kopyalama. Dış test projesindeki kod düzeltmelerini daha sonra depodaki UnityAssets kaynaklarına geri taşı.

## 2. Input ve import

**Edit > Project Settings > Player > Other Settings > Configuration > Active Input Handling**:

- **Both:** Resmi Input System paketi zaten kullanılıyorsa önerilir. Klavye yeni backend'den, IMGUI fare olayları eski backend'den gelir. Input Actions varlığı / PlayerInput bileşeni gerekmez.
- **Input Manager (Old):** Yeni paket gerekmez; WASD, E, Esc ve Enter doğrudan eski API ile okunur. Özel eksen ayarı yoktur.
- **New-only:** Runtime IMGUI tıklamalarını desteklemediğinden bu demo için uygun değildir. Araç uyarıp durur. Both/Old yapıp Editor isterse yeniden başlat.

Input sembollerini elle tanımlama. Yeni backend'de güncelleme modu **Process Events In Dynamic Update** kalsın. Bu özellik için yeni paket kurmak zorunda değilsin. [Unity'nin IMGUI açıklaması](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/use-imgui-alongside-input-system.md).

Console'u aç (**Window > General > Console**); import/derleme bitene kadar bekle. İlk kırmızı hata başka bir scriptte de olsa önce onu çöz. Bu Mac'te gerçek Unity derlemesi yapılmadı.

## 3. Aşama 2 için yeni görsel sahneyi üret

**Görsel köy ve low-poly karakter için bu adım gereklidir.** Eski CompleteDemo sahnesi korunur; sadece kaynakları kopyalayıp eski sahneyi açarsan yeni kaydedilmiş geometriyi görmezsin. Güncel Scene Builder yeni varlıkları ve referansları kurar:

1. Play'i durdur, Prefab düzenleme modundan çık. Açık sahnelerde kaydedilmemiş değişiklik varsa kendin kaydet veya geri al; araç bunları kaydetmez veya atmaz.
2. **Tools > GameUP > Create Demo Scene** seç.
3. Araç temiz yeni sahne açar; mevcut temiz sahneleri kapatır. Eski dosyaları silmez. Yeni yol **`Assets/SonBirUmutDemo/Scenes/SonBirUmutVisualDemo.unity`**; doluysa numaralı benzersiz yol kullanılır.
4. Yeni gerçek mesh (`Art_Canopy`, `Art_GableRoof`, `Art_Icosahedron`) ve materyaller **`Assets/SonBirUmutDemo/Generated/VisualDemo...`** altına kaydedilir; önceki materyaller ezilmez. Console'da oluşturulan sahne yolunu kontrol et.
5. Hierarchy'de `Kırsal harita - Son Bir Umut` altında `01 - ENERJİ İSTASYONU`, `02 - SU DEPOSU`, `03 - TARIM VE DOĞAL ALAN` gruplarını kontrol et. Oyuncu, kamera ve Demo oturumu referansları bağlı olmalı. `Low-poly köylü`, `Köy detayları`, `Su havuzu - kuraklık görünümü`, `DemoCharacterVisual` ve genişletilmiş DemoWorldView alanlarını da kontrol et.

Dünya koordinatlarıyla enerji **(3, 0, 0)**, su **(-2, 0, 5)**, tarım **(6, 0, -5)** konumundadır. Su göstergesi deponun önündeki mavi çubuktur. Enerji noktasında sarı kutu, tarımda ürün sıraları ve tahta işaret bulunur.

## 4. Oyna ve hareketi öncelikle kontrol et

1. Game görünümünü **16:9 / 1280×720 veya büyük** yap. Play ve Game'e tıklama sonrası **Enter** veya Başla düğmesiyle girişten çık.
2. Oyuncunun `DemoPlayerMotor` Inspector değerleri yeni sahnede **3.5 / 12 / 16 / 600** olmalı (hız / hızlanma / yavaşlama / derece-sn dönüş). CharacterController kalmalı; Rigidbody eklenmemeli.
3. W/A/S/D ve dört çaprazı dene. W+A sırasında A'yı bırak, W'den S'ye geç, tüm tuşları bırak. Hızlanma ve fren kısa; dönüş yumuşak; çapraz hız aynı olmalı. Klavye sekiz hedef yön verir; yönelim ve hız aralarında sürekli değişir, analog kontrol eklenmedi.
4. Kameranın açısı karakter dönerken sabit kalmalı. 30/60/120 FPS'de hissi karşılaştır. [Hareket notları](MOVEMENT.md) ve [kontrol listesi](../UNITY_TEST_CHECKLIST.md) sonuçlarını doldur.
5. Kart açılır açılmaz yatay hareket durmalı; Esc/Vazgeç ile kapanınca yeniden hızlanmalı. Giriş ve final de hareketi kilitler. Ev, ağaç, depo ve sınır çarpışmalarını dene. Dekoratif karar modelleri çarpışmasızdır.

## 5. Üç karar ve final

Sıra **enerji → su → tarım**. Önce başka noktaya gitmek kart açmamalı. HUD sıradaki noktayı yazar. Her kartta mevcut kaynaklara göre maliyet ve tur dahil **net** etkiler gösterilir. Uygulanamayan seçenek gerekçesiyle devre dışıdır. Beklemek kaynak tüketmez.

Aşağıdaki saf C# sonuçlarını Unity'de karşılaştır (sıra: bütçe / enerji / su / bitki / emisyon):

| Enerji → Su → Tarım | Son değerler | Beklenen final |
| --- | --- | --- |
| Güneş → Verimlilik → Doğal alanı koru | 20 / 55 / 42 / 59 / 0 | Yeniden Doğuş |
| Dizel → Pompa → Sulama | 45 / 70 / 43 / 73 / 18 | Eksik Dönüşüm |
| Güneş → Mevcut düzen → Depola | 40 / 55 / 17 / 8 / 0 | Kaybedilen Gelecek |

Ek kontroller:

- Enerji tasarrufu bu dalda ücretsizdir. Güneşten sonra pompa maliyeti 25; tasarruftan sonra 30 olmalı. Kapasite su kazanımına yansımalı.
- Güneş/dizel → mevcut su düzeni sonrasında sulama su yetersizliğinden engellenmeli. Depola her zaman seçilebilir kalmalı.
- Aynı noktaya dönmek ikinci seçim veya ikinci maliyet üretmemeli.
- Panel/jeneratör/tasarruf, pompa/vana/uyarı ve koruma/sulama/depolama grupları seçime göre açılmalı.
- Su çubuğu kaynakla değişmeli; bitki sağlığı ≥55 yeşil, 30–54 sarı, daha azı kuru görünmeli. Tarlanın rengi de değişmeli.
- Üçüncü kararda final başlığı, su/tarım/doğa özeti, öne çıkan iki karar ve süren risk görünmeli. Kamera yumuşakça uzaklaşıp/yükselip köyü sol tarafta göstermeli; yeni harita yüklenmemeli. Finalin **Köyün durumu / Kararlarının etkisi** sekmelerini ve kaydırmayı dene. **Köyü izle** paneli gizleyip kadrajı ortalamalı; rapora dönüşte sağ panele yer açılmalı.
- Finalde karakter durmalı. Yeniden Oyna; tüm kaynak, görsel, ilerleme, kamera, oyuncu ve hız durumunu sıfırlayıp girişe dönmeli. Birkaç farklı diziyle arka arkaya dene.
- İlk kez oynayan ekip üyesinin süresini ölç. **3–5 dakika hedeftir; henüz ölçülmedi.** Gerekiyorsa metin ve hareket hissi için geri bildirim yaz; yapay bekleme eklenmedi.

Formüller [DECISIONS](DECISIONS.md), 27 dizi [DECISION_MATRIX](DECISION_MATRIX.md) içindedir. Bu puanlar bilimsel ölçüm değildir. Jeneratör anlık sıcaklık artışı oluşturmaz; giriş yaklaşan kuraklığa odaklanır; hastalık veya NPC görevi içermez.

## Aşama 2 görsel regresyonu — Unity'de henüz yapılmadı

- Eğimli çatılar, pencereler/kapılar, temel, patikalar, çit, kuyu, bank ve kayalar görünmeli. Ev, kuyu, bank, çit ve harita sınırı çarpışmalarını dene; dekoratif küçük nesneler çarpışmasızdır.
- Görünen karakter kapsül değil köylü olmalı; collider hâlâ CharacterController. Yüz/burun ileri yönü göstermeli. Kol/bacak hareketi gerçek hıza bağlı olmalı; duvara dayanırken ve kart/giriş/final sırasında yürüyüş pozu sürmemeli.
- Su deposunun çubuğuna ek olarak havuz yüzeyi küçülmeli; su <20'de kuru zemin çatlakları açılmalı, su 0'da yüzey kapanmalı.
- Bitki <30'da taç yerine kuru dallar; ≥30'da taç, ≥55'te yeşil görünüm. Ürün boyu/toprak rengi sağlıkla değişmeli. Bu animasyonlar yalnız görseldir; HUD kaynakları seçim anında merkezi hesaba göre güncellenir.
- Aynı üç karar grubunu farklı rotalarda izle. Emisyon yüksek diye köyün aniden ısınması veya sağlıklı bitkinin sırf final kategorisi nedeniyle kuruması beklenmez; görsel durum gerçek kaynaklara bağlıdır.
- Tema/kontrast, beş kaynak çubuğu, sıradaki nokta etiketi, kart engeli, bildirim ve yeni final sekmeleri okunmalı. Kaydırma için fare tekerini kullan.
- Yeniden Oyna; model gruplarını, havuz/taç/ürün durumunu, adım pozunu, final sekmesi/kaydırmasını ve kamera panel konumunu sıfırlamalı.
- Sahneyi kapatıp yeniden aç; mesh/materyaller kaybolmamalı. `Scenes`, `Generated/VisualDemo...` ve `.meta` dosyaları birlikte korunmalı.

[Dosya bazında Aşama 2 raporu](VISUAL_STAGE2.md)

## Aşama 1 anlaşılabilirlik kontrolleri de korunur

- Yeni başlık **Son Bir Umut: Kuraklıktan Önce**; kısa köye dönüş anlatımı, hastalık anlatımı yok.
- Ana hedef, 0/3–3/3 ilerleme, sıradaki nokta ve kritik kaynak uyarısı kart/final dahil görünür kalmalı.
- Her kararın problem cümlesi; her kartın Kazanç / Bedeli / Sonrası bölümleri okunmalı.
- İlk güneş seçimi **40 / 55 / 32 / 41 / 0**: seçim bitki **0**, emisyon **0**; tur su kullanımı **-8**, su koşulu bitki **-8**, kuraklık bitki **-6**. Bitkideki net **-14** güneş hasarı olarak sunulmamalı.
- Alt soldaki bildirimde seçilen eylem, bütçe, ayrı bileşenler ve net sonuç görünmeli. **×** veya **Esc** hemen kapatmalı, 10 saniyede kendiliğinden kapanmalı; hareketi veya E ile sonraki etkileşimi kilitlememeli. Merkezdeki değişen nesneyi örtmediğini kontrol et.
- Sulama şimdi 5 bütçe ve 20 su harcar. **Güneş → pompa → sulama: 10 / 55 / 46 / 73 / 0, Yeniden Doğuş** olmalı. Koruma yoluyla su/bitki ödünleşimini karşılaştır.
- Finalde stok, tarım/doğa ortak göstergesi ve karar gerekçeleri okunmalı. Su kalitesinin veya ayrı ürün veriminin ölçüldüğü izlenimi oluşmamalı.
- 1280×720 ve 1920×1080'de kart, bildirim ve final metinlerinin kırpılmadığını dene. Aşama 2 arayüzü bu Mac'te render edilmedi.

## 6. Eski prototip ve sahne güvenliği

Eski `SonBirUmutDemo.unity` dosyasını koru. Yeni kod eski serileştirilmiş referansları ve tek istasyon modunu korur; ancak bu dalın hareketi ve ücretsiz tasarruf dengesi geçerlidir. Eski sahneyi yeni sürümde açarak referans uyumunu ayrıca kontrol et; henüz doğrulanmadı.

Birebir önceki sürüm için `main` kaynaklarını **ayrı Unity test projesinde** kullan. Git dalı değiştirmek dış projeye önceden kopyalanan dosyaları geri almaz. Yeni sürüme ait fazladan scriptlerle eski kaynakları karıştırma. Eski sahneleri veya Hakan'ın yerel değişikliklerini silerek geri dönüş yapma.

Sahne aracını iki kere çalıştırıp yeni numaralı yolları doğrula. Kirli sahneyle dene: uyarı verip dosyayı değiştirmemeli. Kaynak/scene dosyalarının yedeğini almadan dosya temizleme yapma.

## 7. Console ve Windows build

| Sorun | Kontrol |
| --- | --- |
| Tools menüsü yok | Editor klasörünün doğru kopyalanması ve ilk kırmızı derleme hatası |
| Eksik yeni sınıf | Güncel klasörün tamamı kopyalandı mı? Eski ve yeni kaynaklar karıştı mı? |
| InputSystem bulunamadı | Paket / Active Input Handling uyumu; elle eklenen semboller |
| Kart tıklanmıyor | Both veya Old, Editor yeniden başlatma ve Game odağı |
| Yalnız enerji görünüyor | Eski sahne açıktır; yeni araçla VisualDemo sahnesi üret |
| Referans eksik / Missing Script | Mevcut .meta korunmuş mu? Yeni sahne oluşturucu kullanıldı mı? |
| Pembe model | URP/Built-in shader ve proje pipeline eşleşmesi; HDRP kullanılmaması |
| Final kadrajı dar | 16:9 Game görünümü; IsometricFollowCamera Overview Size ve Horizontal Offset; test kaydına değerleri yaz |

Editor kontrolleri geçerse **File > Build Profiles** Windows profili aç; yeni VisualDemo sahnesini Scene List'e ekleyip başlangıç sahnesi yap. Mevcut Windows build modüllerini kullan; Mac'e SDK kurulmaz. Çıktıyı depoya ekleme. EXE içinde kontrol listesinin oynanış adımlarını tekrarla.

Test kaydı: commit, tam Unity sürümü, pipeline, input modu, Windows sürümü, FPS, hareket değerleri, süre ve ilk Console hatası. [UNITY_TEST_CHECKLIST](../UNITY_TEST_CHECKLIST.md) kutularını yalnızca gerçekten çalıştırdıktan sonra işaretle.

## Saf C# ve Git

Mevcut .NET 8/9 ile `dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md` çalışır. Harici NuGet paketi yoktur. Sonuçlar [TEST_RESULTS](TEST_RESULTS.md) içinde; bu testler Unity input/fizik testi değildir.

Düzeltmeleri feature/polish-backup-demo dalına, açık dosya yollarıyla ve staged içerik kontrolüyle commit et. `git add .`, `git add -A`, `git add -f`, force push veya otomatik main merge kullanma. Yerel kişisel belgeleri ve ekran görüntülerini değiştirme/ekleme. Test bin/obj, Unity Library/Temp, SDK ve build çıktıları izlenmez.
