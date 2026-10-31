# Son Bir Umut — GAMEUP Türkiye 2026

Unity 6.3 LTS / C# ile kırsal yaşam, kuraklık ve dayanıklılık temalı küçük 3D izometrik demo. Hedef Windows'ta 3–5 dakikalık bir deneyimdir; süre ve yeni sürümün oynanışı henüz Unity'de ölçülmedi.

**Geliştirme dalı: `feature/complete-demo`.** `main` önceki tek enerji istasyonlu prototipte kalır; otomatik merge yapılmaz. Hakan'ın önceki sürümde harita, kapsül, kamera ve HUD'ı gördüğü kullanıcı tarafından aktarıldı. Bu dalın hareket ve tam oyun döngüsü Unity onayı bekler. Mac geliştiricisinin Unity Editor'ı yoktur.

## Bu dalda hazırlananlar

- CharacterController korunarak hızlanma, frenleme ve yumuşak Y dönüşü; kamera yönüne göre WASD ve çapraz hız sınırı.
- Tek primitive köy; enerji → su → tarım sırasıyla üç nokta ve her birinde üç kart.
- Merkezi kaynak hesabı, enerjiye bağlı pompa ve suya bağlı bitki desteği; bütçe/enerji/su yetersizliğinde açıklamalı engel.
- Her aşamada ücretsiz seçenek; aynı kararın tekrar uygulanmasını önleyen oturum sistemi.
- Kısa ve merkezi Türkçe hikâye, Enter ile başlangıç, net etkileri gösteren IMGUI kartları ve HUD.
- Panel/jeneratör, su seviyesi, vana/pompa, tarla ve ağaç renkleriyle dünya geri bildirimi.
- Üç koşullu final, karar özeti, köy geneline yumuşak açılan kamera ve yeniden oynama.

Kaynaklar boyutsuz örnek oyun puanlarıdır. Emisyon anlık köy sıcaklığı yaratmaz. Babanın rahatsızlığına teşhis, iklim kaynaklı neden veya kararlarla otomatik iyileşme atfedilmez. Crafting, çöp toplama, NPC görevleri, sinematik ve karmaşık animasyon eklenmedi.

## Hakan / Enes için hızlı kurulum

1. Depoda `feature/complete-demo` dalını al; test projesindeki çalışmanı önce koru.
2. Unity **6.3 LTS**, **Universal 3D (URP)** veya **3D Built-in** projesi kullan. Kaynak klasörü doğrudan Unity projesi değildir.
3. `UnityAssets/SonBirUmutDemo` içeriğini mevcut `Assets/SonBirUmutDemo` üzerine **klasörü silmeden** kopyala. Eski `.meta`, `Scenes` ve `Generated` dosyalarını koru.
4. Active Input Handling: **Both** veya **Input Manager (Old)**. IMGUI fare girdisi nedeniyle New-only desteklenmez; gerekirse Editor'ı yeniden başlat.
5. Console derlemesi tamamlanınca **Tools > GameUP > Create Demo Scene**. Yeni sahne `Assets/SonBirUmutDemo/Scenes/SonBirUmutCompleteDemo.unity` veya numaralı benzersiz yola kaydedilir; eski sahneler ezilmez.
6. Game görünümünü 16:9 aç, Play, Enter. WASD ile dolaş; sıradaki noktada E, kartta Seç. Üç karardan sonra finali ve Yeniden Oyna'yı dene.

[Windows kurulum rehberi](docs/INTEGRATION.md) · [Unity test listesi](UNITY_TEST_CHECKLIST.md) · [Hareket değişikliği](docs/MOVEMENT.md) · [Mimari](docs/ARCHITECTURE.md)

## Otomatik testler

Mevcut .NET 8 veya 9 SDK ile:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md
```

Harici test paketi gerekmez. **39 test grubu geçti, 0 başarısız.** 27 dizinin 25'i doğrudan tamamlandı; 2 sulama dizisi su yetersizliği nedeniyle doğru biçimde engellendi ve ücretsiz alternatifle final doğrulandı. Üç final kategorisine ulaşıldı. Unity hareketi, fizik, UI ve Editor derlemesi bu testlerin kapsamında değildir.

[Karar kuralları](docs/DECISIONS.md) · [27 dizi raporu](docs/DECISION_MATRIX.md) · [Doğrulama kaydı ve açık kontroller](docs/TEST_RESULTS.md)

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

Eski sahne dosyaları değiştirilmez. Serileştirilmiş `player` / `station` referansları korunur; eski sahne tek istasyon modunda kalır. Bu dalda hareket ve ücretsiz tasarruf dengesi günceldir. Birebir önceki davranış için `main` kaynaklarını ayrı test projesinde kullan; dal değiştirmek tek başına dış Unity projesindeki kopyayı güncellemez.

[GitHub](https://github.com/omrumcerenguler/gameup-son-bir-umut-demo) herkese açıktır. Yerel PDF, JPG/PNG ve WhatsApp görüntüleri paylaşılmaz, değiştirilmez. Yalnızca açık dosya yollarını stage et, staged içeriği incele; toplu ekleme veya force push kullanma. SDK, gerçek Unity projesi, build çıktısı ve kişisel belge bu kaynak teslimine eklenmez.
