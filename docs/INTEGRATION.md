# Windows entegrasyonu — Kriz Nöbeti Aşama 2/2

Dal **feature/live-village-demo**, Unity **6.3 LTS (6000.3.x)**, Windows / Built-in 3D veya URP. Yeni kod Mac'te Unity Editor ile derlenmedi veya oynanmadı. [Hakan'ın ayrıntılı test planı](FINAL_UNITY_TEST.md) teslim öncesi uygulanmalıdır.

## Kaynakları güvenle al

Önce çalışan dış Unity projesini ve yerel değişikliklerini yedekle. Kaynak depon temizken:

```sh
git fetch origin
git switch feature/live-village-demo
git pull --ff-only origin feature/live-village-demo
```

Yeni klonda `git clone --branch feature/live-village-demo git@github.com:omrumcerenguler/gameup-son-bir-umut-demo.git` kullanılabilir. Bu ekip kurulumu içindir; Mac çalışma klasöründe yeni klon açılmaz.

1. Unity Hub'da mevcut **6.3 LTS** projesini kullan; gerekirse depodan ayrı Built-in 3D/Universal 3D test projesi oluştur. Tam patch sürümünü kaydet. Bu kaynak klasörü gerçek Unity projesi değildir.
2. `UnityAssets/SonBirUmutDemo` içeriğini `Assets/SonBirUmutDemo` üzerine **klasörü silmeden** kopyala. Scripts ve Editor birlikte; yeni dört Simulation dosyası ve CrisisStationMarker dahil olmalı.
3. Mevcut `.meta`, Scenes, Generated ve yerel değişiklikleri koru. Yeni dosyaların .meta'sını Editor üretir. Tests/docs ve kişisel belgeleri Assets içine kopyalama.
4. **Edit > Project Settings > Player > Other Settings > Active Input Handling = Input Manager (Old) veya Both**; gerekirse Editor'ı yeniden başlat. Both yalnız mevcut Input System paketiyle. New-only runtime IMGUI fare olayları için uygun değildir. Input Actions / PlayerInput gerekmez; sembolleri elle ekleme.
5. **Window > General > Console**: import/derleme bitmeli; ilk kırmızı hatayı önce çöz. Tools görünmüyorsa Editor klasörü ve ilk derleme hatasını kontrol et. Unity API derlemesi burada yapılmadı.

## Doğru yeni sahneyi üret

Play kapalı, Prefab düzenleme dışında, açık sahnelerde kaydedilmemiş çalışma yokken:

**Tools > GameUP > Create Crisis Demo Scene**

Araç üç istasyonlu köyü, karakter/kamera/HUD/dünya/atmosfer referanslarını ve kalıcı köklere bağlı üç bakım/bir arıza işaretini kurar. Yeni yol **Assets/SonBirUmutDemo/Scenes/SonBirUmutCrisisDemo.unity**; varlıklar **Generated/CrisisDemo...**. Doluysa numaralı benzersiz yollar; eski dosyalar ezilmez. Sahneyi kaydedip kapat/yeniden aç; referans ve materyal kalıcılığını kontrol et.

Diğer menüler korunur:

| Menü | Akış |
| --- | --- |
| Create Demo Scene | Eski VisualDemo: üç stratejik karar → final; yeni görev/arıza yok |
| Create Crisis Foundation Scene | Aşama 1 temel sahne: süre/baskı var, görev/arıza/erken başarı yok |
| Create Crisis Demo Scene | Tam yeni kaynak akışı: üç karar, bakım, tek arıza, erken başarı veya süre sonu |

Eski bir sahneyi açmak yeni işaretleri eklemez. Yeni CrisisDemo sahnesi oluştur; eski sahne ve Generated varlıklarını silme.

## Oynanış

1280×720 / 16:9 Game, Play, Enter/Başla. WASD, Sol Shift 1.6× koşma. Enerji (3,0,0) → su (-2,0,5) → tarım (6,0,-5); yakınken E, üç karttan seçim. Kart açıkken hareket ve bağımsız sayaç durur.

