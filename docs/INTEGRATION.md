# Windows / Unity 6.3 LTS kurulum rehberi

## Doğrulama durumu

Kaynaklar Mac üzerinde Unity Editor olmadan geliştirildi. Saf C# testleri .NET ile çalıştırıldı: **21 test grubu geçti, 0 başarısız**. Bu, Unity derlemesi veya oynanış testi yerine geçmez. Editor sahne üretimi, input, fizik, IMGUI, materyaller ve Windows build henüz Unity'de doğrulanmamıştır.

## 1. Gerçek Unity projesini oluştur

1. Windows'ta Unity Hub üzerinden **Unity 6.3 LTS (6000.3.x)** Editor kullan. Ekipte aynı patch sürümünü kullanın ve test kaydına tam sürümü yazın. Mac geliştiricisinin Unity, Xcode veya SDK kurması gerekmez.
2. Git deposunu klonla veya temiz çalışma ağacında `git pull --ff-only` ile güncelle.
3. Unity Hub ile **Universal 3D (URP)** veya **3D Built-in Render Pipeline** şablonundan yeni bir test projesi oluştur. HDRP desteklenmez. Harici model, UI paketi veya Asset Store içeriği gerekmez.
4. Test projesini Git deposunun dışında tut. Bu aşamada kaynakların asıl konumu depodaki `UnityAssets` klasörüdür; boş bir Unity projesini bu klasör diye açmaya çalışma.
5. **`UnityAssets/SonBirUmutDemo` klasörünün tamamını**, `Editor` alt klasörü dahil, test projesinin **`Assets/SonBirUmutDemo`** konumuna kopyala. Sonuç `Assets/UnityAssets/...` olmamalı. `Tests` ve `docs` klasörlerini Unity Assets içine kopyalama. `.gitkeep` dosyaları aktarılmayabilir.
6. Unity'nin import ve script derlemesini bitirmesini bekle. `.meta` dosyalarını Editor üretsin. Kaynaklara elle `.meta`, `.unity`, `.csproj` veya paket manifesti ekleme.

## 2. Giriş ayarını kontrol et

**Edit > Project Settings > Player > Other Settings > Configuration > Active Input Handling**:

- **Both (yeni paketli proje için önerilen):** Projede Unity'nin resmi `Input System` paketi kurulu olmalı. Oyun klavyesi `Keyboard.current` kullanır; IMGUI fare olayları eski backend üzerinden gelir. Input Actions varlığı veya PlayerInput bileşeni gerekmez.
- **Input Manager (Old):** Yeni paket gerekmez. Kod doğrudan `Input.GetKey` / `GetKeyDown` kullanır; özel eksen ayarı istemez. IMGUI kartları eski backend'i kullanır.
- **Input System Package (New) tek başına desteklenmez:** Klavye adaptörü bulunsa da bu mod runtime IMGUI'ye fare olaylarını iletmez. Sahne oluşturucu açıklama gösterip durur; daha önce üretilmiş sahne de çalışma sırasında Console hatasıyla oturumu başlatmaz. **Both** veya **Old** seçip Editor'ı yeniden başlat.

Koşullu derleme `ENABLE_INPUT_SYSTEM` ve `ENABLE_LEGACY_INPUT_MANAGER` sembollerini kullanır; bunları elle eklemeyin. Ayar değiştiğinde Unity isterse Editor'ı yeniden başlatın. Yeni sistemde güncelleme modu varsayılan **Process Events In Dynamic Update** olarak kalsın (`Project Settings > Input System Package`). Kurulu olmayan paketi varmış gibi sembol tanımlamayın.

IMGUI kısıtı Unity'nin resmi [IMGUI ve Input System birlikte kullanımı](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/use-imgui-alongside-input-system.md) belgesinde açıklanır. Giriş ayarları için [Input System kurulum rehberi](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/Installation.html) ve [Unity 6.3 Input Manager açıklaması](https://docs.unity.com/en-us/engine/6000.3/manual/input/legacy/class-input-manager) incelenebilir. Oyun Windows masaüstü klavye ve fare kullanımını hedefler.

## 3. Demo sahnesini oluştur

