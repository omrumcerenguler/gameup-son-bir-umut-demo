# Unity entegrasyonu ve ekip iş akışı

## Mevcut durum

Bu depo bir kaynak hazırlık alanıdır. `UnityAssets/SonBirUmutDemo` içinde yalnızca klasör yer tutucuları bulunur; Unity projesi ve oyun kodu henüz yoktur. Mac üzerinde Unity, Xcode veya büyük SDK kurulumu gerekmez. Editor testleri Windows ekip üyelerince yapılacaktır.

## Windows ekibiyle ilk entegrasyon

1. Ekip Unity 6.x'in kesin Editor sürümünü, render pipeline'ı ve giriş sistemini belirlesin; seçilen sürümleri bu belgeye kaydetsin.
2. Windows'ta Unity Hub / Editor kullanarak depo içinde `UnityProject/` adlı gerçek bir proje oluşturulsun. Bu klasör henüz mevcut değildir; elle sahte Unity dosyaları oluşturulmasın.
3. `UnityAssets/SonBirUmutDemo/` klasörü, ilk aktarımda `UnityProject/Assets/SonBirUmutDemo/` altına kopyalansın. `.gitkeep` dosyaları hedefte gerekli değildir ve aktarılmayabilir. İçe aktarma sırasında `.meta` dosyalarını Editor üretsin.
4. Editor ayarlarında Asset Serialization için `Force Text`, Version Control için `Visible Meta Files` kullanılsın; ayarlar doğrulansın.
5. İlk entegrasyon tamamlandığında oyun kaynaklarının tek asıl konumu `UnityProject/Assets/SonBirUmutDemo/` olsun. Hazırlık klasörü kontrollü olarak kaldırılsın; iki kopya paralel geliştirilmesin. README ve bu belgedeki yollar aynı commit içinde güncellensin.
6. `UnityProject/Assets/` içindeki gerekli varlıklar ve `.meta` dosyaları, `UnityProject/Packages/manifest.json`, `UnityProject/Packages/packages-lock.json` ve `UnityProject/ProjectSettings/` Editor tarafından gerçekten oluşturulup incelendikten sonra seçilerek Git'e eklensin. `ProjectVersion.txt` kesin Editor sürümünün kaydı olarak korunsun. `Library`, `Temp`, `Logs`, `UserSettings` ve build çıktıları eklenmesin.
7. Proje Windows'ta açılıp derleme hataları ve Console hataları kontrol edilsin. Oyun henüz uygulanmadığından ilk kontrol yalnızca projenin ve kaynakların sağlıklı içe aktarılmasıdır.

Bu geçiş sonraki aşamanın işidir; mevcut altyapı commit'i bir Unity projesi içermez.

## Günlük geliştirme

- Değişikliklere başlamadan önce çalışma ağacını ve uzak dalı kontrol edin; yerel değişikliklerin üzerine yazmayın. Temiz çalışma ağacında `git pull --ff-only` kullanın.
- Mac üzerinde C# ve dokümantasyon düzenlenebilir. Unity derleme ve çalışma doğrulamasını Windows'ta yapın; Editor çalıştırılmadıysa test edildiğini belirtmeyin.
- Varlıkları Editor içinden taşıyın. `.meta` dosyalarını varlıklarıyla birlikte koruyun; GUID kaybı sahne ve prefab referanslarını bozabilir.
- Dosyaları tek tek veya açık dosya yollarıyla stage edin. `git add .`, `git add -A` ve `git add -f` kullanmayın.
- Commit öncesi `git diff --cached --name-status` ve `git diff --cached` çıktısını kontrol edin. Önceden izlenen gizli bir dosyayı `.gitignore` geçmişten çıkarmaz.
- Herkese açık depoya kişisel belge, WhatsApp ekran görüntüsü, gizli anahtar, token veya parola eklemeyin. Git kimliğinde özel e-posta yerine GitHub noreply adresini kullanın.
- Kök klasördeki mevcut PDF ve görselleri yerinde tutun; taşımayın, değiştirmeyin veya yüklemeyin. Gelecekte eklenecek oyun varlıkları için paylaşım hakkını ve içeriği ayrıca kontrol edin.
- Force push kullanmayın. Uzak dal ilerlemişse önce değişiklikleri inceleyip normal Git akışıyla birleştirin.

## Sonraki aşamada Windows doğrulaması

Oyun özellikleri uygulandıktan sonra aşağıdaki kontroller yapılacak; şu anda tamamlanmış sayılmazlar:

- WASD hareket, izometrik kamera ve E etkileşimi.
- Üç karar noktası, her birinde üç seçenek ve aynı kararın tekrar uygulanmaması.
- Enerji, su ve bitki ilişkileri; kaynak sınırları ve görsel dünya değişimleri.
- Final ekranı ve yeniden oynama sonrasında tüm oturum durumunun sıfırlanması.
- Windows build'inin açılması ve baştan sona kısa oynanış kontrolü.

Doğrulama kaydında commit kimliği, kesin Editor sürümü, test edilen platform, sonuç ve bilinen sorunlar belirtilsin.
