# Kriz Nöbeti — tam oturum denge simülasyonu

Saf C# simülasyonudur; Unity fizik/erişim/girdi/gerçek deneyim süresi testi değildir.

Kararlar aktif saniye 35/70/105; ilk bakım 110; arıza 117; su noktasında basılı onarım 144–146; su bakımı 168; tarım bakımı 175. Dolaşma süreleri model girdisidir, Unity'de ölçülmedi. Kart okuma sayaçtan hariçtir. Engelli stratejiye ücretsiz geçerli alternatif uygulanır.

| İstenen enerji / su / tarım | Uygulanan seçimler | Bitiş aktif sn | Baskı | Bakım | Arıza | Kriz başarısı | Genel final | Bütçe / enerji / su / bitki / emisyon |
| --- | --- | ---: | ---: | ---: | --- | --- | --- | --- |
| solar / pump / protect | solar / pump / protect | 178,00 | 16,73 | 3/3 | Repaired | Evet | Yeniden Doğuş | 5 / 55 / 70 / 63 / 0 |
| solar / pump / irrigate | solar / pump / irrigate | 178,00 | 17,00 | 3/3 | Repaired | Evet | Yeniden Doğuş | 10 / 55 / 50 / 77 / 0 |
| solar / pump / store | solar / pump / store | 178,00 | 17,14 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 15 / 55 / 76 / 36 / 0 |
| solar / efficiency / protect | solar / efficiency / protect | 178,00 | 17,82 | 3/3 | Repaired | Evet | Yeniden Doğuş | 20 / 55 / 46 / 63 / 0 |
| solar / efficiency / irrigate | solar / efficiency / irrigate | 178,00 | 18,36 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 25 / 55 / 26 / 65 / 0 |
| solar / efficiency / store | solar / efficiency / store | 178,00 | 18,35 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 30 / 55 / 48 / 36 / 0 |
| solar / continue / protect | solar / continue / protect | 178,00 | 20,04 | 3/3 | Repaired | Evet | Kaybedilen Gelecek | 30 / 55 / 15 / 26 / 0 |
| solar / continue / irrigate | solar / continue / store | 178,00 | 20,17 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 40 / 55 / 21 / 12 / 0 |
| solar / continue / store | solar / continue / store | 178,00 | 20,17 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 40 / 55 / 21 / 12 / 0 |
| diesel / pump / protect | diesel / pump / protect | 178,00 | 16,47 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 40 / 70 / 67 / 63 / 16 |
| diesel / pump / irrigate | diesel / pump / irrigate | 178,00 | 16,75 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 45 / 70 / 47 / 77 / 18 |
| diesel / pump / store | diesel / pump / store | 178,00 | 16,89 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 50 / 70 / 73 / 36 / 18 |
| diesel / efficiency / protect | diesel / efficiency / protect | 178,00 | 17,58 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 55 / 70 / 43 / 62 / 16 |
| diesel / efficiency / irrigate | diesel / efficiency / irrigate | 178,00 | 18,10 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 60 / 70 / 23 / 65 / 18 |
| diesel / efficiency / store | diesel / efficiency / store | 178,00 | 18,09 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 65 / 70 / 45 / 36 / 18 |
| diesel / continue / protect | diesel / continue / protect | 178,00 | 19,80 | 3/3 | Repaired | Evet | Kaybedilen Gelecek | 65 / 70 / 12 / 25 / 16 |
| diesel / continue / irrigate | diesel / continue / store | 178,00 | 19,91 | 3/3 | Repaired | Evet | Kaybedilen Gelecek | 75 / 70 / 18 / 12 / 18 |
| diesel / continue / store | diesel / continue / store | 178,00 | 19,91 | 3/3 | Repaired | Evet | Kaybedilen Gelecek | 75 / 70 / 18 / 12 / 18 |
| conservation / pump / protect | conservation / pump / protect | 178,00 | 17,76 | 3/3 | Repaired | Evet | Yeniden Doğuş | 60 / 15 / 58 / 75 / 0 |
| conservation / pump / irrigate | conservation / pump / irrigate | 178,00 | 18,30 | 3/3 | Repaired | Evet | Yeniden Doğuş | 65 / 15 / 38 / 77 / 0 |
| conservation / pump / store | conservation / pump / store | 178,00 | 18,18 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 70 / 15 / 64 / 48 / 0 |
| conservation / efficiency / protect | conservation / efficiency / protect | 178,00 | 17,99 | 3/3 | Repaired | Evet | Yeniden Doğuş | 80 / 15 / 54 / 75 / 0 |
| conservation / efficiency / irrigate | conservation / efficiency / irrigate | 178,00 | 18,53 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 85 / 15 / 34 / 77 / 0 |
| conservation / efficiency / store | conservation / efficiency / store | 178,00 | 18,52 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 90 / 15 / 56 / 48 / 0 |
| conservation / continue / protect | conservation / continue / protect | 178,00 | 20,12 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 90 / 15 / 23 / 42 / 0 |
| conservation / continue / irrigate | conservation / continue / irrigate | 178,00 | 20,37 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 95 / 15 / 4 / 56 / 0 |
| conservation / continue / store | conservation / continue / store | 178,00 | 20,34 | 3/3 | Repaired | Evet | Eksik Dönüşüm | 100 / 15 / 29 / 24 / 0 |

27/27 kriz başarısı; 2 sulama dizisinde geçerli alternatif; her enerji seçimiyle başarı; genel sonuçlarda üç kategori de mevcut. İyi güneş/verimlilik/koruma yolu Yeniden Doğuş, dizel/pompa/sulama yolu kriz başarısına rağmen Eksik Dönüşüm. Kötü dizel/mevcut/depolama + yalnız enerji bakımı + kaçırılan arıza: 240 sn'de Kaybedilen Gelecek.

Eksik müdahale örneği: aktif süre 240,00 sn, baskı 80,82, bakım 1/3, arıza Expired, kaynaklar 75/70/8/8/18.

## Denge hesabı

Başlangıç 60. Üç bakım toplam -40, tamir -18. Varsayılan artış/sn tüm kaynak aralığında en çok 0.13 (yapılandırılmış genel tavan 0.25). 185 sn için kaba üst sınır 60 + 185×0.13 − 58 = 26.05 < 35. Kaynaklar/baskı sınırları gerçek simülasyonda her adımda uygulanır. Tek bakım + tamir en az iki görev koşulunu karşılamaz. Arıza değerlendirilmeden istikrar kazanılmaz. Eşiğin altında 10 kesintisiz aktif saniye gerekir.
