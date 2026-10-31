# Son Bir Umut: Kuraklıktan Önce — GAMEUP Türkiye 2026

**KRİZ NÖBETİ** · Unity 6.3 LTS / C# / Windows · Dal: `feature/release-candidate`.

Release Candidate, korunan `feature/playtest-ux` / `391d253c0eabbd9283580e7e52fa463d28105d5e` üzerine eklenmiştir. Bu depo kaynak teslimidir; Mac'te Unity Editor yoktur. **Unity API derlemesi, gerçek oynanış, ses ve Windows build henüz doğrulanmadı.**

## Bu sürüm

- Mevcut köyün geniş izometrik görünümünde ana menü: Yeni Oyun, Nasıl Oynanır, Ayarlar ve Çıkış.
- Esc ile gerçek kullanıcı duraklatması; Devam Et, Yeniden Başlat, Ayarlar, Ana Menüye Dön. Kart açıkken Esc önce kartı kapatır. Kullanıcı pause'u, native pause ve focus bağımsızdır; sayaç/baskı/arıza/ödül durur, onarım iptal olur.
- Altı özgün kısa, kodla üretilen ses efekti; gerçek master ses/mute, yalnız ses tercihleri için PlayerPrefs. Tam oyun kaydı veya yükleme düğmesi yoktur.
- Kısa final sunumu; Köyü İzle, Yeniden Oyna ve Ana Menü. Kaynak, tesis, görev, onarım, atmosfer, kamera ve HUD yeni oturumda sıfırlanır; ses tercihi korunur.
- Korunan oynanış: 32×28 low-poly köy, WASD/1.9×Shift, üç karar, beş bakım/en az dört, tek su arızası/45 sn pencere/2 sn E basılı onarım, 240 aktif sn, kriz baskısı ve üç genel final.

Oyun dengesi, karar ekonomisi ve sürdürülebilirlik eşikleri değiştirilmedi. Değerler örnek oyun puanlarıdır; pus hazırlığın metaforudur, iklim değişikliğinin ortadan kalktığı iddia edilmez. P0/P1 kaynak özellikleri uygulandı; isteğe bağlı P2 geçiş efektleri eklenmedi. Windows teslim onayı ekip testi bekler.

## Hakan için ilk adım

**Önce çalışan Unity projesini yedekle.** Temiz kaynak depoda `feature/release-candidate` dalını al. `UnityAssets/SonBirUmutDemo` içeriğini dış Unity projesindeki `Assets/SonBirUmutDemo` üzerine, klasörü ve eski `.meta`/Scenes/Generated dosyalarını silmeden kopyala. Yeni Audio ve Simulation dosyaları dahil Scripts ve Editor birlikte aktarılmalı. Built-in 3D veya URP, Active Input Handling **Old veya Both** kullan.

Console derlemesi bitince **Tools > GameUP > Create Release Candidate Scene**. Yeni sahne `Assets/SonBirUmutDemo/Scenes/SonBirUmutReleaseCandidate.unity`, varlıklar `Generated/ReleaseCandidate...`; dolu yollar numaralanır. Eski sahneler ezilmez. Kaydet/kapat/yeniden aç, Play ile ana menüden başla.

| Menü | Kaynak profil |
| --- | --- |
| Create Release Candidate Scene | Playtest oynanışı + yeni menü/pause/ses/final akışı |
| Create Playtest Demo Scene | Korunan Aşama 3: beş bakım/en az dört, 1.9 sprint |
| Create Crisis Demo Scene | Korunan üç bakım/en az iki, eski harita, 1.6 sprint |
| Create Crisis Foundation Scene | Süre/baskı temeli; görev/arıza yok |
| Create Demo Scene | Eski üç stratejik karar → final |

Yeni menü/pause/ses yalnız RC sahnesinde etkinleşir. Ortak giriş metni artık profilin gerçek görev sayısını kullanır. Önceki dallar korunur; mevcut kaydedilmiş sahne kendiliğinden RC'ye dönüşmez.

[Hakan'ın RC kabul ve teslim listesi](docs/RELEASE_CANDIDATE_TEST.md) · [Kurulum](docs/INTEGRATION.md) · [RC uygulama ayrıntıları ve dosyalar](docs/RELEASE_CANDIDATE.md)

## Doğrulama

Mevcut SDK ile, harici test paketi veya yeni kurulum olmadan:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release
```

**157 test grubu geçti, 0 başarısız**: önceki 136 korundu, 21 yeni grup eklendi. Menü/oturum/pause/onarım/focus/tekrar/ses modeli, RC akışı üzerinden 27 strateji ve üç genel final test edildi. **41 C# dosyası ×5 Editor/player input sembol birleşimi** Roslyn sözdizimi kontrolünden hatasız geçti; bu Unity API/tür derlemesi değildir.

[Gerçek test kaydı ve sınırları](docs/TEST_RESULTS.md) · [Mimari](docs/ARCHITECTURE.md) · [Korunan 27 rota/denge](docs/PLAYTEST_BALANCE.md)

Gerçek GUI tıklaması/klavye odağı, sesin duyulması ve PlayerPrefs kalıcılığı, sahne referansları/render, 1280×720/1920×1080 ve Windows EXE açılış/çıkışı ekip tarafından doğrulanmalı. 195 aktif sn simüle rota gerçek oyuncu süresi değildir. Build Profiles Scene List'te **RC sahnesi ilk ve etkin** olmalı; teslimde EXE ile _Data/UnityPlayer.dll ve bütün üretilen yan dosyalar birlikte korunmalı. Tam Unity proje arşivi için Assets/Packages/ProjectSettings ve bütün gerekli .meta dosyalarını al; ayrıntılar kabul listesindedir.

Kişisel belgeler/görseller, gizli bilgiler ve build/cache çıktıları Git'e eklenmez. Main veya eski dallara merge/force push yapılmaz.

Tarihsel belgeler: [Aşama 3 UX](docs/PLAYTEST_UX.md), [Aşama 3 test planı](docs/PLAYTEST_UNITY_TEST.md), [Aşama 2](docs/FINAL_UNITY_TEST.md), [Stratejik kurallar](docs/DECISIONS.md).
