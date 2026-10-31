# Son Bir Umut: Kriz Nöbeti — GAMEUP Türkiye 2026

Unity 6.3 LTS / C# / Windows için küçük low-poly izometrik köy demosu. **Güncel geliştirme dalı: `feature/live-village-demo`**, kaynak `feature/polish-backup-demo` (`ee6f8a0`). `main` ve önceki feature dalları korunur. Bu Mac'te Unity Editor yoktur; yeni kaynakların Unity derlemesi/oynanışı ekip doğrulaması bekler.

## Kriz Nöbeti — Aşama 1/2

- Eski üç karar → final akışı varsayılandır: **Tools > GameUP > Create Demo Scene**.
- Yeni mod yalnız **Tools > GameUP > Create Crisis Foundation Scene** ile ayrı sahnede etkinleşir.
- Sol Shift ile 1.6× koşma, yumuşak hızlanma/frenleme; giriş/kart/final hareket kilidi.
- Kısa olay başlığı, üç seçenek ve renkli net +/- etkili odak paneli. Teknik bileşenler isteğe bağlı Etki ayrıntısı ekranındadır.
- Yeni modda yapılandırılabilir 240 aktif saniye ve 0–100 kriz baskısı. Intro/kart/odaksızlıkta süre durur; odak dönüşünde zaman sıçraması atılır.
- Saf C# Intro → Preparation → CrisisManagement → Ending oturum durumu. Erken süre sonunda 0/1/2 kararda da güvenli sonuç/özet.
- Yalnız yeni sahnede hafif, yumuşak sis/ışık atmosferi; pipeline veya ağır paket değişikliği yok.

**Bakım görevleri, acil arıza ve yeni kriz kazanma sistemi henüz yoktur.** Üç karar sonrasında yeni sahnede köyü inceleme sürer; süre dolunca mevcut stratejik sonuç gösterilir. Kriz modu bu aşamada varsayılan teslim değildir. 3–4 dakika hedefi aktif sayaç süresidir; kart okuma dahil toplam süre ölçülmedi.

[Aşama 1 altyapısı ve ayarlar](docs/LIVE_VILLAGE_STAGE1.md)

## Kaynak polish dalından korunan görsel Aşama 2

`a5a41ed` yalnızca Aşama 1 kaydıydı; o commit'te görsel Aşama 2 yoktu. Kaynak polish dalında şu görsel değişiklikler eklendi:

- Kiremit çatılı/pencereli evler, taş temeller, patikalar, çit, kuyu, bank, kaya ve kuru ot detayları.
- Kapsül collider üstünde çokgen köylü modeli: baş, yüz yönü, şapka, ceket, çanta, kol/bacak; gerçek harekete bağlı basit görsel adım hareketi.
- Suya göre çekilen havuz/çatlaklar; bitki sağlığına göre yaprak veya kuru dal, ürün boyu ve toprak görünümü. Geçişler görseldir; kaynak değerleri değişmez.
- Opak koyu tema, renkli kaynak çubukları, belirgin seçim kartları ve sıradaki nokta etiketi.
- Sekmeli/kaydırılabilir final raporu ve **Köyü izle / Sonuç raporuna dön**; kamera panel gizlenince köyü ortalar.

**Yeni köy/karakter için Tools menüsünden yeni sahne üretmek gerekir.** Mevcut sahneyi yalnız açmak, kaydedilmiş geometrisini kendiliğinden değiştirmez.

[Aşama 2 dosya ve doğrulama raporu](docs/VISUAL_STAGE2.md)

## Korunan oyun döngüsü ve Aşama 1 iyileştirmeleri

- CharacterController korunarak hızlanma, frenleme ve yumuşak Y dönüşü; kamera yönüne göre WASD ve çapraz hız sınırı.
- Tek primitive köy; enerji → su → tarım sırasıyla üç nokta ve her birinde üç kart.
- Merkezi kaynak hesabı, enerjiye bağlı pompa ve suya bağlı bitki desteği; bütçe/enerji/su yetersizliğinde açıklamalı engel.
- Her aşamada ücretsiz seçenek; aynı kararın tekrar uygulanmasını önleyen oturum sistemi.
- Kısa köye dönüş / yaklaşan kuraklık hikâyesi; sürekli ana hedef, ilerleme, sıradaki nokta ve kritik kaynak uyarısı.
- Kartlarda kazanç, bedel, maliyet ve net etkiler; seçim/tur/kuraklık ayrımı isteğe bağlı ayrıntıda gösterilir. Bildirim × veya Esc ile kapatılır, 10 saniye sonra kapanır.
- Panel/jeneratör, su seviyesi, vana/pompa, tarla ve ağaç renkleriyle dünya geri bildirimi.
- Korunan üç final eşiği; su stoku, tarım/doğa, öne çıkan iki karar ve süren risk açıklaması. Mevcut final kamerası ve yeniden oynama korunur.

Kaynaklar boyutsuz örnek oyun puanlarıdır. Emisyon anlık köy sıcaklığı yaratmaz. Hastalık anlatımı kaldırıldı; motivasyon aile ve komşuların kuraklık öncesi temel ihtiyaçlarıdır. Crafting, çöp toplama, NPC görevleri, sinematik ve karmaşık animasyon eklenmedi.

## Hakan / Enes için hızlı kurulum

