# Son Bir Umut: Kuraklıktan Önce — GAMEUP Türkiye 2026

Unity 6.3 LTS / C# / Windows için düşük poligonlu izometrik köy demosu. **Aşama 3 dalı: feature/playtest-ux**. Hakan'ın kullanıcı tarafından aktarılan Aşama 2 geri bildirimi temel alınmıştır. Çalışan yedek `feature/live-village-demo` / `1912d943819e6e115192f8d335981f64aed44e1f` korunur. Yeni sürüm bu Mac'te Unity Editor ile test edilmedi.

## Bu sürüm

- İnce tek HUD, beş kompakt kaynak göstergesi, süre/baskı ve bir kısa hedef. Uzakta sürekli etkileşim metni yok; bildirimler kısa.
- Üç anlaşılır karar kartı, büyük düğme, kısa fayda/bedel ve gerçek önizlemeden tur sonu etkileri. Hesap dökümü isteğe bağlı ayrıntıda.
- 32×28 harita: alan %33.3 arttı; daha geniş yollar, üç renkli bölge, ağaç kümeleri, kanal/rezervuar ve kenar ayrıntıları.
- Yeni sahnede 1.9× sprint (3.5 → 6.65), yumuşak hız/dönüş ve mevcut CharacterController/kamera.
- Toplam beş bakım varyasyonu, aynı anda en fazla iki açık görev; en az dört bakım ve tek arızanın değerlendirilmesiyle kriz başarısı hedefi. Tekrar ödül yok.
- 45 sn arıza, 2 sn E basılı onarım; net bar/saniye ve son12 sn artan vurgu. Tamirde sızıntı durur, bakımda onay görünür; güneş çalışma ışığı yalnız Solar seçilmişse yanar.
- Baskıdan daha belirgin uzak pus/solgunluk; yakın oyuncu/yol/istasyon için korunan görüş alanı. Emisyon ve fiziksel hava kirliliğiyle aynı değişken değildir.

240 aktif saniye korunur. Giriş/kart/odak dışı süre durur; serbest dolaşım/onarımda işler. Üç stratejik karar, güncel kriz kaynaklarıyla final ve yeniden oynama korunur. Ana menü, duraklatma menüsü veya ses sistemi eklenmedi. Değerler örnek oyun puanlarıdır; sis hazırlığın metaforudur, iklim değişikliği ortadan kalkmış sayılmaz.

## Hakan için ilk adım

**Çalışan Unity projesini yedekle.** Temiz kaynak depoda `feature/playtest-ux` dalını al. `UnityAssets/SonBirUmutDemo` içeriğini mevcut `Assets/SonBirUmutDemo` üzerine, eski `.meta`, Scenes ve Generated dosyalarını silmeden kopyala. Scripts ve Editor birlikte, yeni dosyalar dahil olmalı. Old veya Both input, Built-in 3D veya URP kullan.

Console derlemesi bitince **Tools > GameUP > Create Playtest Demo Scene**. Yeni sahne `Assets/SonBirUmutDemo/Scenes/SonBirUmutPlaytestDemo.unity`, varlıklar `Generated/PlaytestDemo...`; dolu yollar numaralanır. Mevcut sahneler ezilmez. Tek oyun sahnesidir; eski kaydedilmiş sahne kendiliğinden büyümez veya beş görev almaz.

| Menü | Kaynak profil |
| --- | --- |
| Create Playtest Demo Scene | Aşama 3: yeni harita, beş bakım/en az dört, 1.9 sprint |
| Create Crisis Demo Scene | Korunan üç bakım/en az iki, eski harita, 1.6 sprint |
| Create Crisis Foundation Scene | Süre/baskı temeli; görev/arıza yok |
| Create Demo Scene | Eski üç stratejik karar → final |

Bu daldaki ortak HUD/atmosfer kaynakları eski sahnelerde de değişir; birebir Aşama 2 yedeği kaynak dalın 1912d94 commit'indedir.

[Hakan'ın Aşama 3 test listesi](docs/PLAYTEST_UNITY_TEST.md) · [Kurulum](docs/INTEGRATION.md) · [UX/harita/mekanik ayrıntıları](docs/PLAYTEST_UX.md)

## Doğrulama

Mevcut .NET 8/9 SDK ile, harici test paketi gerekmeden:

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release -- --playtest-report docs/PLAYTEST_BALANCE.md
```

**136 test grubu geçti, 0 başarısız**: önceki 116 korundu, 20 yeni grup eklendi. Eski ve yeni modların 27'şer stratejik dizisi geçti. Yeni beş görevli örnek rota 195 aktif sn'de bitiyor; üç enerji yoluyla kriz başarısı ve üç genel final kategorisi korunuyor. **Bu gerçek Unity hareket/okuma süresi ölçümü değildir.** 3–4 dakika hedefi ekip testinde ölçülmeli; hızlı rota daha erken, kart okuma dahil toplam süre daha uzun olabilir.

[27 yeni oturumun sayısal raporu](docs/PLAYTEST_BALANCE.md) · [Gerçek test kaydı](docs/TEST_RESULTS.md) · [Mimari](docs/ARCHITECTURE.md)

Unity API derlemesi, fiziksel erişim, tuş/menzil, render/fog, 1280×720/1920×1080 UI, sahne referansları ve Windows build henüz doğrulanmadı. Kişisel belgeler/görseller, gizli bilgiler, build/cache çıktıları Git'e eklenmez; eski dallara merge/force push yapılmaz.

Tarihsel kaynaklar: [Aşama 2 test planı](docs/FINAL_UNITY_TEST.md), [Aşama 2 denge](docs/CRISIS_BALANCE.md), [Stratejik kurallar](docs/DECISIONS.md), [Görsel polish](docs/VISUAL_STAGE2.md).
