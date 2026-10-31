# Son Bir Umut: Kuraklıktan Önce — GAMEUP Türkiye 2026

**KRİZ NÖBETİ** · Unity 6.3 LTS / C# / Windows · Dal: `feature/final-pacing`.

Korunan çalışan kaynak: `feature/release-candidate / 5eeb35e7dad7343e2591f1d3b03870ad47d84305`. Bu depo kaynak teslimidir; Mac'te Unity Editor yoktur. **Yeni sahne, fiziksel erişim, GUI/ses ve Windows EXE doğrulanmadı.**

## Son oynanış iyileştirmesi

- **10 ayrı bakım tanımı ve 10 ayrı fiziksel node:** üç enerji, dört su, üç tarım/doğa. Her node ayrı TaskId/LocationId/konum taşır; üç stratejik istasyonun üzerine yığılmaz.
- **Oturumda altı görev**, her bölgeden iki; 54 farklı altılı set. En fazla iki aktif çağrı, kriz başarısı için en az dört bakım. Görevler konum/bölge ve açılma saatine göre sunulur; ayrı bakım son tarihleri yoktur.
- Yakın doğru noktada E, tek ödül, aktif sarı işaret/tamamlanmış yeşil onay ve mevcut bakım sesi. Acil olay bakım etkileşimlerinden önce gelir.
- Kısa hedef, ekran kenarı yönlendirmesi, açık görev süresi/yeni hedef bildirimi; son 60/20 sn için daha belirgin sayaç uyarısı.
- Korunan 32×28 köy, WASD/1.9×Shift, üç karar, tek arıza 45 sn / 2 sn onarım, 240 aktif sn ve üç genel final. Karar ekonomisi/emisyon/final eşikleri değişmedi.
- RC ana menü, gerçek ses/mute/PlayerPrefs, bağımsız Esc pause, restart/menu/final/replay korunur. Yeni oturum görev seti ve on node işaretini mevcut dünya/kamera/HUD ile birlikte temizler.

Değerler oyun dengelemesidir; iklim değişikliğinin tamamen çözüldüğü iddia edilmez. Yeni büyük harita, NPC, crafting, envanter, ekstra arıza, paket veya oyun kaydı eklenmedi.

## Hakan için ilk adım

**Önce çalışan Unity projesini yedekle.** Temiz kaynak depoda `feature/final-pacing` dalını al. `UnityAssets/SonBirUmutDemo` içeriğinin tamamını dış projenin `Assets/SonBirUmutDemo` klasörüne kopyala; yeni dosyalar dahil Scripts ve Editor birlikte olmalı. Eski `.meta`, Scenes ve Generated dosyalarını silme. Built-in/URP, Active Input Handling **Old veya Both**.

Console derlemesi bitince **Tools > GameUP > Create Final Pacing Scene**. Sahne `Assets/SonBirUmutDemo/Scenes/SonBirUmutFinalPacing.unity`, varlıklar `Generated/FinalPacing...`; dolu yollar benzersiz numaralanır. Kaydet/kapat/yeniden aç, Play ile ana menüden başla.

| Menü | Profil |
| --- | --- |
| Create Final Pacing Scene | On konum, oturumda altı/en az dört, RC menü/pause/ses |
| Create Release Candidate Scene | Korunan beş görev/en az dört ve RC kabuğu |
| Create Playtest Demo Scene | Korunan Aşama3, beş görev/en az dört |
| Create Crisis Demo Scene | Eski üç bakım/en az iki |
| Create Crisis Foundation Scene | Süre/baskı temeli; görev/arıza yok |
| Create Demo Scene | Eski üç stratejik karar→final |

Kaydedilmiş eski sahne kendiliğinden Final Pacing olmaz. [Hakan'ın kısa test listesi](docs/FINAL_PACING_UNITY_TEST.md) · [Kurulum](docs/INTEGRATION.md) · [Konumlar/mimari/dosya dökümü](docs/FINAL_PACING.md).

## Gerçekte çalıştırılanlar

**178 test grubu geçti, 0 başarısız:** önceki 157 korundu, 21 yeni. Kimlik/yanlış konum/menzil/modal/tek ödül, çağrı sırası/saatler,54 görev seti, menü/restart/final,27 strateji/üç genel final ve eski profiller test edildi. 44 Unity C# dosyası ×5 sembol birleşimi sözdizimi kontrolünden hatasız geçti; bu Unity API/tür derlemesi değildir.

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release
```

[Rota/süre tahmini](docs/FINAL_PACING_BALANCE.md): yürüyüş 3.5 ve 1.9×sprint, şişirilmiş engel grafiği, yön bulma/etkileşim/tepki payı ve olay saatleriyle hızlı 87–90, yeni oyuncu 155–175 aktif sn. Üç enerji seçeneği için de 54/54 sette başarı yolu; geciken profil 240 sn'de geçerli sonuç. **Gerçek Unity hareketi veya oyuncu süresi ölçümü değildir.**

[Test kaydı ve riskler](docs/TEST_RESULTS.md) · [Mimari](docs/ARCHITECTURE.md). Özellikle on noktanın erişimi, gerçek tempo/arama, işaret/kamera, UI taşıması, pause/focus, ses ve Windows build ekip tarafından denenmeli. Build Profiles Scene List'te Final Pacing sahnesi ilk/etkin; EXE/_Data/UnityPlayer.dll ve bütün yan dosyalar birlikte teslim edilmeli. Tam Unity proje yedeği Assets/Packages/ProjectSettings ve gerekli .meta içermeli; [teslim rehberi](docs/RELEASE_CANDIDATE_TEST.md).

Eski dallar/kişisel belgeler korunur. Kişisel medya, gizli bilgi, build/cache Git'e eklenmez; merge veya force push yapılmaz. [Korunan RC uygulaması](docs/RELEASE_CANDIDATE.md) · [Aşama3 UX](docs/PLAYTEST_UX.md).
