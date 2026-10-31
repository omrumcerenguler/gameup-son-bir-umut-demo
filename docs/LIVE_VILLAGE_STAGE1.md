# Kriz Nöbeti — yeni oynanış altyapısı, Aşama 1/2

Dal: `feature/live-village-demo`. Kaynak: `feature/polish-backup-demo` (`ee6f8a0`). Hedef Unity 6.3 LTS / Windows. Bu sürüm Mac'te Unity Editor ile derlenmedi veya oynanmadı.

## İki ayrı mod

| Menü | Yeni sahne | Akış |
| --- | --- | --- |
| Tools > GameUP > Create Demo Scene | Scenes/SonBirUmutVisualDemo.unity | Varsayılan: üç stratejik karar → mevcut final; sayaç/baskı etkin değil |
| Tools > GameUP > Create Crisis Foundation Scene | Scenes/SonBirUmutCrisisFoundation.unity | İsteğe bağlı: Intro → Preparation → CrisisManagement → süre dolunca Ending |

Yollar `Assets/SonBirUmutDemo` altındadır; doluysa araç numaralı benzersiz yol kullanır. Generated/VisualDemo ve Generated/CrisisFoundation varlıkları ayrıdır. Eski sahneler silinmez veya üzerine yazılmaz. Eski tek istasyon sahnesi tek karar → final olarak korunur.

`DemoGameController.crisisMode` gizli, serileştirilmiş ve varsayılan false olan özellik anahtarıdır. Yalnız ayrı kriz sahnesinin oluşturulması sırasında `ConfigureCrisisFoundation()` ile açılır. Normal menü bu anahtarı kapalı kurar. Bu aşamada yeni kriz modu varsayılan teslim değildir.

## Saf C# katmanı

- `GameSession.IsComplete` yalnız stratejik kararların tamamlanmasıdır. Mevcut GameSession, DecisionCatalog, SimulationRules, EndingResult ve EndingSummary hesapları değiştirilmedi.
- `GameRunState` fazları, modu, bağımsız SessionTimer'ı ve CrisisPressure'ı taşır. Unity tarafı seçimleri `Run.TryChoose` üzerinden geçirir; doğrudan `Session.TryChoose` kullanmak faz geçişini atlar.
- `Start`: Intro → Preparation. Kabul edilen son karar eski modda Ending, yeni modda CrisisManagement oluşturur. Reddedilen karar fazı veya kaynağı değiştirmez.
- `Tick(delta, cardsOpen)` yeni modda serbest oynanış sırasında zamanı ve baskıyı ilerletir. Intro/kart/odaksızlık/native pause/Ending sırasında ilerletmez. `Time.timeScale` yazılmaz.
- `SetFocus` / `SetApplicationPaused` değişikliği ilk uygun dönüş karesinin deltasını atar. Arka planda Update çalışmasa bile dönüşte biriken delta oturumu tüketmez. Çok küçük bir aktif kare süresi de bilinçli olarak atlanır.
- `Reset` yeni GameSession, Intro, tam süre, başlangıç baskısı ve boş sonuç üretir. Unity Restart da modelleri, karakter hızını/konumunu, kamerayı ve UI durumunu sıfırlar.
- `RunOutcome` her bitişte geçerli Summary üretir. 0/1/2 kararda süre bitince eksik hazırlık açıklanır; dokunulmamış kaynaklardan Yeniden Doğuş ilan edilmez. HUD artık null olabilen Session.Ending/Summary yerine Run.Result kullanır.

Aşama 1'de üç karar sonrası bakım yoktur: oyuncu köyü inceleyebilir, sayaç işlemeye devam eder. Süre sonu, tamamlanan hazırlık için mevcut stratejik kategoriyi kullanır. **Baskı ayrı bir kazanma eşiği değildir.** Yeni kriz kazanma/kaybetme sistemi Aşama 2'ye bırakıldı.

## Ayarlar

Yeni kriz sahnesinde `Demo oturumu > DemoGameController` Inspector alanları; değişiklikler Play başlangıcında veya Yeniden Oyna'da okunur:

| Alan | Varsayılan | Anlam |
| --- | --- | --- |
| Duration Seconds | 240 | Yalnız aktif serbest oynanış süresi |
| Initial Pressure | 60 | 0–100 başlangıç göstergesi |
| Base Rise | 0.04 | Saniyelik taban artış |
| Water Risk Rise | 0.04 | Su eksikliği katsayısı |
| Plant Risk Rise | 0.03 | Bitki sağlığı eksikliği katsayısı |
| Energy Risk Rise | 0.02 | Enerji eksikliği katsayısı |
| Maximum Rise Per Second | 0.25 | Saniyelik artış üst sınırı |

`artış/sn = min(üst sınır, taban + suKatsayısı*(1-su/100) + bitkiKatsayısı*(1-bitki/100) + enerjiKatsayısı*(1-enerji/100))`.

