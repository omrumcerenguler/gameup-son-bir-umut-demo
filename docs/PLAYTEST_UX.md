# Aşama 3 — oyun deneyimi ve görsel geri bildirim

Dal: `feature/playtest-ux`. Korunan yedek: `feature/live-village-demo` / `1912d943819e6e115192f8d335981f64aed44e1f`. Kullanıcının aktarımına göre Hakan Aşama 2'nin önemli bölümlerini Unity'de çalıştırdı; bu belgedeki yeni sürüm bu Mac'te Unity Editor'da derlenmedi veya oynanmadı.

## Tespitler ve sınırlar

Kaynak incelemesinde normal ekran 153 px HUD + ikinci uyarı bandı, uzak istasyonda sürekli kontrol metni ve 690×105 bildirim içeriyordu. Hakan'ın “teknik prototip/çok yazı” geri bildirimine uygun olarak bunlar sadeleştirildi. Bu bir kaynak yerleşimi tespitidir; Mac'te render ölçümü değildir.

Mevcut 116 test ve yeni 30/60/120 FPS zaman adımı testlerinde **45 sn arıza / 2 sn tamir için doğrulanmış bir mantık hatası bulunmadı**. Onarım süreleri değiştirilmedi. Çok görevli profil için istasyon araması tamamlanmış ilk kayda takılmak yerine o istasyondaki açık varyasyonu seçer; finalde sabit `/3` yerine gerçek görev toplamı kullanılır. Bunlar yeni çeşitliliğin entegrasyon düzeltmeleridir.

Ana menü, duraklatma menüsü veya ses sistemi eklenmedi. Mevcut Intro ve odak/native pause saat korumaları sürer. Telifli varlık kopyalanmadı; primitive/önceki prosedürel mesh'ler kullanılır.

## İsteğe bağlı yeni sahne

**Tools > GameUP > Create Playtest Demo Scene** → `Assets/SonBirUmutDemo/Scenes/SonBirUmutPlaytestDemo.unity`; `Generated/PlaytestDemo...` varlıkları. Dolu yollar benzersiz numaralanır, mevcut sahneler/materialler ezilmez. Tek harita ve tek oyun sahnesidir; sahneler arası oyun geçişi yoktur.

Yeni gizli serileştirilmiş `playtestMode` anahtarı `GameMode.PlaytestDemo` açar. Diğer üç Editor menüsü eski mod ve harita düzenini üretmeye devam eder. Sadece kaynakları kopyalamak mevcut kaydedilmiş CrisisDemo haritasını veya görev profilini değiştirmez; yeni sahne oluşturulmalıdır. Ortak HUD/atmosfer kaynaklarının bu dalda değiştiği, fakat yedek dalın hiç değiştirilmediği unutulmamalı.

## HUD ve kararlar

- Normal HUD 61 px tek ince şerit: süre, baskı, bütçe/enerji/su/bitki/emisyon için küçük göstergeler ve karar/bakım sayısı. En fazla bir kısa hedef satırı; acilde bu satır yerine tek acil kartı.
- Etkileşim ipucu yalnız menzilde görünür. Küçük bildirimin ömrü kararda/bakımda 3, arıza sonucunda 4 saniyedir; uzun kaynak dökümü normal ekrandan kaldırıldı.
- Kart paneli 870×355, üç belirgin seçim; kısa fayda/bedel, gerçek önizlemeden maliyet ve kompakt tur sonu etkileri. Düğme yüksekliği 40 px. Teknik hesap bileşenleri yalnız isteğe bağlı Etki ayrıntısı alanında. Köy üstte/yanlarda görünür.
- Acil panelde büyük kalan saniye, mavi onarım barı ve 12 sn altında artan kenar vurgusu/nabız. Uzak hedef ekranda kenara sıkıştırılan Yön etiketiyle yönlendirilir; aynı anda bakım uyarı panelleri yığılmaz.

1280×720 referans UI ölçeği ve 1920×1080 görüntü Unity'de henüz render edilmedi. Türkçe metin/font, tıklama ve kaydırma gerçek ekip kontrolüdür.

