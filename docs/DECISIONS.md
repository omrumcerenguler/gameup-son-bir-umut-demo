# Karar ve kaynak kuralları

Bu sayfadaki tüm sayılar boyutsuz oyun dengeleme puanlarıdır. Gerçek bilimsel ölçüm veya sıcaklık simülasyonu değildir.

## Akış ve güvence

`GameSession` yalnızca sıradaki kayıtlı noktayı kabul eder: enerji → su → tarım. `GetPreview` ve `TryChoose` aynı `SimulationRules.Preview` hesabını kullanır. Yetersiz bütçe, enerji veya su; sıra dışı nokta ve tekrar seçim durumu değiştirmez. Her kabul edilen karar tam bir turdur; bekleme kaynak tüketmez. Geçmişte önceki ve sonraki değiştirilemeyen durumlar tutulur.

Başlangıç: **100 bütçe / 20 enerji / 40 su / 55 bitki / 0 emisyon**. Enerji tasarrufu artık ücretsizdir; başlangıç bütçesi sıfır olduğunda da ilerleme sağlar. Bu, önceki prototipin 10 puanlık tasarruf maliyetinden bilinçli bir dengeleme değişikliğidir. Önceki prototip `main` dalında korunur.

## Enerji

- Güneş: 60 bütçe, +35 enerji, 0 emisyon etkisi.
- Dizel: 25 bütçe, +50 enerji, -3 su, +18 emisyon etkisi.
- Tasarruf: 0 bütçe, -5 enerji, +8 su.

## Su

- Arıtma/pompa: 25 bütçe, en az 10 enerji. Enerji 25'in altındaysa maliyet +5. Kazanım `min(50, enerji × 2)` sudur. Enerji kapasitesi tüketilmez; sistemin bu turdaki üretim sınırıdır.
- Verimlilik: 10 bütçe, kaçakların onarılmasıyla +18 kullanılabilir su. Bu tur ve sonraki tarım turunda olağan su kullanımı 8 yerine 4 olur.
- Mevcut düzen: ücretsiz, kaçaklardan -5 su; olağan tur tüketimi devam eder.

## Tarım ve doğal alan

- Doğal alanları koru: 10 bütçe. Bitki desteği `min(22, mevcut su / 2)`; tam sayı bölme kullanılır. Emisyon etki göstergesi -2.
- Sulama: 5 bütçe ve en az 20 su; -20 su. Bitki desteği `min(36, mevcut su)`.
- Depola: ücretsiz; su üretmez, bu turun tüketimini en fazla 2 ile sınırlar. Ertelenen sulama için bitki desteği -5. Su sıfırken de seçilebilir.

Önce su etkisi ve tur tüketimi uygulanır; su 0–100'e sınırlandırılır. Kalan su ≥35 ise bitki değişimine +4, aksi halde -8 eklenir; dışsal kuraklık baskısı 6 çıkarılır. Bitki ve diğer göstergeler 0–100, bütçe 0–999 sınırındadır. Emisyon bu bitki hesabına veya anlık sıcaklığa bağlanmaz.

## Seçim ve çevre ayrımı

Güneşin önceki +1 emisyonu doğrudan seçenek tanımıydı; genel emisyon artışı değildi. Bu prototipte yaşam döngüsü emisyon modeli bulunmadığından +1 kaldırıldı. İlk güneş seçimi şimdi **40 / 55 / 32 / 41 / 0** verir.

Bitki 55 → 41 hesabı değişmedi: seçim desteği **0**, su koşulu **-8**, dışsal kuraklık **-6**. Su 40 → 32 kaybı ise olağan **8 tur su kullanımıdır**. Güneş doğrudan bitki hasarı vermez. Kart ve bildirim bu bileşenleri ayrı gösterir.

`TurnBreakdown` seçim enerji/su/bitki/emisyon etkisini, su kullanımını, suya bağlı bitki etkisini, kuraklığı ve 0–100 sınır düzeltmelerini tutar. Bileşenler sınırlar öncesidir; net etki gerçek önceki/sonraki durum farkıdır. Sıfır bitki sağlığında -6 kuraklık bileşeni daha fazla negatif sağlık oluşturmaz.

Sulamanın maliyeti 15 → 5 yapıldı: mevcut kanalların kullanılması ucuzdur; bedeli yine **20 su** ve daha az rezervdir. Güneş → pompa → sulama, **10 / 55 / 46 / 73 / 0** ile Yeniden Doğuş verir. Koruma yolu daha fazla su, sulama yolu daha fazla bitki ve bütçe bırakır. Varsayılan 27 dizide beş Yeniden Doğuş yolu vardır; tek zorunlu kombinasyon yoktur.

## Final

Tek bir toplam puan yoktur:

- **Yeniden Doğuş:** su ≥35, bitki ≥55, enerji ≥15, emisyon ≤12 ve bir tampon (bütçe ≥10 veya su ≥50).
- **Kaybedilen Gelecek:** su <20 ve bitki <30; veya bitki <15 ve enerji <20; veya su <10 ve bütçe <10.
- **Eksik Dönüşüm:** diğer durumlar.

Final eşikleri korunur. Ekran; su stokunu, tarım/doğanın ortak bitki göstergesini, öne çıkan iki kararı ve süren riskleri açıklar. Su kararı her zaman öne çıkarılır; enerji/emisyon riski varsa enerji kararı, diğer durumlarda tarım kararı ikinci olur. Bu anlatım önceliğidir, yeni bir puan sistemi değildir. Su kalitesi veya bağımsız ürün verimi modellenmez.

## Otomatik doğrulama

Mevcut .NET SDK ile:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md
```

**54 test grubu geçti, 0 başarısız.** Mevcut 39 test grubu korundu; ayrıştırma, açıklama ve alternatif başarı yolları için 15 grup eklendi. 27 temel diziden 25'i doğrudan tamamlandı. Güneş/dizel → mevcut su düzeni → sulama dizilerinde su 20'nin altında kaldığından sulama doğru biçimde engellendi; ücretsiz depolama ile final mümkün olduğu doğrulandı. Üç final kategorisine ulaşıldı. Tüm aşamalarda sıfır bütçe/kaynakla da en az bir seçenek kaldığı ayrıca test edildi.

[Üretilen 27 dizi tablosu](DECISION_MATRIX.md) yalnızca saf C# doğrulamasıdır; Unity oynanış testi değildir.