1. Depoda `feature/live-village-demo` dalını al; test projesindeki çalışmanı önce koru.
2. Unity **6.3 LTS**, **Universal 3D (URP)** veya **3D Built-in** projesi kullan. Kaynak klasörü doğrudan Unity projesi değildir.
3. `UnityAssets/SonBirUmutDemo` içeriğini mevcut `Assets/SonBirUmutDemo` üzerine **klasörü silmeden** kopyala. Eski `.meta`, `Scenes` ve `Generated` dosyalarını koru.
4. Active Input Handling: **Both** veya **Input Manager (Old)**. IMGUI fare girdisi nedeniyle New-only desteklenmez; gerekirse Editor'ı yeniden başlat.
5. Console derlemesi tamamlanınca **Tools > GameUP > Create Demo Scene** çalıştır. Görsel sürüm **`Assets/SonBirUmutDemo/Scenes/SonBirUmutVisualDemo.unity`** veya numaralı yeni yola kaydedilir. Yeni mesh/materyaller `Generated/VisualDemo...` içindedir; eski sahneler ezilmez.
6. Kriz altyapısını denemek için ayrıca **Create Crisis Foundation Scene** ile ayrı sahne üret. Bu sahne `SonBirUmutCrisisFoundation.unity` ve `Generated/CrisisFoundation...` yollarını kullanır; varsayılan sahne eski modda kalır.
7. Game görünümünü 16:9 aç, Play, Enter. WASD ile dolaş, Sol Shift ile koş; sıradaki noktada E, kartta Seç. Üç karardan sonra finali ve Yeniden Oyna'yı dene.

[Windows kurulum rehberi](docs/INTEGRATION.md) · [Unity test listesi](UNITY_TEST_CHECKLIST.md) · [Hareket değişikliği](docs/MOVEMENT.md) · [Mimari](docs/ARCHITECTURE.md)

## Otomatik testler

Mevcut .NET 8 veya 9 SDK ile:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md
```

Harici test paketi gerekmez. **80 test grubu geçti, 0 başarısız.** Önceki 54 grup korundu; 26 oturum/sayaç/baskı/koşma politikası grubu eklendi. 27 dizinin 25'i doğrudan tamamlandı; 2 sulama dizisi su yetersizliği nedeniyle doğru biçimde engellendi ve ücretsiz alternatifle final doğrulandı. Üç final kategorisine ulaşıldı. Unity hareketi, fizik, UI ve Editor derlemesi bu testlerin kapsamında değildir.

[Polish tasarımı ve güneş sorununun nedeni](docs/POLISH_DESIGN.md) · [Karar kuralları](docs/DECISIONS.md) · [27 dizi raporu](docs/DECISION_MATRIX.md) · [Doğrulama kaydı ve açık kontroller](docs/TEST_RESULTS.md)

## Kaynak düzeni

| Konum | İçerik |
| --- | --- |
| `UnityAssets/SonBirUmutDemo/Editor` | Benzersiz sahne ve materyal üreten Editor aracı |
| `Scripts/Core` | Oturum koordinasyonu, giriş adaptörü, merkezi hikâye metni |
| `Scripts/Player` | Hareket ve sabit açılı takip/final kamerası |
| `Scripts/Interaction` | Korunan EnergyStation ve su/tarım için DecisionStation |
| `Scripts/Decisions`, `Scripts/Simulation` | UnityEngine'den bağımsız karar, kaynak, geçmiş ve final hesapları |
| `Scripts/World`, `Scripts/UI` | Dünya görselleri ve IMGUI |
| `Data` | Sonraki veri varlıkları için ayrılan klasörler |
| `Tests/LogicTests` | Gerçek mantık kaynaklarını doğrudan derleyen test koşucusu |

## Eski prototip ve gizlilik

Aşağıdaki görsel Aşama 2 açıklamaları kaynak dalın tarihsel kapsamıdır. Bu dalda kontrolcü, giriş, motor ve HUD yeni altyapı için genişletildi; mevcut stratejik hesap dosyaları değiştirilmedi.

Eski sahne dosyaları değiştirilmez. Serileştirilmiş `player` / `station` referansları korunur; eski sahne tek istasyon modunda kalır. Kaynak polish dalındaki görsel Aşama 2, önceki oyun mantığını ve motoru koruyordu; güncel dal bunların üstüne yukarıdaki oturum ve koşma eklerini getirir. Kamera ve dünya kodundaki değişiklikler sunum içindir. Aşama 1 dengeleme farkları: güneş emisyonu +1 → 0; mevcut kanallarla sulama bütçesi 15 → 5. Aşama 2 modeli ve köy ayrıntıları C# Editor aracıyla üretilir; harici model paketi gerekmez. Birebir önceki davranış için `main` kaynaklarını ayrı test projesinde kullan; dal değiştirmek tek başına dış Unity projesindeki kopyayı güncellemez.

[GitHub](https://github.com/omrumcerenguler/gameup-son-bir-umut-demo) herkese açıktır. Yerel PDF, JPG/PNG ve WhatsApp görüntüleri paylaşılmaz, değiştirilmez. Yalnızca açık dosya yollarını stage et, staged içeriği incele; toplu ekleme veya force push kullanma. SDK, gerçek Unity projesi, build çıktısı ve kişisel belge bu kaynak teslimine eklenmez.
