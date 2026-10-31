# Son Bir Umut — Unity test kontrol listesi

**Durum: Yeni polish sürümü için aşağıdaki Unity kontrolleri henüz yapılmadı.** Mac üzerinde Unity Editor bulunmaz. Kutuları Hakan veya Enes yalnızca testi gerçekten çalıştırınca işaretlemelidir. Saf C# testlerinin geçmesi bu kutuları tamamlamaz.

| Test kaydı | Değer |
| --- | --- |
| Test eden / tarih | |
| Branch / commit | feature/polish-backup-demo / |
| Unity tam sürümü (6000.3.x) | |
| Windows sürümü / pipeline | |
| Input Handling | Both veya Old / |
| FPS / Game çözünürlüğü | |
| Hız / hızlanma / fren / dönüş | 3.5 / 12 / 16 / 600 başlangıç; test edilen: |
| İlk kez oynanış süresi | 3–5 dk hedef; ölçülen: |
| İlk Console hatası / adım | |

## Import ve sahne güvenliği

- [ ] Mevcut Assets/SonBirUmutDemo silinmeden kaynaklar güncellendi; eski .meta, Scenes ve Generated korundu.
- [ ] Unity 6.3 LTS script derlemesi tamamlandı; Console'da hata / Missing Script yok.
- [ ] UnityEditor kodları build'e sızmıyor; Editor klasöründe.
- [ ] Tools > GameUP > Create Demo Scene üç noktayı ve bağlı referansları kuruyor.
- [ ] SonBirUmutCompleteDemo.unity yeni dosya; eski enerji sahnesi değiştirilmedi.
- [ ] Aracı ikinci kez çalıştırınca ayrı numaralı sahne ve materyal klasörü oluşuyor.
- [ ] Kirli açık sahnede araç uyarıp duruyor; kaydetme, silme veya değişiklik atma yok.
- [ ] Prefab düzenleme / Play modunda araç mevcut içeriğe müdahale etmiyor.
- [ ] Sahneyi kapatıp tekrar açınca materyaller ve referanslar korunuyor; pembe model yok.
- [ ] Eski tek istasyonlu sahnenin referansları ve tek noktalı akışı yeni kaynaklarla ayrıca denendi.

## Öncelik: akıcı hareket

- [ ] CharacterController korunuyor; Rigidbody yok. Kapsül zeminden düşmüyor.
- [ ] W, A, S, D tek başına kamera düzlemine göre doğru yönlere götürüyor.
- [ ] W+A, W+D, S+A, S+D çaprazlarının hepsi çalışıyor.
- [ ] Çapraz hız düz hareketten yüksek değil.
- [ ] W+A basılıyken A bırakılması düzgün yön değiştiriyor.
- [ ] W'den S'ye doğrudan geçişte anlık 180° sıçrama yok.
- [ ] Hızlanma kısa ve doğal; tüm tuşlar bırakılınca kontrollü fren var.
- [ ] Karakter anlamlı hız yönüne Y ekseninde yumuşak dönüyor; yön işareti anlaşılır.
- [ ] 30/60/120 FPS karşılaştırmasında gereksiz kayma, titreşim veya yön tersliği yok.
- [ ] Evler, ağaçlar, depo, kaide ve harita sınırlarının içinden geçilemiyor.
- [ ] Dekoratif karar modellerinin çarpışmasız olması oyuncuyu hapsetmiyor.
- [ ] Alt-tab sonrası geri dönünce kontrol düzgün; eski hız birikmiyor.

## Giriş, kamera ve kartlar

- [ ] Kısa Türkçe giriş görünüyor; Enter veya Başla tek adımda oyunu başlatıyor.
- [ ] Giriş açıkken karakter yürümüyor.
- [ ] Kamera yumuşak takip ediyor; karakter dönünce kamera kendi açısını döndürmüyor.
- [ ] Sıradaki noktaya yaklaşınca E ipucu, uzaklaşınca normal keşif bilgisi görünüyor.
- [ ] E üç kartı açıyor; yatay hareket aynı karede duruyor.
- [ ] Esc/Vazgeç hiçbir kaynağı değiştirmeden kapatıyor; hareket yeniden hızlanıyor.
- [ ] Both modunda klavye ve fare kartları çalışıyor (paket varsa).
- [ ] Old modunda yeni Input System paketi olmadan klavye ve kartlar çalışıyor.
- [ ] New-only modunda araç açıklama gösterip duruyor; eski sahnede açık Console uyarısı var.
- [ ] Kart metinleri 1280×720 ve 1920×1080'de okunaklı; kırpılma yok.

## Polish: hikâye ve kararların anlaşılabilirliği