## Harita ve hareket

Yeni sahne 28×24 yerine **32×28**: alan 672 → 896, **%33.3 artış**. Harita sıfırdan değiştirilmedi; üç ev, rezervuar, karakter, istasyonlar ve mevcut ana collider yapısı korunur. Yeni ağaç kümeleri, kanal yatağı/suyu, kaya/otlar, rezervuar korkulukları ve bölge renk şeritleri eklendi. Renk şeridi bir su havuzu veya sağlıklı tarla gibi sabit dolu yüzey değildir; mevcut kaynağı yanlış göstermez.

| Nokta | Kök X/Z | Yaklaşım |
| --- | --- | --- |
| Enerji | 8 / 1 | Ön taraf Z≈−0.2; yol Z−1.4'e gelir |
| Su | −8 / 8 | Ön taraf Z6; yol Z5.8'e gelir |
| Tarım | 8 / −7 | Ön taraf Z−9.5; yol Z−10.1'e gelir |

1.9 birim geniş yollar merkez kavşaktan bölgelere gider. Su hattında ara yol noktası evin önünü dolaşır; tarım yolu tarlanın önüne gelir. Sınırlar yeni zemine göre ayarlanır. Ağaçların bir kısmı yeni kenar konumlarına taşındı; ilave küçük ağaç/kaya/ot/işaret/kanal dekorları yeni blokaj yaratmamak için çarpışmasızdır. Ana ev, eski ağaç gövdeleri, istasyon ve sınır collider'ları korunur. Kaynak geometrisinin böyle kurulması, fiziksel erişimin test edildiği anlamına gelmez.

Yeni sahnede yürüyüş **3.5**, sprint çarpanı **1.9**, hedef koşma **6.65**; Inspector'dan ayarlanabilir. CharacterController, normalize kamera yönlü giriş, hızlanma 12/fren 16 ve yumuşak dönüş korunur. Eski menüler 1.6 kurar; kaydedilmiş eski sahnelerin serileştirilmiş değeri korunur.

## Görev çeşitliliği ve denge

Yeni profile toplam **5 bakım varyasyonu**, bir de tek acil olay: oturumda 5–6 müdahale olayı. Mevcut üç görev korunur; aynı mekanik üstüne iki küçük veri varyasyonu eklenir. 10 yeni mekanik veya sınırsız yeniden doğan iş yoktur.

| Görev | İstasyon | Baskı / ek kaynak |
| --- | --- | --- |
| Enerji bağlantısını denetle | Enerji | −12 |
| Su deposundaki kaybı gider | Su | −14; en çok +4 korunmuş su |
| Sulama vanasını ayarla | Tarım | −14; en çok +4 bitki desteği |
| Tasarruf bağlantısını kontrol et | Enerji | −4 |
| Su filtresini temizle | Su | −4 |

Başlangıçta enerji/su açıktır. Tamamlama yalnız bir kez ödül verir; sıradaki kilitli işler deterministik olarak açılır, aynı anda **en fazla 2**, aynı istasyonda en fazla 1 iş açıktır. İstasyona ikinci ziyaret yeni bir iş olabilir; tamamlanan ID tekrar ödüllendirilmez. Oyuncu iki açık görev arasında seçim yapabilir. Tek arıza tüm E girdilerinden önceliklidir.

Yeni modda başarı için **en az 4 bakım**, üç stratejik karar, değerlendirilmiş arıza ve ≤35 baskıda 10 aktif sn gerekir. Beşinci bakım isteğe bağlıdır. Eski CrisisDemo modunda üç görev / en az iki bakım kuralı ve eski 116 test korunur. Yeni görevler toplam sınırlı rahatlamayı 40'tan 48'e çıkarır; kaynak desteği hâlâ yalnız +4 su/+4 bitkidir. Stratejinin önemini kaybettiren bir ekonomi kurulmadı.

