# Doğrulama kaydı — Kriz Nöbeti Aşama 2/2

Dal: `feature/live-village-demo`; başlangıç `f60b0b1c822be158e894905593f753a3d553b64c`. Bu Mac'te Unity Editor yoktur.

## Bu aşamada gerçekten çalıştırılanlar

- Başlangıç regresyonu: **80 test geçti, 0 başarısız**. Önceki testlerin beklentileri ve RunStateTests değiştirilmedi.
- Yeni 36 grupla toplam **116 geçti, 0 başarısız**. Mevcut .NET 9.0.303 / net8.0 / C# 9, warnings-as-errors. Yeni SDK/paket kurulmadı.
- Görev oluşumu/istasyon/veri/durum, hazırlıkta ödül yokluğu, tek ve çift ödül engeli, altı görev sırası, güncel kaynaklar/sınırlar, arızanın tek tetiklenmesi, geç hazırlık zamanlaması, gerçekten 2 sn basılı tamir, bırakma/menzil/focus/pause/modal iptali, deadline ve tam deadline onarımı test edildi.
- Tek etkileşim önceliği; tek görevle kazanamama, iki görevin yeterliliği, olay değerlendirilmeden kazanamama, 10 aktif saniye, eşik geçişinde istikrarın sıfırlanması ve 100 sınırındaki geçerli ayar test edildi.
- Eski stratejik cache'in korunması ve bakım sonrası yeni kaynakla final/özet, 0/1/2/3 karar zaman aşımı, bakım tamam/arıza aktif sonu, kaçırılan olay sonrası devam/başarı, iki replay ve final sonrası ödül engeli geçti.
- Tam oturumlar: **27/27 stratejik dizide kriz başarısı**, iki engelli sulamada ücretsiz depolama alternatifi; üç enerji seçimiyle başarı; üç sürdürülebilirlik kategorisi. Güneş/verimlilik/koruma Yeniden Doğuş, dizel/pompa/sulama kriz başarılı ama Eksik Dönüşüm. Kötü dizel/mevcut/depolama + tek bakım + kaçırılan arıza 240 sn'de Kaybedilen Gelecek. Ayrıntı [CRISIS_BALANCE](CRISIS_BALANCE.md).
- İlk simülasyon rotasında ikinci görevden 10 sn sonra erken başarıya erişildiğinden 27 sn sonra üçüncü görevi tamamlama denemesi reddedildi. Tam üç bakım simülasyon rotası, üçüncü görevi başarı penceresi dolmadan yapacak şekilde düzeltildi. En az iki bakım kuralı değişmedi; iki görevle başarı ayrıca test edildi.
- **34 Unity C# dosyası × 5 Editor/player input sembol birleşimi** Roslyn C# 9 parser kontrolü: **0 sözdizimi hatası**. Unity API/tür çözümlemesi veya Unity derlemesi değildir.
- `git diff --check`, seçili stage dosyaları/içerikleri, gizli veri kalıpları, doküman bağlantıları ve korunan kaynak/kişisel dosya içerik kontrolleri yapıldı. Eski dallara merge/force push yoktur.

Çalıştırılan komut:

```sh
DOTNET_CLI_HOME=/private/tmp/sonbirumut-dotnet DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_GENERATE_ASPNET_CERTIFICATE=false dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release --no-restore -p:UseSharedCompilation=false -- --crisis-report docs/CRISIS_BALANCE.md
```

## Henüz Unity'de doğrulanmayanlar

Unity 6.3 LTS API derlemesi, scene builder/varlık importu, sahne kapatıp açma ve serileştirilmiş referanslar, işaretlerin bütün stratejilerde fiziksel erişimi, gerçek E pressed/held ve Shift, tek kare girdi sırası, CharacterController, gerçek odağa dönüş callback'leri, IMGUI 1280×720/1920×1080 metin/scroll, Built-in/URP sis/ışık görünürlüğü ve Windows EXE **test edilmedi**. Yeni bir doğrulanmış Unity hata kaydı yoktur; test edilmeden hatasız çalışma iddiası yoktur.

