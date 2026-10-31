# Aşama 2 — görsel kalite, oyuncu deneyimi ve final sunumu

## Başlangıç denetimi

Bu çalışma başlamadan önce son commit **a5a41ed8a6699cac2adb749fe632558b45f8974e** idi. `git status --short --branch` yalnız `## feature/polish-backup-demo...origin/feature/polish-backup-demo` gösteriyordu: çalışma ağacı temiz ve uzak dalla eşitti.

Aşama 2 o commit'te uygulanmamıştı. Scene Builder, karakter geometrisi, dünya görünümü ve kamera Aşama 1 sırasında değiştirilmemişti; HUD değişiklikleri metin/açıklama düzeniydi. Şimdi aşağıdaki gerçek kaynak değişiklikleri eklendi. Kullanıcının atıf yaptığı önceki Aşama 2 tam metni erişilebilir kayıtlarda bulunamadığından bu mesajdaki beş hedef esas alındı.

## Gerçek kod değişiklikleri

| Dosya (UnityAssets/SonBirUmutDemo altında) | Değişiklik |
| --- | --- |
| `Editor/DemoVisualBuilder.cs` — yeni | Gerçek düşük poligon mesh ve materyal varlıkları; eğimli çatılı/pencereli evler, temel, patika, çit, kuyu, bank, kaya, ot, ağaç ve köylü geometrisi |
| `Editor/DemoSceneBuilder.cs` | Yeni görsel yardımcıyı mevcut üç noktalı sahneye bağlar; hücre çizgili panel, jeneratör ızgarası/yakıt bidonu, arıtma filtresi, havuz ve çatlaklar; yeni referansları kurar |
| `Scripts/Player/DemoCharacterVisual.cs` — yeni | Kol/bacak pozlarını gerçek controller hızıyla değiştirir; kart/giriş/finalde sıfırlar; root/collider'a yazmaz |
| `Scripts/World/DemoWorldView.cs` | Sağlığa göre taç/kuru dal, ürün boyu ve toprak; suya göre havuz/gösterge ve çatlak görünümü; kısa görsel geçiş ve oturum değişiminde sıfırlama |
| `Scripts/UI/DemoGuiTheme.cs` — yeni | Ortak koyu/opak yüzeyler, kontrastlı metinler, vurgu renkleri, düğmeler, kaynak çubukları; runtime dokularının temizliği |
| `Scripts/UI/DemoHud.cs` | Temalı HUD/kart/giriş/bildirim, sıradaki nokta etiketi; sekmeli ve kaydırılabilir sonuç raporu; Köyü izle / rapora dön / yeniden oyna |
| `Scripts/Player/IsometricFollowCamera.cs` | Final raporu gizlenince köyü ortalar, geri açılınca sağ panele yer açar; sabit açı ve normal takip korunur |

Dokümantasyon değişiklikleri: `README.md`, `UNITY_TEST_CHECKLIST.md`, `docs/ARCHITECTURE.md`, `docs/INTEGRATION.md`, `docs/TEST_RESULTS.md`, Aşama 1'e tarihsel not eklenen `docs/POLISH_DESIGN.md` ve bu dosya.

## Oyun mantığının korunması

`Scripts/Simulation` ve `Scripts/Decisions` içindeki dosyalar, DemoGameController, DemoInput ve DemoPlayerMotor Aşama 2 başlangıcıyla birebir aynıdır. Bütçe/maliyet, kaynak etkileşimi, sıra, uygulanabilirlik ve üç final eşiği değiştirilmedi. Ek simülasyon, envanter veya görev yoktur. Görsel animasyon değerleri yalnız sunum içindir; kaynak hesaplarına geri yazılmaz.

Kamera hâlâ 35.264°/45° sabit izometrik açıdadır. Kapsülün görünen geometrisi değişmiştir; fizik hâlâ CharacterController'dır. Modelde harici rig, Animator/Root Motion veya üçüncü taraf paket yoktur. Emisyon ani sıcaklık/ışık/bitki hasarı üretmez. Örneğin yüksek emisyonlu Eksik Dönüşüm'de bitki sağlığı yüksekse sahne yeşil kalabilir; final kategorisi kaynakları görsel olarak zorla değiştirmez.

## Hakan'ın görsel sürümü açması

1. Aynı `feature/polish-backup-demo` dalını güncelle. Eski Assets klasörünü veya `.meta` dosyalarını silmeden kaynakları kopyala; **Editor dahil yeni dosyaları da al**.
2. Unity 6.3 LTS / Built-in veya URP, Input Handling Both veya Old. Console derlemesinin bitmesini bekle.
3. Açık sahne değişikliklerini kendin koru, Play/Prefab modundan çık. **Tools > GameUP > Create Demo Scene** çalıştır.
4. Yeni sahne **Assets/SonBirUmutDemo/Scenes/SonBirUmutVisualDemo.unity** veya numaralı benzersiz yola kaydolur. Üretilen gerçek mesh/materyaller **Generated/VisualDemo...** altındadır.
5. Eski CompleteDemo'yu açmak yeni geometrileri otomatik eklemez. Yeni VisualDemo sahnesini açıp Play → Enter ile test et.
6. Üç final örneğini [kurulum rehberinden](INTEGRATION.md) oyna; final sekmelerini kaydır, Köyü izle / rapora dön / Yeniden Oyna'yı dene. Windows Build Profiles başlangıç sahnesi yeni VisualDemo olmalı.

Araç eski sahne dosyalarını ezmez. Kirli sahne, Prefab düzenleme modu veya uyumsuz input ayarında durma davranışı korunur. Oluşan Scene/Generated/.meta varlıkları birlikte korunmalıdır.

## Gerçekte yapılan doğrulama

- Mevcut saf C# test koşucusu: **54 geçti, 0 başarısız**. 27 karar dizisi, iki engelli sulama alternatifi, üç final ve yeni oturum testleri dahil.
- Başlangıç içerik özetleriyle oyun mantığı/motor/giriş/oturum kaynaklarının aynı olduğu kontrol edildi.
- 22 kaynak dosyası, 5 Editor/player input sembol birleşiminde C# 9 sözdizimi taramasından geçti. Bu Unity tür/API derlemesi değildir.
- Üç prosedürel mesh'in 36 üçgeninde indeks, dejenere yüz ve dışa yön sayısal kaynak kontrolü geçti. Bu Unity mesh üretimi/render testi değildir.
- Git diff, stage listesi, gizlilik ve korunan yerel dosyalar denetlendi. Unity, Xcode veya SDK kurulmadı.

## Açık Unity kontrolleri

Bu Mac'te Unity yoktur. Görsellerin estetik sonucu, mesh import/render, shader, ayak teması, yeni fizik engelleri, waypoint yerleşimi, UI metin/kaydırma/kontrast, final kadraj geçişi, yeniden oynama görselleri ve Windows build **henüz Unity'de doğrulanmadı**. Sahne ekran görüntüsü uydurulmadı. 3–5 dakika hedefi ve FPS ölçümü de bekler.

[Unity kontrol listesi](../UNITY_TEST_CHECKLIST.md) gerçek testten sonra Hakan/Enes tarafından doldurulmalıdır. İlk sorun için commit, tam Editor sürümü, pipeline, input modu, adım ve Console hatası kaydedilmelidir.
