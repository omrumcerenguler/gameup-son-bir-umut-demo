# Son Bir Umut

GAMEUP Türkiye 2026 Hackathonu için geliştirilecek, iklim değişikliği ve kuraklık temalı Unity oyun demosu.

**Durum:** Yalnızca depo altyapısı ve geliştirme dokümantasyonu hazırdır. Henüz oyun kodu, çalıştırılabilir demo veya Unity Editor ile oluşturulmuş proje bulunmaz.

## Hedeflenen demo

- Unity 6.x ve C#; low-poly 3D görünüm ve izometrik kamera.
- Tek küçük kırsal harita; WASD hareket ve E ile etkileşim.
- Üç karar noktası; her noktada üç seçenekli karar kartları.
- Birbirine bağlı enerji, su ve bitki sistemleri.
- Kararlara göre görsel dünya değişimleri.
- Final ekranı ve yeniden oynama.

Bu özellikler sonraki geliştirme aşamasının kapsamıdır; henüz uygulanmamıştır.

## Depo yapısı

```text
docs/
  ARCHITECTURE.md       Planlanan sorumluluklar ve veri akışı
  INTEGRATION.md        Windows / Unity entegrasyonu ve ekip iş akışı
UnityAssets/
  SonBirUmutDemo/
    Scripts/
      Core/
      Player/
      Interaction/
      Decisions/
      Simulation/
      World/
      UI/
    Data/
      Decisions/
      Simulation/
```

Boş kaynak klasörleri `.gitkeep` ile korunur. `UnityAssets`, ileride gerçek Unity projesinin `Assets` klasörüne aktarılacak kaynakların hazırlık alanıdır. Şu anda bu depo Unity Hub üzerinden proje olarak açılamaz.

## Ekip çalışma düzeni

MacBook M4 üzerinde Codex ile kod ve dokümantasyon geliştirilecek. Unity projesini oluşturma, içe aktarma, derleme ve oyun testlerini Windows kullanan ekip üyeleri gerçekleştirecek. Unity Editor ve büyük SDK kurulumu bu altyapının parçası değildir.

Başlamadan önce [mimari planını](docs/ARCHITECTURE.md) ve [entegrasyon adımlarını](docs/INTEGRATION.md) okuyun. Unity 6.x'in kesin sürümü, render pipeline ve paket sürümleri gerçek proje oluşturulurken ekipçe belirlenip kaydedilecek.

## Git ve gizlilik

GitHub: https://github.com/omrumcerenguler/gameup-son-bir-umut-demo

SSH remote: `git@github.com:omrumcerenguler/gameup-son-bir-umut-demo.git`

Depo herkese açıktır. Kök klasördeki yerel PDF'ler, PNG/JPG/JPEG görselleri ve WhatsApp ekran görüntüleri paylaşılmayacak; mevcut dosyalar olduğu yerde korunacak. `.gitignore` bu dosyalar için koruma sağlar, ancak tek başına gizlilik denetimi yerine geçmez.

Dosyaları açık yollarıyla seçerek stage edin; toplu ekleme ve ignore kurallarını zorlayan ekleme kullanmayın. Her commit öncesinde `git diff --cached --name-status` ve `git diff --cached` ile dosya listesini ve içeriği inceleyin. Kişisel belge, erişim anahtarı, token veya parola eklemeyin. Git commit kimliğinde özel e-posta yerine GitHub noreply adresi kullanın.
