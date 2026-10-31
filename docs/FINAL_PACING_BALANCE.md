# Final Pacing — rota ve aktif süre tahmini

**Model bazlı tahmindir; gerçek Unity hareket, fizik, görüş veya oyuncu süresi ölçümü değildir.**

3.5 m/sn yürüyüş, 1.9× sprint; 0.5 m dört yönlü rota grafiği. Ev/tesis/kuyu/bank/gövde sınırları kapsül payıyla genişletilir; görsel havuz ve kanal da kaçınılır. Gerçek collider/navmesh kullanılmaz. Noktalar katalogla ortaktır.

| Profil | Koşulan zaman payı | Yol/hız güvenlik katsayısı | Hedef başına yön bulma | Etkileşim | Acile tepki | Ek gecikme |
|---|---:|---:|---:|---:|---:|---:|
| Hızlı | %85 | 1,10 | 1,5 sn | 0,8 sn | 1,0 sn | 0 sn |
| Yeni oyuncu | %30 | 1,30 | 9,0 sn | 3,0 sn | 5,0 sn | 0 sn |
| Geciken | %10 | 1,50 | 21,0 sn | 5,0 sn | 12,0 sn | 50 sn |

Kart okuma/Intro/menü/pause aktif sürede yoktur; toplam gerçek süre daha uzundur. Yön bulma ve gecikme serbest oyun süresine eklenir. Acil çağrı yolu keser ve su deposuna rota yeniden kurulur; yol/tepki boyunca 45 sn işler, onarım 2 sn gerçek model süresi tüketir.

| Profil | Enerji (su=verimlilik, tarım=koruma) | 54 setten kriz başarısı | Aktif süre min–maks | Mesafe min–maks | Acil yolculuk en çok |
|---|---|---:|---:|---:|---:|
| Hızlı | solar | 54/54 | 87,3–89,7 sn | 114,4–143,4 m | 4,3 sn |
| Hızlı | diesel | 54/54 | 87,3–89,7 sn | 114,4–143,4 m | 4,3 sn |
| Hızlı | conservation | 54/54 | 87,3–89,7 sn | 114,4–143,4 m | 4,3 sn |
| Yeni oyuncu | solar | 54/54 | 154,9–175,2 sn | 143,7–185,3 m | 8,9 sn |
| Yeni oyuncu | diesel | 54/54 | 154,9–175,2 sn | 143,7–185,3 m | 8,9 sn |
| Yeni oyuncu | conservation | 54/54 | 154,9–175,2 sn | 143,7–185,3 m | 8,9 sn |
| Geciken | solar | 0/54 | 240,0–240,0 sn | 81,4–86,9 m | 0,0 sn |
| Geciken | diesel | 0/54 | 240,0–240,0 sn | 81,4–86,9 m | 0,0 sn |
| Geciken | conservation | 0/54 | 240,0–240,0 sn | 81,4–86,9 m | 0,0 sn |

## Seed 0 örnekleri

| Profil | Aktif sn | Bakım | Arıza | Baskı | Genel final | Rota |
|---|---:|---:|---|---:|---|---|
| Hızlı | 87,3 | 4/6 | Repaired | 25,4 | Yeniden Doğuş | node-irrigation-valve → node-distribution-box → ARIZA → node-soil-check → node-solar-link |
| Yeni oyuncu | 158,0 | 4/6 | Repaired | 31,7 | Yeniden Doğuş | node-irrigation-valve → ARIZA → node-distribution-box → node-soil-check → node-solar-link → node-irrigation-valve → node-filter-clean |
| Geciken | 240,0 | 2/6 | Expired | 77,2 | Eksik Dönüşüm | ARIZA → node-irrigation-valve → node-soil-check → node-solar-link |

## 27 strateji — seed 0, yeni oyuncu

| İstenen seçimler | Uygulanan tarım | Aktif sn | Bakım | Kriz başarısı | Genel final |
|---|---|---:|---:|---|---|
| solar/pump/protect | protect | 158,0 | 4/6 | Evet | Yeniden Doğuş |
| solar/pump/irrigate | irrigate | 158,0 | 4/6 | Evet | Yeniden Doğuş |
| solar/pump/store | store | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| solar/efficiency/protect | protect | 158,0 | 4/6 | Evet | Yeniden Doğuş |
| solar/efficiency/irrigate | irrigate | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| solar/efficiency/store | store | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| solar/continue/protect | protect | 158,0 | 4/6 | Evet | Kaybedilen Gelecek |
| solar/continue/irrigate | store | 158,0 | 4/6 | Evet | Kaybedilen Gelecek |
| solar/continue/store | store | 158,0 | 4/6 | Evet | Kaybedilen Gelecek |
| diesel/pump/protect | protect | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| diesel/pump/irrigate | irrigate | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| diesel/pump/store | store | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| diesel/efficiency/protect | protect | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| diesel/efficiency/irrigate | irrigate | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| diesel/efficiency/store | store | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| diesel/continue/protect | protect | 158,0 | 4/6 | Evet | Kaybedilen Gelecek |
| diesel/continue/irrigate | store | 158,0 | 4/6 | Evet | Kaybedilen Gelecek |
| diesel/continue/store | store | 158,0 | 4/6 | Evet | Kaybedilen Gelecek |
| conservation/pump/protect | protect | 158,0 | 4/6 | Evet | Yeniden Doğuş |
| conservation/pump/irrigate | irrigate | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| conservation/pump/store | store | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| conservation/efficiency/protect | protect | 158,0 | 4/6 | Evet | Yeniden Doğuş |
| conservation/efficiency/irrigate | irrigate | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| conservation/efficiency/store | store | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| conservation/continue/protect | protect | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| conservation/continue/irrigate | irrigate | 158,0 | 4/6 | Evet | Eksik Dönüşüm |
| conservation/continue/store | store | 158,0 | 4/6 | Evet | Eksik Dönüşüm |

Yetersiz su yüzünden engellenen iki sulama isteğinde mevcut ücretsiz depolama alternatifi kullanılır. Bakım bütçe/enerji/emisyonu silmez; kaynakları kötü bırakılmış stratejiler kriz yönetimi başarılı olsa da iyi genel final garantilemez.

Denge: altı işten en az dört, arıza değerlendirilmiş, baskı ≤35 ve 10 aktif sn istikrar. Her iş -6 baskı; iki su işi toplam +4 su, iki doğa işi toplam +4 bitki; bütçe/enerji/emisyon değişmez. Çağrılar kriz başından 0/0/25/55/85/115 sn önce açılmaz; boş aktif yer ve tamamlanma sırası da gerekir. İşlerin ayrı son tarihi yoktur.

[Windows fizik/oynanış kabul listesi](FINAL_PACING_UNITY_TEST.md). Haritada gerçek dolaşma, yön bulma, ses ve erişim Hakan tarafından ölçülmelidir. Bu tablo keyfî Inspector değerleri için garanti değildir.
