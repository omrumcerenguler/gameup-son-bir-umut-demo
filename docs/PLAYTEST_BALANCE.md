# Aşama 3 — beş bakım varyasyonu, tam oturum simülasyonu

Unity hareketi/fiziği/okunabilirlik veya gerçek deneyim süresi ölçümü değildir. Kart okuma hariç aktif zaman girdileri kullanılır.

Kararlar 35/70/100 sn; ilk bakım 105; arıza 112; onarım 133–135; su bakımı 145; tarım 165; enerji denetimi 185; isteğe bağlı filtre 190. En az 4 bakım + 1 arıza değerlendirmesi + düşük baskıda 10 aktif sn gerekir. Başlangıç 60, süre 240; toplam bakım azaltımı 48, tamir 18.

| İstenen seçimler | Uygulanan seçimler | Bitiş aktif sn | Baskı | Bakım | Kriz başarısı | Genel final |
| --- | --- | ---: | ---: | ---: | --- | --- |
| solar/pump/protect | solar/pump/protect | 195.00 | 9.89 | 5/5 | Evet | Yeniden Doğuş |
| solar/pump/irrigate | solar/pump/irrigate | 195.00 | 10.25 | 5/5 | Evet | Yeniden Doğuş |
| solar/pump/store | solar/pump/store | 195.00 | 10.43 | 5/5 | Evet | Eksik Dönüşüm |
| solar/efficiency/protect | solar/efficiency/protect | 195.00 | 11.14 | 5/5 | Evet | Yeniden Doğuş |
| solar/efficiency/irrigate | solar/efficiency/irrigate | 195.00 | 11.84 | 5/5 | Evet | Eksik Dönüşüm |
| solar/efficiency/store | solar/efficiency/store | 195.00 | 11.83 | 5/5 | Evet | Eksik Dönüşüm |
| solar/continue/protect | solar/continue/protect | 195.00 | 13.80 | 5/5 | Evet | Kaybedilen Gelecek |
| solar/continue/irrigate | solar/continue/store | 195.00 | 13.97 | 5/5 | Evet | Eksik Dönüşüm |
| solar/continue/store | solar/continue/store | 195.00 | 13.97 | 5/5 | Evet | Eksik Dönüşüm |
| diesel/pump/protect | diesel/pump/protect | 195.00 | 9.60 | 5/5 | Evet | Eksik Dönüşüm |
| diesel/pump/irrigate | diesel/pump/irrigate | 195.00 | 9.96 | 5/5 | Evet | Eksik Dönüşüm |
| diesel/pump/store | diesel/pump/store | 195.00 | 10.14 | 5/5 | Evet | Eksik Dönüşüm |
| diesel/efficiency/protect | diesel/efficiency/protect | 195.00 | 10.88 | 5/5 | Evet | Eksik Dönüşüm |
| diesel/efficiency/irrigate | diesel/efficiency/irrigate | 195.00 | 11.55 | 5/5 | Evet | Eksik Dönüşüm |
| diesel/efficiency/store | diesel/efficiency/store | 195.00 | 11.54 | 5/5 | Evet | Eksik Dönüşüm |
| diesel/continue/protect | diesel/continue/protect | 195.00 | 13.54 | 5/5 | Evet | Kaybedilen Gelecek |
| diesel/continue/irrigate | diesel/continue/store | 195.00 | 13.69 | 5/5 | Evet | Kaybedilen Gelecek |
| diesel/continue/store | diesel/continue/store | 195.00 | 13.69 | 5/5 | Evet | Kaybedilen Gelecek |
| conservation/pump/protect | conservation/pump/protect | 195.00 | 11.08 | 5/5 | Evet | Yeniden Doğuş |
| conservation/pump/irrigate | conservation/pump/irrigate | 195.00 | 11.78 | 5/5 | Evet | Yeniden Doğuş |
| conservation/pump/store | conservation/pump/store | 195.00 | 11.62 | 5/5 | Evet | Eksik Dönüşüm |
| conservation/efficiency/protect | conservation/efficiency/protect | 195.00 | 11.33 | 5/5 | Evet | Yeniden Doğuş |
| conservation/efficiency/irrigate | conservation/efficiency/irrigate | 195.00 | 12.03 | 5/5 | Evet | Eksik Dönüşüm |
| conservation/efficiency/store | conservation/efficiency/store | 195.00 | 12.02 | 5/5 | Evet | Eksik Dönüşüm |
| conservation/continue/protect | conservation/continue/protect | 195.00 | 13.88 | 5/5 | Evet | Eksik Dönüşüm |
| conservation/continue/irrigate | conservation/continue/irrigate | 195.00 | 14.20 | 5/5 | Evet | Eksik Dönüşüm |
| conservation/continue/store | conservation/continue/store | 195.00 | 14.17 | 5/5 | Evet | Eksik Dönüşüm |

27/27 başarı; 2 engelli sulamada ücretsiz depolama alternatifi. Her enerji seçimiyle kriz başarısı, üç genel final kategorisi korunur. Dizelin emisyonu en iyi genel finali engeller. Eksik müdahale senaryolarında bir bakım ve kaçırılmış arıza, 240 aktif sn'de kısmi veya kötü final verir.

Üst sınır: varsayılan artış en çok 0.13/sn. 195 aktif sn için 60 + 195×0.13 − 48 − 18 = 19.35 < 35. Dört bakım asgari koşulu erken iki görev finalini engeller. Zamanlama modele verilmiştir; yol uzunluğu, çarpışma ve oyuncunun okuma süresi Windows'ta ölçülmelidir.
