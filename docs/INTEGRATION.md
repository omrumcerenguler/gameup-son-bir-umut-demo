# Hakan / Enes — Windows, Unity 6.3 LTS entegrasyonu

## Durum ve dal

Bu sürüm **feature/complete-demo** dalındadır. `main` önceki tek istasyonlu prototipte bırakıldı; otomatik merge yapılmaz. Mac üzerinde Unity yoktur. Yeni hareket ve tam demo Unity'de doğrulanmadı; saf C# testleri **39/39** geçti.

## 1. Kaynakları güvenle al

Çalışma ağacındaki kendi değişikliklerini önce koru; yerel değişikliklerin üzerine yazma. Depo temizken:

```sh
git fetch origin
git switch feature/complete-demo
git pull --ff-only origin feature/complete-demo
```

İlk klonlamada `git clone --branch feature/complete-demo git@github.com:omrumcerenguler/gameup-son-bir-umut-demo.git` kullanılabilir. Bu Windows'taki ekip kurulumu içindir; mevcut Mac çalışma klasöründe yeni klon oluşturulmaz.

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

## 3. Yeni tam demo sahnesi

1. Play'i durdur, Prefab düzenleme modundan çık. Açık sahnelerde kaydedilmemiş değişiklik varsa kendin kaydet veya geri al; araç bunları kaydetmez veya atmaz.
2. **Tools > GameUP > Create Demo Scene** seç.
3. Araç temiz yeni sahne açar; mevcut temiz sahneleri kapatır. Eski dosyaları silmez. Yeni yol **`Assets/SonBirUmutDemo/Scenes/SonBirUmutCompleteDemo.unity`**; doluysa numaralı benzersiz yol kullanılır.
4. Yeni materyaller **`Assets/SonBirUmutDemo/Generated/CompleteDemo...`** altına kaydedilir; önceki materyaller ezilmez. Console'da oluşturulan sahne yolunu kontrol et.
5. Hierarchy'de `Kırsal harita - Son Bir Umut` altında `01 - ENERJİ İSTASYONU`, `02 - SU DEPOSU`, `03 - TARIM VE DOĞAL ALAN` gruplarını kontrol et. Oyuncu, kamera ve Demo oturumu referansları bağlı olmalı.

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
| Dizel → Pompa → Sulama | 35 / 70 / 43 / 73 / 18 | Eksik Dönüşüm |
| Güneş → Mevcut düzen → Depola | 40 / 55 / 17 / 8 / 1 | Kaybedilen Gelecek |

Ek kontroller:

- Enerji tasarrufu bu dalda ücretsizdir. Güneşten sonra pompa maliyeti 25; tasarruftan sonra 30 olmalı. Kapasite su kazanımına yansımalı.
- Güneş/dizel → mevcut su düzeni sonrasında sulama su yetersizliğinden engellenmeli. Depola her zaman seçilebilir kalmalı.
- Aynı noktaya dönmek ikinci seçim veya ikinci maliyet üretmemeli.
- Panel/jeneratör/tasarruf, pompa/vana/uyarı ve koruma/sulama/depolama grupları seçime göre açılmalı.
- Su çubuğu kaynakla değişmeli; bitki sağlığı ≥55 yeşil, 30–54 sarı, daha azı kuru görünmeli. Tarlanın rengi de değişmeli.
- Üçüncü kararda final başlığı ve üç kararın net etkisi görünmeli. Kamera yumuşakça uzaklaşıp/yükselip köyü sol tarafta göstermeli; yeni harita yüklenmemeli.
- Finalde karakter durmalı. Yeniden Oyna; tüm kaynak, görsel, ilerleme, kamera, oyuncu ve hız durumunu sıfırlayıp girişe dönmeli. Birkaç farklı diziyle arka arkaya dene.
- İlk kez oynayan ekip üyesinin süresini ölç. **3–5 dakika hedeftir; henüz ölçülmedi.** Gerekiyorsa metin ve hareket hissi için geri bildirim yaz; yapay bekleme eklenmedi.

Formüller [DECISIONS](DECISIONS.md), 27 dizi [DECISION_MATRIX](DECISION_MATRIX.md) içindedir. Bu puanlar bilimsel ölçüm değildir. Jeneratör anlık sıcaklık artışı oluşturmaz; hikâye tıbbi neden/iyileşme iddiası içermez.

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
| Yalnız enerji görünüyor | Eski sahne açıktır; yeni araçla CompleteDemo sahnesi üret |
| Referans eksik / Missing Script | Mevcut .meta korunmuş mu? Yeni sahne oluşturucu kullanıldı mı? |
| Pembe model | URP/Built-in shader ve proje pipeline eşleşmesi; HDRP kullanılmaması |
| Final kadrajı dar | 16:9 Game görünümü; IsometricFollowCamera Overview Size ve Horizontal Offset; test kaydına değerleri yaz |

Editor kontrolleri geçerse **File > Build Profiles** Windows profili aç; yeni CompleteDemo sahnesini Scene List'e ekleyip başlangıç sahnesi yap. Mevcut Windows build modüllerini kullan; Mac'e SDK kurulmaz. Çıktıyı depoya ekleme. EXE içinde kontrol listesinin oynanış adımlarını tekrarla.

Test kaydı: commit, tam Unity sürümü, pipeline, input modu, Windows sürümü, FPS, hareket değerleri, süre ve ilk Console hatası. [UNITY_TEST_CHECKLIST](../UNITY_TEST_CHECKLIST.md) kutularını yalnızca gerçekten çalıştırdıktan sonra işaretle.

## Saf C# ve Git

Mevcut .NET 8/9 ile `dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md` çalışır. Harici NuGet paketi yoktur. Sonuçlar [TEST_RESULTS](TEST_RESULTS.md) içinde; bu testler Unity input/fizik testi değildir.

Düzeltmeleri feature/complete-demo dalına, açık dosya yollarıyla ve staged içerik kontrolüyle commit et. `git add .`, `git add -A`, `git add -f`, force push veya otomatik main merge kullanma. Yerel kişisel belgeleri ve ekran görüntülerini değiştirme/ekleme. Test bin/obj, Unity Library/Temp, SDK ve build çıktıları izlenmez.