- [ ] Başlık Son Bir Umut: Kuraklıktan Önce; ana hedef tüm ekranlarda açık.
- [ ] İlerleme, sıradaki nokta ve kritik kaynak durumu sürekli görülüyor.
- [ ] Problem cümlesi ve Kazanç / Bedeli / Sonrası alanları okunuyor.
- [ ] Güneş sonrası 40 / 55 / 32 / 41 / 0; doğrudan bitki ve emisyon etkisi sıfır.
- [ ] Tur su -8, su koşulu bitki -8, kuraklık bitki -6 ayrı; net bitki -14 açık.
- [ ] 0/100 sınırındaki net değişim bileşenlerle karıştırılmıyor.
- [ ] Seçim bildirimi değişen nesneyi kapatmıyor; ×/Esc ile hemen, 10 saniyede otomatik kapanıyor.
- [ ] Bildirim açıkken hareket ve sonraki E etkileşimi çalışıyor; final düğmesini örtmüyor.
- [ ] Güneş → pompa → sulama, 10 / 55 / 46 / 73 / 0 ile Yeniden Doğuş; koruma yolu farklı kaynak dağılımıyla da başarılı.
- [ ] Final stok, tarım, doğal yaşam, iki karar ve süren risk bakımından anlaşılır.
- [ ] Emisyon/enerji riski olan final enerji + su kararını; diğer finaller su + tarım kararını öne çıkarıyor.
- [ ] Final açıklamaları su kalitesi veya bağımsız ürün verimi ölçümü iddiası taşımıyor.
- [ ] Yeni arayüz 1280×720 ve 1920×1080'de kırpılmıyor; yeniden oynama bildirim ve özeti de sıfırlıyor.

## Üç karar ve bağımlılıklar

- [ ] Enerji → su → tarım sırası korunuyor; erken su/tarım kartı açılamıyor.
- [ ] Her noktada üç seçenek ve kısa açıklama var.
- [ ] Maliyetler ve net enerji/su/bitki/emisyon etkileri seçimle birebir eşleşiyor.
- [ ] Yetersiz bütçe, enerji veya su nedeni görünüyor; seçenek uygulanamıyor.
- [ ] Her aşamada en az bir seçenek seçilebilir kalıyor.
- [ ] Güneş sonrası pompa maliyeti 25; tasarruf sonrası 30 ve su üretimi daha düşük.
- [ ] Verimlilik tarım turunun su kullanımını da azaltıyor.
- [ ] Güneş/dizel → mevcut su düzeninde sulama engelli; depolamayla tamamlanıyor.
- [ ] Aynı noktada ikinci seçim ve ikinci maliyet uygulanamıyor.
- [ ] Beklerken kaynaklar değişmiyor; her kabul edilen karar yalnız bir tur ilerletiyor.

## Dünya görünümü

- [ ] Panel / jeneratör / tasarruf işareti yalnız seçilen enerji modelini gösteriyor.
- [ ] Su seviyesi, pompa / vana / mevcut düzen uyarısı seçime ve duruma uyuyor.
- [ ] Doğal alan koruması / sulama kanalları / depolama varilleri doğru açılıyor.
- [ ] Bitki ≥55 yeşil, 30–54 sarı, <30 kuru; tarla rengi de değişiyor.
- [ ] Görseller tek haritada değişiyor; üç ayrı harita veya sıcaklık simülasyonu yok.

## Final ve yeniden oynama

- [ ] Güneş → verimlilik → koruma: 20 / 55 / 42 / 59 / 0, Yeniden Doğuş.
- [ ] Dizel → pompa → sulama: 45 / 70 / 43 / 73 / 18, Eksik Dönüşüm.
- [ ] Güneş → mevcut düzen → depola: 40 / 55 / 17 / 8 / 0, Kaybedilen Gelecek.
- [ ] Final yalnız üçüncü karardan sonra açılıyor; su/tarım/doğa, öne çıkan iki karar ve süren risk okunuyor.
- [ ] Kamera uzaklaşıp yükseliyor; köy sonuç panelinin solunda izlenebiliyor, açı dönmüyor.
- [ ] Final açıkken karakter kontrolü kapalı; tekrar karar uygulanamıyor.
- [ ] Yeniden Oyna; kaynakları 100 / 20 / 40 / 55 / 0, ilerlemeyi 0/3 ve karakteri başlangıca döndürüyor.
- [ ] Panel/jeneratör ve diğer seçim modelleri kapanıyor; su, tarla ve ağaçlar ilk duruma dönüyor.
- [ ] Kamera normal takip ve zoom değerlerine, hız sıfıra dönüyor; giriş yeniden başlatılabiliyor.
- [ ] En az üç ardışık farklı oynanışta önceki seçim veya görsel sızıntısı yok.
- [ ] Giriş kısa köye dönüş ve yaklaşan kuraklık anlatısı; hastalık veya NPC görevi yok.

## Windows teslimi

- [ ] Build Profiles sahne listesinde yeni CompleteDemo başlangıç sahnesi.
- [ ] Windows build hatasız oluşuyor ve EXE açılıyor.
- [ ] EXE içinde WASD/çapraz, E, kart tıklama, üç karar, final ve yeniden oynama tekrar denendi.
- [ ] Oynanış süresi ölçüldü; 3–5 dakika hedefine ilişkin geri bildirim yazıldı.
- [ ] Son Console/oyuncu log kontrolü temiz; başarısız adımlar kayda işlendi.

Başarısız adımda commit, adım, beklenen/görülen davranış ve ilk hata metnini yazın. Bu dosyaya kişisel ekran görüntülerini eklemek yerine gerekirse ekip içinde paylaşın.
