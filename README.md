# Son Bir Umut: Kuraklıktan Önce — GAMEUP Türkiye 2026

Unity 6.3 LTS / C# ile kırsal yaşam, kuraklık ve dayanıklılık temalı küçük 3D izometrik demo. Hedef Windows'ta 3–5 dakikalık bir deneyimdir; süre ve yeni sürümün oynanışı henüz Unity'de ölçülmedi.

**Geliştirme dalı: `feature/polish-backup-demo`.** Çalışan üç kararlı `feature/complete-demo` temel alındı; bu kaynak dal ve `main` değiştirilmez. Kullanıcının aktarımına göre Hakan önceki dalda üç kararı, dünya değişimlerini ve Eksik Dönüşüm finalini Unity'de oynadı. Bu polish sürümü Unity onayı bekler; Mac'te Editor yoktur.

## Korunan demo ve bu aşamanın iyileştirmeleri

- CharacterController korunarak hızlanma, frenleme ve yumuşak Y dönüşü; kamera yönüne göre WASD ve çapraz hız sınırı.
- Tek primitive köy; enerji → su → tarım sırasıyla üç nokta ve her birinde üç kart.
- Merkezi kaynak hesabı, enerjiye bağlı pompa ve suya bağlı bitki desteği; bütçe/enerji/su yetersizliğinde açıklamalı engel.
- Her aşamada ücretsiz seçenek; aynı kararın tekrar uygulanmasını önleyen oturum sistemi.
- Kısa köye dönüş / yaklaşan kuraklık hikâyesi; sürekli ana hedef, ilerleme, sıradaki nokta ve kritik kaynak uyarısı.
- Kartlarda kazanç, bedel, sonraki karara etki; seçim, tur tüketimi ve kuraklık ayrı gösterilir. Bildirim × veya Esc ile kapatılır, 10 saniye sonra kapanır.
- Panel/jeneratör, su seviyesi, vana/pompa, tarla ve ağaç renkleriyle dünya geri bildirimi.
- Korunan üç final eşiği; su stoku, tarım/doğa, öne çıkan iki karar ve süren risk açıklaması. Mevcut final kamerası ve yeniden oynama korunur.

Kaynaklar boyutsuz örnek oyun puanlarıdır. Emisyon anlık köy sıcaklığı yaratmaz. Hastalık anlatımı kaldırıldı; motivasyon aile ve komşuların kuraklık öncesi temel ihtiyaçlarıdır. Crafting, çöp toplama, NPC görevleri, sinematik ve karmaşık animasyon eklenmedi.

## Hakan / Enes için hızlı kurulum

1. Depoda `feature/polish-backup-demo` dalını al; test projesindeki çalışmanı önce koru.
2. Unity **6.3 LTS**, **Universal 3D (URP)** veya **3D Built-in** projesi kullan. Kaynak klasörü doğrudan Unity projesi değildir.
3. `UnityAssets/SonBirUmutDemo` içeriğini mevcut `Assets/SonBirUmutDemo` üzerine **klasörü silmeden** kopyala. Eski `.meta`, `Scenes` ve `Generated` dosyalarını koru.
4. Active Input Handling: **Both** veya **Input Manager (Old)**. IMGUI fare girdisi nedeniyle New-only desteklenmez; gerekirse Editor'ı yeniden başlat.
5. Mevcut üç noktalı sahneyi kullanabilirsin; yeni sahne gerekirse Console derlemesi tamamlanınca **Tools > GameUP > Create Demo Scene**. Yeni sahne `Assets/SonBirUmutDemo/Scenes/SonBirUmutCompleteDemo.unity` veya numaralı benzersiz yola kaydedilir; eski sahneler ezilmez.
6. Game görünümünü 16:9 aç, Play, Enter. WASD ile dolaş; sıradaki noktada E, kartta Seç. Üç karardan sonra finali ve Yeniden Oyna'yı dene.

[Windows kurulum rehberi](docs/INTEGRATION.md) · [Unity test listesi](UNITY_TEST_CHECKLIST.md) · [Hareket değişikliği](docs/MOVEMENT.md) · [Mimari](docs/ARCHITECTURE.md)

## Otomatik testler

Mevcut .NET 8 veya 9 SDK ile:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md
```

Harici test paketi gerekmez. **54 test grubu geçti, 0 başarısız.** Mevcut 39 grup korundu; 15 açıklama/ayrıştırma testi eklendi. 27 dizinin 25'i doğrudan tamamlandı; 2 sulama dizisi su yetersizliği nedeniyle doğru biçimde engellendi ve ücretsiz alternatifle final doğrulandı. Üç final kategorisine ulaşıldı. Unity hareketi, fizik, UI ve Editor derlemesi bu testlerin kapsamında değildir.

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

Eski sahne dosyaları değiştirilmez. Serileştirilmiş `player` / `station` referansları korunur; eski sahne tek istasyon modunda kalır. Hareket, kamera, etkileşimler, dünya görünümü ve Scene Builder kaynak daldakiyle aynıdır. Küçük dengeleme farkları: güneş emisyonu +1 → 0; mevcut kanallarla sulama bütçesi 15 → 5. Yeni 3D model eklenmedi. Birebir önceki davranış için `main` kaynaklarını ayrı test projesinde kullan; dal değiştirmek tek başına dış Unity projesindeki kopyayı güncellemez.

[GitHub](https://github.com/omrumcerenguler/gameup-son-bir-umut-demo) herkese açıktır. Yerel PDF, JPG/PNG ve WhatsApp görüntüleri paylaşılmaz, değiştirilmez. Yalnızca açık dosya yollarını stage et, staged içeriği incele; toplu ekleme veya force push kullanma. SDK, gerçek Unity projesi, build çıktısı ve kişisel belge bu kaynak teslimine eklenmez.