Varsayılan rota simülasyonu 178 aktif saniyedir; hareket/dolaşma zamanları modele verilmiştir. Gerçek 3–4 dakika hedefi ölçülmedi; hızlı oyuncu daha erken bitirebilir. En az iki bakım kuralı üçüncü işi zorunlu kılmaz. Varsayılan denge için bütün enerji yollarında kriz başarısı doğrulandı; keyfî Inspector ayarlarında aynı kazanılabilirlik garanti değildir. Basılı tamir ve aynı kare menzil geçişleri özellikle Windows'ta denenmelidir.

[Hakan için son test planı](FINAL_UNITY_TEST.md) · [Genel kontrol listesi](../UNITY_TEST_CHECKLIST.md)

---

Aşağıdaki bölümler **tarihsel Aşama 1 ve polish kayıtlarıdır**; güncel toplam 116'dır.

# Doğrulama kaydı — Kriz Nöbeti Aşama 1/2

Dal: `feature/live-village-demo`, kaynak `ee6f8a0`. Tarih: 2026-10-31.

## Bu görevde gerçekten çalıştırılanlar

- Değişiklik öncesi mevcut **54 test geçti, 0 başarısız**.
- Oturum katmanı ve 26 yeni grupla **80 test geçti, 0 başarısız**. Mevcut .NET 9.0.303 / net8.0 / C# 9, warnings-as-errors; harici paket/SDK indirilmedi.
- Koşma hızı niyeti, Shift bırakma ve modal kilit politikası; sayacın pause/fractional/clamp/reset davranışı; sayısal ayar doğrulaması; baskı sınırları/artış tavanı ve enerji/su/bitki etkisi; intro/kart/odak/native pause; faz geçişleri; eski tek/üç karar modu; 0/1/2 karar erken sonu; üç karar sonrası süre sonu; tekrarlanan bitişin etkisizliği; yeniden oynama ve gelecekteki azaltma API'si test edildi.
- Yeni legacy sarmalayıcı 27 temel seçim dizisinde doğrudan GameSession ile kaynak/sonuç karşılaştırmasından geçti. Önceki 54 testin kaynak/karar/final beklentileri değiştirilmedi.
- **29 C# dosyası × 5 Editor/player input sembol birleşimi** C# 9 Roslyn sözdizimi kontrolünden hatasız geçti. Bu yalnız parser kontrolüdür; Unity API/tür çözümlemesi veya Unity derlemesi değildir.
- `git diff --check`, seçili stage listesi/içerikleri, korunan 9 stratejik kaynak dosyasının içerik özeti, 3 kişisel dosyanın içerik/değişiklik zamanı ve korunan dal commit'leri karşılaştırıldı.

Çalıştırılan komut (mevcut restore çıktılarıyla):

```sh
DOTNET_CLI_HOME=/private/tmp/sonbirumut-dotnet DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_GENERATE_ASPNET_CERTIFICATE=false dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release --no-restore -p:UseSharedCompilation=false
```

## Unity'de yapılmayanlar ve riskler

Editor kurulu değil. Unity 6.3 LTS derlemesi, sahne üretimi/yeniden açılması, referanslar, gerçek klavye, IMGUI fare/ölçek/metin taşması, koşma hissi, çarpışma/sınır, karakter pozu, odak callback sırası, render pipeline/sis ve Windows EXE testi **yapılmadı**. Windows'ta [kontrol listesi](../UNITY_TEST_CHECKLIST.md) uygulanmalı. 1280×720 panel boyutları kaynakta düzenlendi; okunaklı olduğu render edilerek doğrulanmadı.

Kriz modunda bakım, acil arıza ve yeni kazanma sistemi yoktur. Üç karar sonrası yalnız köyü inceleme/sayaç/baskı devam eder. Atmosferin varsayılan akışta azalması henüz oynanışa bağlı değildir. 240 aktif saniye toplam deneyim süresini ölçmez. Sis için Built-in/URP ve Use Basic Fog kapalı alternatifinin okunurluğu ekip kontrolüdür. Eski stratejik hesaplar değişmedi; Unity sahne uyumluluğu saf C# testlerinin kapsamı değildir.

---

Aşağıdaki bölüm **kaynak polish dalının tarihsel test kaydıdır**; 54 sayısı bu sürümün güncel toplamı değildir.

# Doğrulama kaydı — görsel Aşama 2

## Gerçekten çalıştırılan kontroller

