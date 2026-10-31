# Hakan — Aşama 3 gerçek oynanış testi

**Bu yeni sürümdeki bütün kutular bekliyor.** Mac'te Unity Editor veya Windows build çalıştırılmadı. Aşama 2 hakkında kullanıcının aktardığı geri bildirim, bu yeni sürümün doğrulaması değildir.

## Güvenli kurulum

- [ ] Çalışan Unity projesini ve yerel değişiklikleri yedekledim. Eski .meta, Scenes, Generated korunuyor.
- [ ] Temiz kaynak depomda `git fetch origin`, `git switch feature/playtest-ux`, `git pull --ff-only origin feature/playtest-ux` yaptım; commit'i kaydettim. Yedek feature/live-village-demo değiştirilmedi.
- [ ] UnityAssets/SonBirUmutDemo içeriğini dış projenin Assets/SonBirUmutDemo üzerine silmeden kopyaladım. Yeni Simulation/World/Editor dosyaları dahil; eski .meta GUID'leri korunuyor, yeni .meta'yı Editor üretiyor.
- [ ] Unity 6.3 LTS Built-in/URP, Active Input Handling Old veya Both; Console derlemesi tamamlandı, ilk kırmızı hata/Missing Script yok.
- [ ] Play kapalı ve sahneler temizken **Tools > GameUP > Create Playtest Demo Scene**. Yeni Scenes/SonBirUmutPlaytestDemo.unity ve Generated/PlaytestDemo... benzersiz yollar; önceki sahne/materialler ezilmedi.
- [ ] Sahneyi kaydedip kapat/yeniden açtım: marker, feedback, terrain/kanal dizileri, solar lamp, dünya/kamera/controller referansları korunuyor; pembe/kayıp model yok.

## Sade HUD / kartlar — her iki çözünürlükte

- [ ] **1280×720 ve 1920×1080**: ince tek HUD; süre, baskı, bütçe/enerji/su/bitki/emisyon ve ilerleme okunuyor. Merkez oyun dünyasına ayrılmış; sürekli uzun hesap/uyarı yok.
- [ ] En fazla bir kısa hedef satırı; istasyon uzakken altta sürekli kontrol metni yok, menzilde kısa E ipucu var. Ekran dışı hedefte Yön etiketi istasyonu bulduruyor.
- [ ] Üç kart birkaç saniyede anlaşılır: kısa başlık, fayda/bedel, maliyet, tur sonu etkileri, yeterince büyük düğme. Köyün bir kısmı görünür.
- [ ] Engelli kart gerekçesi okunuyor/tıklanamıyor; Etki ayrıntısı isteğe bağlı, scroll ve Esc/Vazgeç çalışıyor. Kart/Intro açıkken hareket/süre duruyor.
- [ ] Bildirimler kısa ve tek; bakım/karar 3 sn, arıza sonucu 4 sn. Arıza uyarısının üstüne başka bildirim yığılmıyor.
- [ ] Finalde beş görev ve gerçek tamamlanma sayısı doğru; bütün satırlara kaydırılabiliyor, Köyü izle/rapora dön/yeniden oyna çalışıyor.

## Harita / hareket

- [ ] 32×28 zeminde enerji (8,1), su (−8,8), tarım (8,−7) bölgeleri ayrılıyor. Yeni yollar, ağaç kümeleri, kanal/rezervuar, çit, kaya ve otlar okunaklı.
- [ ] Üç istasyona iki yönden ulaşabildim; ev/bench/kuyu/ana ağaç/sınır çarpışmaları geçişi kapatmıyor. Su yolu ara noktasını, tarla ön yolunu ve yeni köşeleri özellikle denedim.
- [ ] İlave küçük kenar dekorları çarpışmasız; ana collider'lar ve sınırlar çalışıyor. Zeminden düşme/istasyon içinde doğma/sıkışma yok.
- [ ] Motor: yürüyüş3.5, Sprint Multiplier1.9 (hedef6.65); Shift bırakınca frenleme ve dönüş yumuşak. Dört çapraz ek hız vermiyor.
- [ ] 30/60/120 FPS: kamera sabit izometrik ve yumuşak; giriş/kart/final hareket kilidi; duvarda yerinde yürüyüş veya poz sapması yok.

## Görevler / arıza

