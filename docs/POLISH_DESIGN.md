# Aşama 1 — hikâye ve kararların anlaşılabilirliği

Bu belge Aşama 1 kaydıdır. Sonraki görsel değişiklikler ve değişen dosyalar [Aşama 2 raporunda](VISUAL_STAGE2.md) açıklanır. Aşağıdaki “korunan dosyalar” ifadesi Aşama 1 için geçerlidir.

## Korunan temel

Çalışan `feature/complete-demo` tabanı kullanılır. Geliştirme `feature/polish-backup-demo` dalındadır; `main` ve kaynak dal değiştirilmez, merge yapılmaz. Üç etkileşim, CharacterController, kamera, giriş adaptörü, dünya görünümü ve Scene Builder dosyaları korunur. Yeni 3D model yoktur. Final sınıflandırmasının eşikleri de aynıdır.

## Hikâye ve sürekli hedef

Kimlik: **Son Bir Umut: Kuraklıktan Önce**. Oyuncu çocukluk köyüne yaklaşan ağır kuraklık öncesi döner. Aile ve komşuların su, tarım ve doğal yaşam ihtiyaçları için üç karar verir. Hastalık, NPC araştırması veya ayrı hikâye görevi yoktur.

Ana hedef: **Kuraklık gelmeden köyün temel ihtiyaçlarını koru.** `DemoStory` giriş/hedef/problem metinlerini merkezde tutar. HUD hedef, tamamlanma, sıradaki nokta ve kritik kaynak uyarısını giriş, kart ve final sırasında da gösterir.

## Güneş sonrası değişimin gerçek nedeni

Kaynak sürümün hesabı:

| Bileşen | Kaynak / bitki etkisi |
| --- | --- |
| Güneş seçimi | Bütçe -60, enerji +35, doğrudan su 0, doğrudan bitki 0, emisyon +1 |
| Olağan karar turu | Su kullanımı -8; su 40 → 32 |
| Su koşulu | Kalan su 35'in altında olduğu için bitki -8 |
| Dışsal kuraklık | Bitki -6 |
| Net | Bitki 55 → 41; emisyon 0 → 1 |

Emisyon +1 **genel sistem olayı değildi**; `DecisionCatalog` güneş satırında açıkça tanımlıydı. Kaynakta yaşam döngüsü/üretim emisyon modeli veya bunu açıklayan bir mekanik yoktu. Bu yüzden prototipte güneşin operasyonel emisyon etkisi **0** yapıldı; gerçek dünyada tüm güneş sistemlerinin sıfır yaşam döngüsü emisyonu olduğu iddia edilmez.

Bitki hesabı korunur: güneş bitkiye hasar vermez; **0 seçim desteği -8 su koşulu -6 kuraklık = -14 net**. İlk güneş sonucu artık **40 bütçe / 55 enerji / 32 su / 41 bitki / 0 emisyon**.

`TurnBreakdown` ayrı kayıt tutar. 0–100 sınırları öncesi planlanan bileşenler ile gerçek önceki/sonraki net fark ayrı gösterilir. Örneğin bitki 0'dayken -6 kuraklık bileşeni daha fazla negatif sağlık yaratmaz; sınır düzeltmesi toplamı uzlaştırır. Emisyon kuraklık parametresini değiştirmez; jeneratör yerel sıcaklığı anında artırmaz.

## Kartların ödünleşimleri ve küçük dengeleme

Her seçenekte `Advantage`, `Tradeoff`, `NextEffect`, `Feedback` metinleri vardır. Sayısal kart hesabı hâlâ tek `SimulationRules.Preview` kaynağından gelir. Engelli kartın bütçe/enerji/su nedeni gösterilir. Ücretsiz tasarruf, mevcut su düzeni ve depolama seçenekleri korunur.

Yalnız iki sayısal tanım değişti:

- Güneş emisyon etkisi **+1 → 0**: açıklanmayan teknoloji cezası kaldırıldı.
- Mevcut kanallarla sulama bütçesi **15 → 5**: başlangıç yatırımları sonrasında tarım ağırlıklı başarılı bir rota açıldı. En az 20 su ve 20 su harcaması korunur.

Örnek ödünleşim (güneş → pompa sonrası):

| Son karar | Bütçe | Su | Bitki | Sonuç |
| --- | ---: | ---: | ---: | --- |
| Doğal alanı koru | 5 | 66 | 59 | Yeniden Doğuş |
| Tarımı sula | 10 | 46 | 73 | Yeniden Doğuş |
| Depola | 15 | 72 | 32 | Eksik Dönüşüm |

Koruma daha fazla su, sulama daha fazla bitki ve bütçe, depolama daha fazla stok ve bütçe bırakır. Depolamada sulama ertelendiği için bitki riski kalır; sağlıklı ve bol sulu farklı başlangıçta depolamanın da Yeniden Doğuş verebildiği test edilir. Varsayılan dizilerde beş başarılı rota vardır; tek zorunlu kombinasyon yoktur.

## Seçim bildirimi ve final açıklaması

Bildirim alt soldaki sınırlı şerittedir; sahnenin merkezini ve final panelini örtmemesi hedeflenir. Eylemi, bütçeyi, doğrudan etkiyi, tur/kuraklığı ve net farkı söyler. **× veya Esc** hemen kapatır; 10 saniyede kapanır. Modal değildir, hareketi veya sıradaki E etkileşimini kilitlemez. Unity'de gerçek kadraj/okunabilirlik henüz doğrulanmadı.

`EndingResult` sınıflandırması değiştirilmez. `EndingSummary` mevcut kaynak ve karar ID'lerinden şunları üretir:

- İçme suyu için ayrılabilir stok ve rezerv riski; su kalitesi simüle edilmez.
- Tarımın önceliği ve ortak bitki göstergesi.
- Doğal alan korumasının seçilip seçilmediği ve aynı bitki göstergesi; ayrı doğa/ürün sayacı icat edilmez.
- Su kararı ile risk açısından ilgili ikinci karar. Enerji <15 veya emisyon >12 ise enerji; diğer durumlarda tarım öne çıkarılır. Bu açıklama önceliğidir, matematiksel bir önem puanı değildir.
- Kalan su, bitki, enerji, emisyon veya kaynak tamponu riskleri.

## Doğrulama

**54 saf C# test grubu geçti.** Önceki 39 grup korunur; yeni 15 grup ayrıştırmayı, geri bildirimi, alternatif başarıları, final açıklamalarını ve sıfırlamayı doğrular. 27 dizi tekrar tarandı: 25 doğrudan tamamlandı; 2 su yetersizliği durumu doğru engellendi ve ücretsiz alternatifle tamamlandı. Üç final kategorisi erişilebilir.

[27 dizi](DECISION_MATRIX.md) · [Test kaydı](TEST_RESULTS.md) · [Unity kontrol listesi](../UNITY_TEST_CHECKLIST.md)

Hakan'ın kaynak sürümdeki oynanış doğrulaması bu polish sürümüne taşınmış bir test sayılmaz. Yeni GUI metinleri, bildirim kadrajı/kapanması, tüm final açıklamaları ve Windows build gerçek Editor'da kontrol edilmelidir.
