# 27 karar dizisi — saf C# test raporu

Unity oynanış testi değildir. Değer sırası: bütçe / enerji / su / bitki / emisyon.

| Enerji | Su | Tarım | Sonuç | Son değerler |
| --- | --- | --- | --- | --- |
| solar | pump | protect | Yeniden Doğuş | 5 / 55 / 66 / 59 / 0 |
| solar | pump | irrigate | Yeniden Doğuş | 10 / 55 / 46 / 73 / 0 |
| solar | pump | store | Eksik Dönüşüm | 15 / 55 / 72 / 32 / 0 |
| solar | efficiency | protect | Yeniden Doğuş | 20 / 55 / 42 / 59 / 0 |
| solar | efficiency | irrigate | Eksik Dönüşüm | 25 / 55 / 22 / 61 / 0 |
| solar | efficiency | store | Eksik Dönüşüm | 30 / 55 / 44 / 32 / 0 |
| solar | continue | protect | Kaybedilen Gelecek | 30 / 55 / 11 / 22 / 0 |
| solar | continue | irrigate | Engelli: Su yetersiz: en az 20 gerekli. Depolama ile final mümkün. | — |
| solar | continue | store | Kaybedilen Gelecek | 40 / 55 / 17 / 8 / 0 |
| diesel | pump | protect | Eksik Dönüşüm | 40 / 70 / 63 / 59 / 16 |
| diesel | pump | irrigate | Eksik Dönüşüm | 45 / 70 / 43 / 73 / 18 |
| diesel | pump | store | Eksik Dönüşüm | 50 / 70 / 69 / 32 / 18 |
| diesel | efficiency | protect | Eksik Dönüşüm | 55 / 70 / 39 / 58 / 16 |
| diesel | efficiency | irrigate | Eksik Dönüşüm | 60 / 70 / 19 / 61 / 18 |
| diesel | efficiency | store | Eksik Dönüşüm | 65 / 70 / 41 / 32 / 18 |
| diesel | continue | protect | Kaybedilen Gelecek | 65 / 70 / 8 / 21 / 16 |
| diesel | continue | irrigate | Engelli: Su yetersiz: en az 20 gerekli. Depolama ile final mümkün. | — |
| diesel | continue | store | Kaybedilen Gelecek | 75 / 70 / 14 / 8 / 18 |
| conservation | pump | protect | Yeniden Doğuş | 60 / 15 / 54 / 71 / 0 |
| conservation | pump | irrigate | Eksik Dönüşüm | 65 / 15 / 34 / 73 / 0 |
| conservation | pump | store | Eksik Dönüşüm | 70 / 15 / 60 / 44 / 0 |
| conservation | efficiency | protect | Yeniden Doğuş | 80 / 15 / 50 / 71 / 0 |
| conservation | efficiency | irrigate | Eksik Dönüşüm | 85 / 15 / 30 / 73 / 0 |
| conservation | efficiency | store | Eksik Dönüşüm | 90 / 15 / 52 / 44 / 0 |
| conservation | continue | protect | Eksik Dönüşüm | 90 / 15 / 19 / 38 / 0 |
| conservation | continue | irrigate | Eksik Dönüşüm | 95 / 15 / 0 / 52 / 0 |
| conservation | continue | store | Eksik Dönüşüm | 100 / 15 / 25 / 20 / 0 |

Bu dalda güneş emisyon etkisi 0; mevcut kanallarla sulama bütçe maliyeti 5. Hem koruma hem sulama ile Yeniden Doğuş mümkündür.

25 dizi doğrudan finale ulaştı. Su yetersizliği nedeniyle engellenen 2 sulama dizisinde ücretsiz depolama seçilerek ilerleme doğrulandı. Üç final kategorisine de ulaşıldı.
