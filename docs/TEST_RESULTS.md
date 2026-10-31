# Doğrulama kaydı

## Gerçekten çalıştırılan kontroller

- Mevcut .NET 9.0.303 SDK, net8.0 hedefi ve C# 9 ile saf mantık kaynakları derlendi; yeni SDK veya NuGet paketi kurulmadı.
- `dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md`: **39 test grubu geçti, 0 başarısız**.
- 27 temel karar dizisinde 25 doğrudan tamamlanma, 2 doğru engellenen sulama ve her ikisinde ücretsiz depolama ile tamamlanma doğrulandı. Üç final kategorisine ulaşıldı.
- Ek kapsam: karar sırası, yetersiz bütçe/enerji/su, atomik ret, tekrar engeli, kaynak sınırları, sıfır kaynakla ilerleme, pompa bağımlılığı, kalıcı su verimliliği, suya bağlı bitki desteği, emisyon ayrımı, önizleme/uygulama eşitliği, geçmiş ve yeni oturum sıfırlaması.
- 16 C# kaynak dosyası, geçici Roslyn denetleyicisiyle C# 9 düzeyinde 5 Editor/player input sembol birleşiminde tarandı; **sözdizimi hatası yok**. Bu araç Unity API/tür çözümlemesi veya Unity derlemesi yapmaz ve depoya dahil değildir.
- `git diff --check` / staged dosya listesi denetlendi. Yerel PDF ve görseller için içerik özeti ve değişiklik zamanı karşılaştırması yapıldı; korunuyor ve izlenmiyorlar.

## Ekipten aktarılan önceki durum

Kullanıcının aktarımına göre Hakan, önceki `main` enerji prototipinde kırsal harita, kapsül karakter, izometrik kamera ve HUD'ı Unity'de görüntüledi. Hareketin sert hissedildiğini bildirdi. Bu kayıt yeni feature sürümünün testi değildir.

## Henüz yapılmayanlar

Bu Mac'te Unity Editor bulunmadığından yeni sürümün Unity 6.3 LTS derlemesi, sahne üretimi, eski sahne uyumu, CharacterController çarpışmaları, akıcı hareket hissi, input/fare, IMGUI düzeni, shader/dünya renkleri, final kadrajı ve Windows build doğrulanmadı. Otomatik mantık testleri bu kontrollerin yerine geçmez.

3–5 dakika deneyim süresi ölçülmedi. Görsel kalite primitive prototip düzeyindedir. Klavye sekiz dijital hedef üretir; sürekli hız ve yumuşak yönelim vardır, analog giriş eklenmedi. Yeni sürümde doğrulanmış bir Unity hata kaydı yoktur; Editor doğrulaması olmadan hatasız çalışma iddiası da yoktur.

Hakan/Enes [kontrol listesini](../UNITY_TEST_CHECKLIST.md) gerçek sonuçlarla doldurmalıdır. Her kayıt commit, tam Unity patch sürümü, Windows, pipeline, input modu, test edilen adım ve başarısızlık varsa ilk Console hatasını içermelidir.