- Mevcut .NET 9.0.303 SDK, net8.0 hedefi ve C# 9 ile saf mantık kaynakları derlendi. Yeni SDK veya NuGet paketi kurulmadı.
- `dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md`: **54 test grubu geçti, 0 başarısız**. Önceki 39 test grubu korundu; güneş emisyon beklentisi 0 olarak güncellendi ve 15 yeni grup eklendi.
- 27 temel karar dizisinin 25'i doğrudan tamamlandı. Su yetersizliğinden engellenen 2 sulama dizisinde ücretsiz depolama ile tamamlanma doğrulandı. Üç final kategorisine ulaşıldı; hem koruma hem sulama ile Yeniden Doğuş mümkün.
- Yeni kapsam: seçim etkisi / tur kullanımı / su koşulu / dışsal kuraklığın ayrımı; değişen kuraklığın seçim etkisini değiştirmemesi; kaynak sınırlarında bileşenlerle net değişimin uzlaşması; doğru bildirim; kart metinlerinin tamamlığı; alternatif başarılı kaynak dağılımları; mevcut durum/ID'lerden sonuç açıklamaları; yeni oturumda özet/geçmiş sıfırlanması.
- Önceki sıra, bütçe/enerji/su yetersizliği, atomik ret, tekrar engeli, kaynak sınırları, sıfır kaynakla ilerleme, pompa bağımlılığı ve verimlilik testleri de geçti.
- 22 C# kaynak dosyası, geçici Roslyn denetleyicisiyle C# 9 düzeyinde 5 Editor/player input sembol birleşiminde tarandı; **sözdizimi hatası yok**. Bu Unity API/tür çözümlemesi veya Unity derlemesi değildir; araç depoya eklenmedi.
- `git diff --check`, açık stage dosya listesi ve içerik incelemesi yapıldı. Korunan dallar ve yerel PDF/görseller için commit/içerik özeti ile değişiklik zamanı karşılaştırması yapıldı.

## Aşama 2 regresyonu

- Aynı 54 test yeniden çalıştırıldı: 54 geçti, 0 başarısız. Kaynak/karar/final hesapları, DemoGameController, DemoInput ve DemoPlayerMotor içerik özetleri `a5a41ed` başlangıcıyla karşılaştırıldı; birebir aynı.
- Üç prosedürel mesh'in kaynak üçgen dizileri sayısal olarak denetlendi: Canopy 8, GableRoof 8, Icosahedron 20 üçgen. İndeks sınırı, dejenere üçgen ve dışa dönük yüz kontrolleri geçti. Bu denetim Unity mesh import/render testi değildir.
- `git diff --check` ve korunan PDF/görsellerin içerik/değişiklik zamanı denetlendi. Test kaynakları veya karar dengesi Aşama 2 için değiştirilmedi.

## Kaynak dalın ekip doğrulaması

Kullanıcının aktarımına göre Hakan `feature/complete-demo` sürümünü Unity'de açtı; oynanış videosunda enerji, su ve tarım seçiliyor, dünya nesneleri değişiyor ve **Eksik Dönüşüm** finaline ulaşılıyor. Bu, kullanıcının aktardığı ekip doğrulamasıdır; yeni polish sürümünün testi değildir.

## Henüz yapılmayanlar

Bu Mac'te Unity Editor yoktur. Yeni Aşama 2 sürümünün Unity 6.3 LTS derlemesi, prosedürel mesh/materyal importu, sahne üretimi, köylü modeli/ayak teması, yeni engeller, havuz/dal/ürün geçişleri, tema/kart/sekme/kaydırma, hedef etiketi, finali izleme kadrajı ve Windows build doğrulanmadı. Korunan hareket, kamera ve sahne yapısında da regresyon kontrolü Hakan/Enes'tedir.

3–5 dakika deneyim süresi ölçülmedi. Bu aşamada görsel kaynak kodu eklendi; yeni sahne Windows Editor aracında üretilmelidir. Bu Mac'te sahne/render çıktısı veya doğrulama ekran görüntüsü üretilmedi. Doğrulanmış yeni bir Unity hata kaydı yoktur; Editor testi olmadan hatasız çalışma iddiası yoktur.

[Kontrol listesi](../UNITY_TEST_CHECKLIST.md) kutularını yalnız gerçekten çalıştırılan polish adımlarıyla doldurun. Commit, tam Unity patch sürümü, Windows, pipeline, input modu ve ilk Console hatasını kaydedin. Saf C# geçişi UI/fizik testinin yerine geçmez.