1. **Window > General > Console** penceresini aç. Önce tüm kırmızı script hatalarını çöz. Projedeki başka bir scriptin derleme hatası da menünün görünmesini engelleyebilir.
2. Açık sahnelerde kaydedilmemiş değişiklik varsa **kendin kaydet** veya geri al. Araç bu durumda uyarıp durur; mevcut sahneleri kaydetmez veya değişiklikleri atmaz.
3. Play modu kapalıyken ve Prefab düzenleme modundan çıktıktan sonra **Tools > GameUP > Create Demo Scene** seç. New-only input ayarı varsa önce yukarıdaki giriş ayarını düzelt.
4. Araç temiz bir sahne açar; önceki temiz sahneleri kapatır. Mevcut sahne dosyalarını silmez veya üzerlerine yazmaz. Harita, evler, ağaçlar, su deposu, sarı enerji istasyonu, kapsül oyuncu, kamera, ışık ve arayüz referansları otomatik kurulur.
5. Sahne **`Assets/SonBirUmutDemo/Scenes/SonBirUmutDemo.unity`** altında kaydedilir. Bu yol doluysa `GenerateUniqueAssetPath` ile numaralı yeni dosya seçilir. Her çalıştırmada materyaller de **`Assets/SonBirUmutDemo/Generated/Demo.../`** altında yeni bir klasöre kaydedilir. Materyaller sahneye bağlı gerçek Editor varlıklarıdır; Play sonrası kaybolmamalıdır.
6. Console'da sahnenin oluşturulduğu yolun yazıldığını doğrula. Araç sahneyi Build Profiles listesine kendiliğinden eklemez.

Araç Unity'nin [NewScene](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/scenemanagement/editorscenemanager/newscene), [GenerateUniqueAssetPath](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/assetdatabase/generateuniqueassetpath) ve [SaveScene](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/scenemanagement/editorscenemanager/savescene) API'lerini kullanır. Bu API tercihi çalışma testi yapıldığı anlamına gelmez.

## 4. Play ile uçtan uca doğrula

1. **Game** görünümünü 16:9, tercihen 1280×720 veya daha büyük ayarla. Play'e bas ve klavye odağı için Game görünümüne tıkla.
2. Ortografik izometrik kamera kapsülü yumuşakça takip etmeli. W ekranın üst yönüne, D sağına götürmeli; çapraz hareket daha hızlı olmamalı. Evler ve harita sınırları oyuncuyu durdurmalı, kapsül zeminden düşmemeli.
3. Sarı kontrol kutulu istasyonun önüne yaklaş. `[E] Enerji istasyonunda karar ver` ipucu yalnızca menzil içinde ve karar tamamlanmamışken görünmeli.
4. E ile üç kartı aç. WASD artık karakteri yürütmemeli. Esc ve **Vazgeç** kartları kapatmalı ve hiçbir kaynak değişmemeli.
5. Güneş seçeneğini tıkla: bütçe **40**, enerji **55**, su **32**, bitki **41**, emisyon **1** olmalı. Panel grubu ve yeşil tamamlanma lambası görünmeli; jeneratör ve tasarruf işareti kapalı olmalı. Sonuç mesajını kontrol et.
6. E'ye tekrar bas: karar yeniden açılamamalı ve ikinci maliyet kesilmemeli.
7. **Yeniden oyna** ile başlangıç değerlerini **100 / 20 / 40 / 55 / 0**, oyuncu konumunu ve görsellerin sıfırlanmasını kontrol et. Dizel seç: **75 / 70 / 29 / 41 / 18** ve jeneratör görünmeli. Yeniden başlayıp tasarrufu seç: **90 / 15 / 40 / 53 / 0** ve yeşil tasarruf işareti görünmeli.
8. Oyuncuyu beklet: kaynaklar zamanla kendiliğinden değişmemeli. Bu prototipte yalnızca karar bir tur ilerletir; henüz su pompası yoktur. Emisyon anlık yerel sıcaklık artışı üretmez.
9. Play'i durdurup yeniden başlat. Console'da `NullReferenceException`, `Missing Script`, shader veya input hatası olmamalı. Sahneyi kapatıp tekrar açarak materyallerin ve referansların kalıcı olduğunu doğrula.
10. Sahne oluşturma menüsünü yeniden çalıştır: eski sahne ve materyaller değişmemeli, yeni numaralı sahne oluşmalı. Kaydedilmemiş sahneyle çalıştır: uyarı vermeli ve hiçbir sahneyi değiştirmemeli.
11. Kullanılan input modunu kaydet. Mümkünse **Old** ve **Both** modlarında ayrı oturumlarla klavye ve IMGUI kart tıklamalarını doğrula. **New-only** modunda sahne aracının uyarıp durduğunu ve mevcut sahnenin açıklayıcı Console hatası verdiğini kontrol et.

## Console sorunlarını giderme

