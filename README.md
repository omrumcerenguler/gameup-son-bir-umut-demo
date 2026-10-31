# Son Bir Umut

GAMEUP Türkiye 2026 Hackathonu için iklim değişikliği, kuraklık ve kırsal yaşam temalı Unity 6.3 LTS / C# demosu.

**Durum:** Tek enerji istasyonlu prototip kaynakları ve Editor sahne oluşturucusu hazırdır. Mac üzerinde Unity kurulu değildir; Unity derlemesi, sahne üretimi, oynanış ve Windows build doğrulaması ekip tarafından yapılmalıdır. Depo henüz Unity Hub'da doğrudan açılabilecek bir proje içermez.

## İlk prototip

- Primitive 3D nesnelerden küçük kırsal harita, evler, ağaçlar ve su deposu.
- Kapsül oyuncu, kamera yönüne göre WASD hareket, sabit izometrik açı ve smooth follow.
- E ile enerji istasyonunda üç karar: güneş, dizel veya tasarruf; kartlar açıkken hareket girdisi durur.
- Bütçe, enerji, su, bitki sağlığı ve emisyon için merkezi saf C# hesapları.
- Seçime bağlı panel/jeneratör/tasarruf görseli, tamamlanma, sonuç mesajı ve yeniden oynama.
- IMGUI arayüzü; harici model veya üçüncü taraf UI paketi gerekmez.

Değerler örnek oyun puanlarıdır, bilimsel ölçüm değildir. Emisyon anlık yerel sıcaklık artışına bağlanmaz. Gelecekteki su pompaları için enerjiye bağlı kapasite hesabı vardır; bu sürümde pompa çalışmaz.

## Windows ekibi için başlangıç

1. Unity **6.3 LTS**, Universal 3D (URP) veya 3D Built-in test projesi oluştur.
2. `UnityAssets/SonBirUmutDemo` klasörünü `Assets/SonBirUmutDemo` içine kopyala; `Editor` alt klasörünü dahil et.
3. Active Input Handling ayarını **Both** veya **Input Manager (Old)** yap; gerekirse Editor'ı yeniden başlat. Script derlemesi bittikten sonra **Tools > GameUP > Create Demo Scene** seç.
4. Oluşan sahnede Play'e bas. Game görünümüne tıkla, WASD ile sarı istasyona yaklaş ve E'ye bas.

Tam kurulum, input modları, beklenen değerler ve Console kontrolleri: [INTEGRATION](docs/INTEGRATION.md). Sistem sorumlulukları ve hesaplar: [ARCHITECTURE](docs/ARCHITECTURE.md).

## Kaynak düzeni

```text
UnityAssets/SonBirUmutDemo/
  Editor/                 Sahne oluşturucu
  Scripts/
    Core/                 Oturum ve giriş adaptörü
    Player/               Hareket ve kamera
    Interaction/          Enerji istasyonu
    Decisions/            Saf C# karar tanımları
    Simulation/           Saf C# kaynak ve oturum hesapları
    UI/                   IMGUI arayüzü
    World/                İlerisi için ayrılan klasör
  Data/                   İlerisi için ayrılan veri klasörleri
Tests/LogicTests/          Paket gerektirmeyen .NET test koşucusu
docs/                     Mimari ve Windows entegrasyonu
```

Kaynakların asıl konumu `UnityAssets` altıdır. Test projesindeki düzenlemeler buraya geri taşınır. Sahne ve materyaller Windows'taki gerçek Editor tarafından üretilir; sahte Unity proje dosyaları bulunmaz.

## Otomatik mantık testleri

Mevcut .NET 8 veya 9 SDK ile depo kökünde:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release
```

Koşucu başarısızlıkta sıfırdan farklı kod döndürür; harici NuGet paketi kullanmaz. Unity input, fizik, görüntü ve Editor doğrulaması ayrıca yapılmalıdır.

## Sonraki aşamalar

Üç karar noktasına genişletme, su/bitki kararları, daha kapsamlı görsel dünya değişimleri ve tam final akışı sonraya bırakılmıştır. Crafting, çöp toplama, NPC, kapsamlı hikâye ve animasyon bu sürümün kapsamında değildir.

## Git ve gizlilik

[GitHub deposu](https://github.com/omrumcerenguler/gameup-son-bir-umut-demo) · SSH: `git@github.com:omrumcerenguler/gameup-son-bir-umut-demo.git`

Depo herkese açıktır. Mevcut yerel PDF, görsel ve WhatsApp ekran görüntüleri paylaşılmaz; değiştirilmeden yerinde korunur. `.gitignore` kişisel belgeleri, gizli anahtarları ve üretilen dosyaları dışarıda tutmaya yardımcı olur. Yalnızca açık dosya yollarını stage edin; her commit öncesinde staged dosya listesi ve içeriğini inceleyin. Git kimliğinde özel e-posta yerine GitHub noreply adresini kullanın. Force push kullanmayın.