Artış yalnız gerçekten tüketilen süreyle çarpılır; son kare süreden büyükse fazlası sayılmaz. Sonuç 0–100'e sıkıştırılır. Stratejik seçimler kaynakları değiştirerek artış hızını etkiler. Emisyonun anlık sıcaklık/sis cezası yoktur. Gelecek bakım kodu `TryReducePressure(amount)` çağırabilir; yalnız CrisisManagement'ta kabul edilir. Bu çağrıyı yapan oynanış veya UI henüz yoktur. Tüm değerler örnek oyun dengelemesidir; bilimsel iklim ölçümü değildir.

## Hareket ve sunum

Sol Shift: varsayılan yürüyüşün 1.6 katı (3.5 → 5.6). Kamera yönlü normalize giriş, CharacterController, yumuşak dönüş ve mevcut collider/sınırlar korunur. Shift bırakılınca frenleme, basılınca hızlanma kullanılır. Modal kilit yatay hızı hemen sıfırlar; yerçekimi sürer. Saf testler hız niyeti/kilit politikasını denetler; gerçek tuş, çapraz hız ve çarpışma Editor testidir.

Karar paneli 1280×720 referansta 870×405'tir. Üç seçenek, kısa olay başlığı/problem, kazanç/bedel/maliyet ve dört net etki çipi vardır. Artış yararlıysa yeşil, zararlıysa kırmızıdır; emisyon için yön tersidir, sıfır nötrdür. Net etkiler tur ve kuraklığı içerir; güneşin net bitki kaybı tek başına güneşe ait etki değildir. Ayrı bileşenler isteğe bağlı **Etki ayrıntısı** ekranındadır. Engelli seçeneğin gerekçesi görünür, düğmesi kapalıdır. Ayrıntı ve Esc/Vazgeç süresince sayaç durur.

`DemoCrisisAtmosphere` yalnız yeni sahnededir. Baskı ışık/ambient/arka planı hafifçe değiştirir. Built-in veya Universal pipeline'da temel linear fog denenir; `Use Basic Fog` kapatılınca yalnız renk/ışık alternatifi kalır. Pipeline/paket değiştirilmez. HDRP mevcut oluşturucuda desteklenmez. Sis başlangıcı 24–38, sonu 75–100 birimle uzaktadır; yakın karakter ve yolların okunurluğu önceliklidir. Geçiş `MoveTowards` ile görseldir. Yeniden oynamada yeni baskıya döner, bileşen devre dışıyken başlangıç render ayarlarını geri yükler. Atmosfer iki modu aynı anda yükleyen additive sahne düzeni için tasarlanmadı.

Pipeline, shader, gölge ve final kadrajında görünürlük gerçek Unity testine bağlıdır. Sis iklimin fiziksel olarak temizlenmesini anlatmaz; oyun baskısını gösterir. Varsayılan akışta baskı azalmaz; azalmanın görsel yönü geçici Inspector test ayarıyla veya ileride bakım API'siyle doğrulanabilir.

## Eksik kalan Aşama 2

Bakım görevleri, görev etkileşimleri, acil arıza, kriz kazanma sistemi ve bu sistemlere ait UI yoktur. NPC/crafting/çöp toplama eklenmedi. 3–4 dakika hedefi 240 aktif saniyeyle sınırlandırılır; giriş/kart okuma/odak dışı süre yüzünden toplam duvar saati daha uzun olabilir. Gerçek oyuncu deneyim süresi ölçülmedi.

[Windows entegrasyonu](INTEGRATION.md) · [Gerçek test sonuçları](TEST_RESULTS.md) · [Bekleyen Unity kontrolleri](../UNITY_TEST_CHECKLIST.md)

Unity 6.3 resmi API referansları: [unscaledDeltaTime](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/time/unscaleddeltatime), [OnApplicationFocus](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/monobehaviour/onapplicationfocus), [RenderSettings.fog](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/rendersettings/fog). API belgesine bakılması bu kodun Unity'de derlendiği anlamına gelmez.

## Bu dalda değişen dosyalar

Kaynak polish commit'ine göre toplam 23 dosya:

| Alan | Dosyalar |
| --- | --- |
| Scripts/Simulation (yeni) | GameRunState.cs, GameRunSettings.cs, SessionTimer.cs, CrisisPressure.cs, RunOutcome.cs, MovementPolicy.cs |
| Scripts/Core | DemoGameController.cs, DemoInput.cs, DemoStory.cs |
| Scripts/Player | DemoPlayerMotor.cs |
| Scripts/UI | DemoHud.cs, DemoGuiTheme.cs |
| Scripts/World (yeni) | DemoCrisisAtmosphere.cs |
| Editor | DemoSceneBuilder.cs |
| Tests/LogicTests | Program.cs, RunStateTests.cs (yeni) |
| Belgeler | README.md, UNITY_TEST_CHECKLIST.md, docs/ARCHITECTURE.md, docs/INTEGRATION.md, docs/MOVEMENT.md, docs/TEST_RESULTS.md, docs/LIVE_VILLAGE_STAGE1.md (yeni) |

Scripts ve Editor yolları `UnityAssets/SonBirUmutDemo` altındadır. Sahne/mesh/materyal veya gerçek proje ayarı dosyası burada üretilmedi. Mevcut 9 Simulation/Decisions dosyası, kamera, karakter görseli ve mevcut dünya/istasyon kaynakları değiştirilmedi.