- [ ] Üç karar sonrası beş bakım kaydı, ilk iki açık görev (enerji/su). Aynı anda en fazla iki, aynı istasyonda en fazla bir açık iş; tamamlandıkça diğerleri beliriyor.
- [ ] Üç eski görev + tasarruf kontrolü + su filtresi temizliği denendi. İstasyona dönüşte yeni açık varyasyon seçiliyor; eski ID ikinci ödül vermiyor. Su filtresi ve tarım kontrolü seçilmemiş pompa/sulama tesisini varsaymıyor.
- [ ] Görev tamamlanınca işaret/onay değişiyor; başka varyasyon aynı istasyonda açılınca yapılacak iş açık. Kaynak desteği bir kez +4 su/+4 bitkiyi aşmıyor.
- [ ] Kriz başlangıcından 12 aktif sn sonra tek arıza; kırmızı su işareti/yön, kalan **45 saniye** açık. Son12 sn kenar vurgusu/nabız artıyor; bütün görüşü kapatmıyor.
- [ ] Menzilde E held: bar gerçek 2 aktif saniyede doluyor. Bırakma/uzaklaşma/Alt+Tab iptal; dönüşte onarım aniden bitmiyor. Onarımda ana sayaç işler.
- [ ] Arızada E başka görev/karar çalıştırmıyor. Tamir tamamlanan karede bakım ikinci ödül vermiyor; E'yi bırakıp yeniden basmak gerekiyor.
- [ ] Tamirde damlalar durdu, yeşil hat ve kısa başarı mesajı geldi. Bilerek kaçırınca yalnız bir kez −6 su/+8 baskı, kırmızı kayıp izi, görevler devam ediyor; kilitlenme yok.

## Görsel tepki / atmosfer

- [ ] Solar seçilince panel ve çalışma ışığı var. Dizel/tasarrufta solar ışık yok; seçilmeyen tesis çalışır görünmüyor.
- [ ] Su değişince rezervuar/kanal genişliği değişiyor; 0 suda su yüzeyleri yok, kuru yatak var. Bölge rengi dolu su havuzuyla karışmıyor.
- [ ] Bitki durumuna göre taç/dal/ürün/toprak değişiyor; baskı yüksekken hafif solgunluk var. Başarılı müdahalelerde görünür toparlanma; HUD değerleriyle çelişki yok.
- [ ] Düşük baskı açık/sıcak, yüksek baskı uzak çevrede belirgin pus. **Karakter, yakın yol ve etkileşim işareti kaybolmuyor**; koşma ve final kamerasında da denendi.
- [ ] Görsel karşılaştırma için Play dışında Initial Pressure20/60/100 ile ayrı denemeler yaptım; bu ayarların oyun dengesini değiştirdiğini biliyorum. Sonra60'a döndürdüm.
- [ ] Built-in/URP'de fog, Use Basic Fog kapalıyken ışık/ambient alternatifi denendi; fallback'in uzak nesne örtmediği not edildi. Pipeline veya ağır paket gerekmiyor.

## Süre / başarı / tekrar / eski mod

- [ ] 240 aktif sn; giriş/kart/focus/native pause ilerletmiyor; serbest oyun ve arıza sırasında ilerliyor. Alt+Tab dönüşü sıçrama yok; yeni duraklatma menüsü eklenmedi.
- [ ] Yeni modda üç karar + **en az dört bakım** + değerlendirilmiş arıza + ≤35 baskıda10 aktif sn erken final; iki bakım erken bitirmiyor. Beşinci bakım isteğe bağlı.
- [ ] İyi güneş, orta dizel ve kötü/eksik müdahale rotalarını denedim. Kriz başarısı genel sürdürülebilirlik kategorisiyle karışmıyor; dizel otomatik en iyi final değil.
- [ ] 5 sn ayarıyla 0/1/2 kararda süre sonu; eksik görev ve aktif arızayla süre sonu geçerli. Sonra240'a döndüm.
- [ ] İki replay: Intro/240/60,0/5 görev,0/3 karar, baştaki iki görevin tekrar açılması, arıza/onarım/istikrar, renkler/damlalar/solar/onay/kanallar ve UI/kamera sıfırlandı.
- [ ] Eski üç menü ayrı test edildi: Legacy üç karar→final, Foundation süre/baskı, CrisisDemo üç görev/en az iki bakım. Eski sahne referansları korunuyor; bu dalda ortak HUD/atmosferin değiştiği not edildi.

## Gerçek deneyim ve Windows

- [ ] İlk kez oynayan bir kişiyle başlangıç yönlendirmesi, eğlence/hareket hissi ve **gerçek aktif süre + kartlar dahil toplam süre** ölçüldü. 195 sn simülasyonun fizik/okuma ölçümü olmadığı not edildi.
- [ ] Gerekirse yalnız Inspector hız veya onarım/timer ayarları test kaydına yazıldı; teslim değerleri 240/60/35/10/12/45/2 ve Sprint1.9'a döndü.
- [ ] Build Profiles/Scene List'te yeni **PlaytestDemo** başlangıç. Windows build alınıp EXE Editor dışında test edildi; EXE/_Data/UnityPlayer.dll ve diğer gerekli yan dosyalar birlikte korundu.

| Kayıt | Değer |
| --- | --- |
| Test eden / tarih / branch / tam commit | |
| Unity patch / Windows / pipeline / input | |
| Sahne / çözünürlük / FPS | |
| Strateji / görev sırası / arıza sonucu | |
| Aktif süre / toplam süre | |
| Baskı / görev sayısı / kriz başarısı / genel final | |
| İlk Console/Player.log hatası ve adımı | |
| HUD/metin/atmosfer/erişim geri bildirimi | |