| Belirti | Kontrol |
| --- | --- |
| Tools menüsü yok | Kopyalama yolu, `Editor` klasörünün varlığı ve ilk kırmızı derleme hatası |
| `UnityEngine.InputSystem` bulunamadı | Resmi Input System paketi ve Active Input Handling; elle tanımlanmış input sembollerini kaldır |
| Eski Input API için `InvalidOperationException` | Doğru güncel kaynakları kopyaladığını ve input ayarını değiştirdikten sonra Editor yeniden başladığını kontrol et |
| Kartlar tıklanmıyor / IMGUI giriş uyarısı | Active Input Handling ayarını Both veya Old yapıp Editor'ı yeniden başlat |
| Tuşlar çalışmıyor | Game odağı, kartların açık olup olmadığı ve New input için Dynamic Update ayarı |
| Pembe materyal veya shader bulunamadı | URP/Built-in şablonuyla pipeline ayarının eşleşmesi; HDRP yerine desteklenen şablon |
| Demo referansları eksik / NullReference | Scriptleri elle boş sahneye eklemek yerine sahne oluşturucuyu çalıştır; Inspector'da Missing Script olup olmadığını kontrol et |
| Kaydetme hatası | Proje Assets klasörüne yazma izni, Console'daki ilk hata ve disk durumu; kaynak sahneyi elle taklit etme |

## Saf C# otomatik testleri

Depo kökünde, mevcut .NET 8 veya 9 SDK ile:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release
```

Bu, harici test paketi kullanmayan bir console test koşucusudur; `dotnet test` projesi değildir. Başarısızlıkta çıkış kodu 1 verir. Üretimde kullanılan dört saf C# dosyasını doğrudan derler; mantığın ayrı bir kopyasını test etmez. `NuGet.Config` harici paket kaynaklarını kapatır. Gerekli SDK yoksa bu adım atlanır; büyük SDK kurulumu yapılmaz.

Ayrıca 11 kaynak dosyası C# 9 sözdizimi açısından 5 Editor/player input sembol birleşiminde geçici Roslyn denetleyicisiyle tarandı; hata bulunmadı. Bu denetim Unity API veya tür doğrulaması yapmaz.

21 test grubu; üç seçeneğin kesin sonuçlarını, bütçe yetersizliği ve tam bütçeyi, bilinmeyen kimlikleri, tekrar seçimin engellenmesini, birden fazla kayıtlı noktayı, kaynak sınırlarını, su/kuraklık/bitki ilişkisini, emisyon ayrımını, pompa kapasitesini, bağımsız oturumları ve geçersiz tanımları kapsar. Son grup 432 başlangıç/seçenek birleşiminin durum kurallarını kontrol eder. Bunlar Unity input, fizik, UI veya Editor testleri değildir.

## Windows build ve test kaydı

Editor kontrolleri geçtikten sonra **File > Build Profiles** üzerinden Windows profili oluştur. Üretilen demo sahnesini Scene List'e ekle; başka örnek sahneler varsa demo sahnesinin başlangıç sahnesi olduğunu kontrol et. Mevcut Windows build modüllerini kullan; eksik modül varsa ekipte uygun kurulu makinede derle. Çıktıyı depoya ekleme. Çalıştırılabilir dosyada hareket, kart tıklaması ve yeniden oynamayı tekrar dene.

Test kaydına commit kimliği, tam Editor sürümü, pipeline, input modu, platform, geçen/başarısız adımlar ve Console hatalarını yaz. Bu Mac üzerinde doğrulanmış gibi raporlama.

## Git ve kaynak paylaşımı

Kaynak değişikliklerini `UnityAssets/SonBirUmutDemo` altına geri taşı ve dokümanları güncelle. İleride gerçek Unity projesi depoya alınacaksa `Assets` ve `.meta` dosyaları, `Packages/manifest.json`, `packages-lock.json`, `ProjectSettings` ve `ProjectVersion.txt` birlikte incelenmelidir; mevcut kaynak düzenini tek başına değiştirme.

Yalnızca açık dosya yollarını stage et; `git add .`, `git add -A`, `git add -f` veya force push kullanma. `git diff --cached --name-status` ve `git diff --cached` ile incele. Yerel PDF, PNG/JPG/JPEG ve WhatsApp dosyaları yerinde kalmalı, değiştirilmemeli ve Git'e eklenmemeli. Depo herkese açıktır; sır, kişisel belge ve özel e-posta paylaşma. Test `bin/obj`, Unity `Library/Temp` ve build çıktıları izlenmez.