Üçüncü kararda görevler açılır. Sarı işaretli ana istasyonlarda **yeni E basışı** tek seferlik bakım yapar; sıra serbesttir. Su pompası seçilmese bile su kökü ve işareti aktiftir. 12 aktif saniye sonra tek kırmızı su arızası çıkar; aktif arıza E önceliğini alır. Menzilde E'yi 2 aktif saniye tut; bırakma/uzaklaşma/odak kaybı ilerlemeyi sıfırlar. Onarımda süre ilerler. Onarım sonrası E basılı kalınca bakım otomatik ödüllendirilmez; bırakıp yeniden bas.

Arızanın 45 aktif saniyesi kaçırılırsa bir kez su -6, baskı +8; oyun devam eder. Üç bakım toplam -40, tamir -18 baskı. Su bakımı +4 korunmuş su, tarım bakımı +4 bitki desteği; sınırlar ve tek ödül korunur. Kaynak/atmosfer canlı güncellenir.

Üç karar, en az iki bakım, arızanın değerlendirilmesi ve baskı ≤35 altında 10 aktif saniye: erken kriz başarısı. Tek bakım yetmez. Kriz başarısı ile genel sürdürülebilirlik ayrı gösterilir; dizel yüksek emisyonla kriz başarısına rağmen Eksik Dönüşüm olabilir. 240 saniye sonunda eksik hazırlık/aktif arıza dahil her durumda geçerli sonuç; Yeniden Oyna tüm oturumu sıfırlar.

## Inspector ve hızlı QA

Demo oturumu / DemoGameController ayarları Play başlangıcında veya Yeniden Oyna'da okunur:

| Ayar | Varsayılan |
| --- | ---: |
| Duration Seconds / Initial Pressure | 240 / 60 |
| Stable Threshold / Stable Duration | 35 / 10 |
| Emergency Delay / Emergency Window / Repair Duration | 12 / 45 / 2 |
| Base / Water / Plant / Energy Risk Rise | .04 / .04 / .03 / .02 |
| Maximum Rise Per Second | .25 |

Süre ve istikrar/onarım pozitif; arıza penceresi en az onarım süresi olmalı. Gecikme negatif olamaz. Geç hazırlıkta gecikme kalan sürenin dörtte biriyle sınırlanır. Yeni süre/baskı alanları her iki kriz modunda, bakım/arıza ayarları yalnız CrisisDemo'da etkilidir.

Play dışında süreyi 5 yaparak 0/1/2 kararda sonu, 15 yaparak tamamlanmış bakım/çözülmemiş arıza sonunu dene. Sonra 240'a dön. Arıza kaçırma, iki replay, Alt+Tab (EXE dahil), üç enerji seçimi ve pompasız su işareti en önemli regresyonlardır. Use Basic Fog kapalıyken hafif ışık/ambient alternatifi kalır; pipeline/paket değişmez.

## Build ve hata kaydı

Editor kontrolleri geçince **File > Build Profiles** Windows profili, **Scene List** içinde **yeni CrisisDemo sahnesini başlangıç** yap. Mevcut modüllerle build al. EXE'yi Editor dışında dene. EXE + aynı isimli _Data klasörü + UnityPlayer.dll ve üretilen gerekli yan dosyaları birlikte koru. Build'i kaynak deposuna ekleme.

Tools yok: ilk Console hatası/Editor kopyası. Pembe model: URP/Built-in shader uyumu. Görev yok: yanlış Foundation/VisualDemo sahnesi veya eski kaynak. Pompasız görev yok: yeni CrisisDemo'daki kalıcı kök/marker referansını kontrol et; seçenek görseline taşımamalısın. Kart tıklanmıyor: Old/Both, yeniden başlatma ve Game odağı. Onarım tamamlanmıyor: menzil, E held ve arıza süresi; focus değişimi ilerlemeyi sıfırlar.

Commit, tam Unity patch, Windows, pipeline/input, çözünürlük/FPS, strateji, görev/arıza ve ilk Console/Player.log hatasını yaz. 3–4 dakika duvar saati hedefi henüz ölçülmedi; 178 aktif saniyelik simülasyon gerçek deneyim süresi değildir.

[Son Unity test planı](FINAL_UNITY_TEST.md) · [Denge raporu](CRISIS_BALANCE.md) · [Mantık test kaydı](TEST_RESULTS.md)

Yalnız feature/live-village-demo'ya açık dosya yollarıyla commit; staged listeyi incele. Kişisel medya/anahtarlar, SDK, bin/obj ve Unity Library/build çıktıları eklenmez. Force push veya eski dallara otomatik merge yapılmaz.
