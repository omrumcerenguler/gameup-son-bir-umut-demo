# Hareket iyileştirmesi

Önceki `DemoPlayerMotor` zaten W+A gibi birleşimleri okuyordu ve CharacterController kullanıyordu. Sertlik, dijital girişin doğrudan tam yürüme hızına çevrilmesinden ve hızlanma/frenleme durumunun bulunmamasından kaynaklanıyordu. Dört yön kısıtı eklenmedi; mevcut kamera yönüne göre XZ hesabı ve CharacterController çarpışmaları korundu.

Hedef yatay hız ile mevcut hız arasında `Vector3.MoveTowards`, bakış yönünde `Quaternion.RotateTowards` kullanılır. Her iki sınır da `deltaTime` ile ölçeklenir. WASD sekiz hedef yön verir; karakterin hızı ve Y ekseni yönelimi bu hedefler arasında sürekli değişir. Analog kontrol iddiası yoktur. Çapraz giriş 1 ile sınırlandırılır.

Yeni oluşturulan oyuncuda Inspector başlangıç değerleri:

| Alan | Değer |
| --- | ---: |
| Speed | 3.5 birim/sn |
| Acceleration | 12 birim/sn² |
| Deceleration | 16 birim/sn² |
| Turn Speed | 600 derece/sn |

Tam hıza yaklaşık 0.29 sn, tam hızdan duruşa yaklaşık 0.22 sn gerekir. Kart açılması normal frenleme yerine yatay hızı hemen sıfırlar. Tam demoda giriş ve final de hareketi kilitler. Yeniden başlama da hız biriktirmez. Yerçekimi ve tek `CharacterController.Move` çağrısı devam eder; Rigidbody eklenmez. Kamera sabit 35.264°/45° izometrik açıyı korur, oyuncunun dönüşünü miras almaz.

Eski kaydedilmiş sahnelerde serileştirilen Speed değeri 4 olarak kalabilir; istenirse Inspector'dan 3.5 yapılmalıdır. Yeni sahne oluşturucu yeni varsayılanları kullanır. Kapsülün mevcut küçük yön işareti korunur.

## Hakan / Enes için manuel kontroller — henüz yapılmadı

- [ ] W, A, S, D tek başına; W+A, W+D, S+A, S+D çaprazları.
- [ ] W+A basılıyken A'yı bırak; tam hız korunurken yön yumuşakça düzelmeli.
- [ ] W'den doğrudan S'ye geç; anlık 180° sıçrama olmamalı.
- [ ] Tüm tuşları bırak; kısa ve kontrollü frenleme, gereksiz kayma olmamalı.
- [ ] E ile kart açarken yatay hareket aynı karede durmalı; kapatınca kontrol geri gelmeli.
- [ ] Kamera takip etmeli; karakter döndüğünde kendi açısı dönmemeli.
- [ ] Ev, ağaç, istasyon ve harita sınırları: içinden geçme veya zeminden düşme olmamalı.
- [ ] 30/60/120 FPS'de tepkiyi karşılaştır; Hakan'ın hissiyatına göre Inspector değerlerini kaydet.

Mac üzerinde Unity bulunmadığından bu kontroller başarılı olarak işaretlenmedi. API referansları: [MoveTowards](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/vector3/movetowards), [RotateTowards](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/quaternion/rotatetowards).