[27 yeni tam oturum simülasyonu](PLAYTEST_BALANCE.md): tüm enerji seçimleriyle kriz başarısı, üç sürdürülebilirlik kategorisi ve eksik müdahalede kısmi/kötü sonuçlar korundu. Örnek rota **195 aktif saniye**; hareket/okuma ölçümü değil, modele verilen zaman çizelgesidir. 240 sn sınırı korunur. Hızlı oyuncu daha erken bitebilir, kart okuma dahil duvar saati 4 dakikayı aşabilir; gerçek 3–4 dakika hedefi Windows'ta ölçülmeli.

## Atmosfer ve dünya tepkisi

`AtmosphereProfile`, yalnız baskı ve oyuncu-kamera uzaklığından görünüm parametresi üretir. Emisyon girdi değildir. Düşük baskı açık/sıcak, yüksek baskı uzak görüşü belirgin azaltan pus ve solgun tonlar verir. Linear fog başlangıcı her zaman oyuncu-kamera uzaklığından en az 7 birim sonra; baskı 100'de bitiş bundan 21 birim sonradır. Kamera takip/final uzaklığı da hesaba katılır. Saf test bu matematik aralığını doğrular; gerçek shader/fog görünürlüğünü doğrulamaz.

Built-in/URP temel fog denenir; Use Basic Fog kapalıyken ışık/ambient/arka plan alternatifi sürer. Bu alternatif renk değiştirir, fiziksel uzak nesne örtmesi sağlamaz. Pipeline veya ağır paket değiştirilmedi.

`DemoStationFeedback` her kalıcı istasyondadır: bakım onayı, su arızasında üç basit damla, tamirde durmuş sızıntı/yeşil bağlantı, kaçırılan olayda kırmızı rezerv kaybı izi. Damla sayısı su üretimi/tüketimi hesabı değildir. Güneş çalışma göstergesi yalnız gerçekten Solar seçiliyken, panel grubunun içinde çalışır; seçilmeyen tesis etkin gösterilmez. Su filtresi için kalıcı depoda pasif giriş süzgeci, tarımda ortak tasarruf vanası vardır; pompa veya sulama tesisi seçimini varsaymaz.

Rezervuar/kanal suyu güncel suyla, ürün/taç/dal ve zemin güncel bitki sağlığıyla değişir; çok yüksek baskıda hafif solgunluk eklenir. Bakım ve tamir kaynak/baskıyı düşürdükçe görsel toparlanır. Sis fiziksel hava kirliliği veya iklim değişikliğinin ortadan kalkması değildir. Dünya görünümü kaynak değerlerini kendi kendine değiştirmez.

## Doğrulama ve dosyalar

**136 test grubu geçti, 0 başarısız** (116 korunan + 20 yeni). 30/60/120 FPS arıza/onarım adımları, aktif görev sınırı/terfi/tekrar engeli, dört görev eşiği, replay, 1.9 hız niyeti ve atmosfer sınırları dahil. **37 C# × 5 sembol** C# 9 parser kontrolünde hatasız; Unity API/tür derlemesi değil.

Başlangıç commitine göre toplam **25 proje dosyası** değişti. Yeni dosyalar: `Scripts/Simulation/AtmosphereProfile.cs`, `Scripts/World/DemoStationFeedback.cs`, `Editor/DemoPlaytestBuilder.cs`, `Tests/LogicTests/PlaytestUxTests.cs`, bu belge, `PLAYTEST_BALANCE.md`, `PLAYTEST_UNITY_TEST.md`. Güncellenenler: GameRunState, MaintenanceTask, RunOutcome, DemoGameController, DemoStory, DemoPlayerMotor, DemoHud, DemoGuiTheme, DemoWorldView, DemoCrisisAtmosphere, DemoSceneBuilder, DemoVisualBuilder, test Program ve başlangıç/kurulum/test belgeleri. Scripts/Editor yolları UnityAssets/SonBirUmutDemo altındadır.

Unity 6.3 LTS API derlemesi, gerçek E/Shift/çarpışma, bölge erişimi, karakter/kamera hissi, fog yakın görüş ve iki çözünürlük, sahneyi yeniden açma ve Windows EXE testi bekler. [Hakan'ın Aşama 3 listesi](PLAYTEST_UNITY_TEST.md) uygulanmalıdır.
