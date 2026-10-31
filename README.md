# Son Bir Umut: Kriz Nöbeti — GAMEUP Türkiye 2026

Unity **6.3 LTS**, C#, Windows için low-poly izometrik köy demosu. Güncel dal **feature/live-village-demo**; Aşama 2, `f60b0b1` üstüne eklenmiştir. Bu Mac'te Unity Editor yoktur; yeni kaynakların Unity derlemesi ve oynanışı henüz ekip tarafından doğrulanmalıdır.

## Oynanış kaynaklarında eklenenler

- Kamera yönlü WASD, yumuşak hız/dönüş ve Sol Shift ile 1.6× koşma; mevcut CharacterController ve harita korunur.
- Enerji → su → tarım için üç stratejik karar; kısa odak kartları, maliyet, net etkiler ve isteğe bağlı hesap ayrıntısı.
- Kararlar sonrası bağımsız sırada üç tek seferlik bakım: enerji denetimi, su kaybını giderme, sulama vanası.
- Bir su hattı arızası: kırmızı işaret/uyarı, menzilde E'yi 2 aktif saniye tutarak onarım; bırakma/uzaklaşma/odak kaybı ilerlemeyi sıfırlar.
- 240 aktif saniye, 0–100 kriz baskısı ve hafif sis/ışık sunumu. Giriş/kart/odak dışı süre durur; gerçek zamanlı onarımda ilerler.
- Üç karar + en az iki bakım + arıza sonucu değerlendirilmiş + baskı ≤35 altında 10 aktif saniye: erken kriz başarısı. Eksik hazırlık/arıza dahil süre sonları güvenli sonuç üretir.
- Kriz nöbetini yönetmek ve uzun vadeli sürdürülebilirlik ayrıdır. Dizel ile kriz başarısı mümkündür; yüksek emisyon en iyi genel finali engeller. Üç eski kategori korunur.
- Bakım/arıza sonrası HUD, dünya ve final güncel kaynakları kullanır; stratejik karar geçmişi korunur. Yeniden Oyna oturumu sıfırlar.

Kaynaklar boyutsuz örnek oyun puanlarıdır. Sis kriz hazırlığının görsel metaforudur; emisyon anlık sıcaklık yaratmaz, oyun iklim değişikliğini ortadan kaldırdığını söylemez. Yeni paket, model, NPC, crafting, envanter veya ses sistemi eklenmedi.

## Hakan için başlangıç

**Önce çalışan Unity projesini yedekle.** Ardından bu dalın `UnityAssets/SonBirUmutDemo` içeriğini dış Unity projesindeki `Assets/SonBirUmutDemo` üzerine, mevcut `.meta`, `Scenes` ve `Generated` dosyalarını silmeden kopyala. Yeni scriptleri ve Editor klasörünü birlikte aktar.

Active Input Handling: **Old veya Both**; New-only IMGUI fare tıklamalarını desteklemez. Built-in 3D veya URP kullan; HDRP desteklenmez. Console derlemesi bitince **Tools > GameUP > Create Crisis Demo Scene**.

| Menü | Benzersiz sahne yolu (Assets/SonBirUmutDemo altında) | Mod |
| --- | --- | --- |
| Create Demo Scene | Scenes/SonBirUmutVisualDemo.unity | Eski üç karar → final; zaman/görev/arıza yok |
| Create Crisis Foundation Scene | Scenes/SonBirUmutCrisisFoundation.unity | Aşama 1 süre/baskı temeli; bakım/arıza/erken başarı yok |
| **Create Crisis Demo Scene** | **Scenes/SonBirUmutCrisisDemo.unity** | **Yeni üç karar + bakım + arıza + kriz başarısı** |

Dolu yollar numaralanır; eski sahneler/materyaller ezilmez. Yeni oyun için yeni CrisisDemo sahnesi üretilmelidir; eski Foundation sahnesini açmak yeni görevleri kendiliğinden eklemez. Bu depo doğrudan Unity projesi değildir.

[Hakan'ın tam Windows test planı](docs/FINAL_UNITY_TEST.md) · [Entegrasyon](docs/INTEGRATION.md) · [Mimari ve yeni sınıflar](docs/ARCHITECTURE.md)

## Gerçek otomatik doğrulama

Mevcut .NET 8/9 SDK ile; yeni paket/SDK gerekmez:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --crisis-report docs/CRISIS_BALANCE.md
```

**116 test grubu geçti, 0 başarısız**: önceki 80 grup korundu, 36 kriz grubu eklendi. 27 stratejik dizinin tamamı tam oturum simülasyonunda kriz başarısına ulaştı; iki engelli sulama dizisinde ücretsiz depolama alternatifi kullanıldı. Her enerji seçimiyle başarı ve genel sonuçlarda üç kategori doğrulandı. Örnek tam müdahale rotası 178 aktif saniyede bitti; bu, Unity'de ölçülen yürüyüş/deneyim süresi değildir.

[27 oturumun sayısal raporu](docs/CRISIS_BALANCE.md) · [Test kaydı ve sınırları](docs/TEST_RESULTS.md) · [Unity kontrol listesi](UNITY_TEST_CHECKLIST.md)

Unity API derlemesi, gerçek klavye/fizik/erişim, sahne referansları, okunabilirlik, sis/pipeline, 3–4 dakika hedefi ve Windows EXE **henüz doğrulanmadı**. İki bakım yeterli olabildiği için hızlı rota daha erken bitebilir; gerçek süre Windows testinde ölçülmeli.

## Kaynak düzeni ve korunanlar

`UnityAssets/SonBirUmutDemo/Editor` sahne/varlık oluşturur; `Scripts/Simulation` ve `Scripts/Decisions` saf C#; Core koordinasyon/giriş, Interaction istasyon/işaret, Player hareket/kamera, World görünüm/atmosfer, UI IMGUI sunumudur. `Tests/LogicTests` gerçek mantık kaynaklarını doğrudan derler.

`GameSession`, `DecisionCatalog`, `SimulationRules`, `EndingResult`, `EndingSummary`, mevcut istasyonlar, karakter/motor/kamera ve köy geometrisi korunur. `CurrentResources` kriz kaynaklarını stratejik snapshot'tan ayırır. Eski dallara merge veya force push yapılmaz. Kişisel PDF/görsel/video, gizli dosyalar ve build/cache çıktıları Git'e eklenmez.

Tarihsel belgeler: [Görsel polish](docs/VISUAL_STAGE2.md), [Aşama 1 altyapısı](docs/LIVE_VILLAGE_STAGE1.md), [Stratejik kurallar](docs/DECISIONS.md), [Stratejik 27 dizi](docs/DECISION_MATRIX.md).
