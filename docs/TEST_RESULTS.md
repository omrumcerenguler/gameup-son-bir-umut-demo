# Doğrulama kaydı — polish aşama 1

## Gerçekten çalıştırılan kontroller

- Mevcut .NET 9.0.303 SDK, net8.0 hedefi ve C# 9 ile saf mantık kaynakları derlendi. Yeni SDK veya NuGet paketi kurulmadı.
- `dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --report docs/DECISION_MATRIX.md`: **54 test grubu geçti, 0 başarısız**. Önceki 39 test grubu korundu; güneş emisyon beklentisi 0 olarak güncellendi ve 15 yeni grup eklendi.
- 27 temel karar dizisinin 25'i doğrudan tamamlandı. Su yetersizliğinden engellenen 2 sulama dizisinde ücretsiz depolama ile tamamlanma doğrulandı. Üç final kategorisine ulaşıldı; hem koruma hem sulama ile Yeniden Doğuş mümkün.
- Yeni kapsam: seçim etkisi / tur kullanımı / su koşulu / dışsal kuraklığın ayrımı; değişen kuraklığın seçim etkisini değiştirmemesi; kaynak sınırlarında bileşenlerle net değişimin uzlaşması; doğru bildirim; kart metinlerinin tamamlığı; alternatif başarılı kaynak dağılımları; mevcut durum/ID'lerden sonuç açıklamaları; yeni oturumda özet/geçmiş sıfırlanması.
- Önceki sıra, bütçe/enerji/su yetersizliği, atomik ret, tekrar engeli, kaynak sınırları, sıfır kaynakla ilerleme, pompa bağımlılığı ve verimlilik testleri de geçti.
- 19 C# kaynak dosyası, geçici Roslyn denetleyicisiyle C# 9 düzeyinde 5 Editor/player input sembol birleşiminde tarandı; **sözdizimi hatası yok**. Bu Unity API/tür çözümlemesi veya Unity derlemesi değildir; araç depoya eklenmedi.
- `git diff --check`, açık stage dosya listesi ve içerik incelemesi yapıldı. Korunan dallar ve yerel PDF/görseller için commit/içerik özeti ile değişiklik zamanı karşılaştırması yapıldı.

## Kaynak dalın ekip doğrulaması

Kullanıcının aktarımına göre Hakan `feature/complete-demo` sürümünü Unity'de açtı; oynanış videosunda enerji, su ve tarım seçiliyor, dünya nesneleri değişiyor ve **Eksik Dönüşüm** finaline ulaşılıyor. Bu, kullanıcının aktardığı ekip doğrulamasıdır; yeni polish sürümünün testi değildir.

## Henüz yapılmayanlar

Bu Mac'te Unity Editor yoktur. Yeni polish sürümünün Unity 6.3 LTS derlemesi, genişletilen HUD/kart/bildirim/final metinlerinin okunabilirliği, ×/Esc ve zaman aşımı, mevcut sahne uyumu, yeniden oynama sırasında bildirim temizliği ve Windows build doğrulanmadı. Korunan hareket, kamera ve sahne yapısında da regresyon kontrolü Hakan/Enes'tedir.

3–5 dakika deneyim süresi ölçülmedi. Yeni 3D model veya görsel dünya düzenlemesi yapılmadı. Doğrulanmış yeni bir Unity hata kaydı yoktur; Editor testi olmadan hatasız çalışma iddiası yoktur.

[Kontrol listesi](../UNITY_TEST_CHECKLIST.md) kutularını yalnız gerçekten çalıştırılan polish adımlarıyla doldurun. Commit, tam Unity patch sürümü, Windows, pipeline, input modu ve ilk Console hatasını kaydedin. Saf C# geçişi UI/fizik testinin yerine geçmez.
