# Son Bir Umut: Kuraklıktan Önce — GAMEUP Türkiye 2026

**KRİZ NÖBETİ** · Unity 6.3 LTS / C# / Windows · `feature/final-polish`.

Korunan kaynak: `feature/final-pacing / 747c08afa2ba9ace9e8fdee77d9f0b83bc670c4f`. Son ses/UX ve teslim hazırlığıdır. **Mac'te Unity Editor yok; gerçek Unity derlemesi, dinleme, fizik, GUI ve Windows build doğrulanmadı.**

## Bu sürüm

- Yeni Final Polish sahnesinde özgün 12 sn menü müziği, 8 sn rüzgâr ve 8 sn hafif baskı uğultusu. Üç standart PCM WAV, toplam yaklaşık 1.18 MiB; dış kayıt/paket/vokal yok. Müzikal kaliteyi Hakan dinlemeli.
- Gerçek Ana ses / Müzik / Efekt ve ortam / Mute; eski master/mute kayıt anahtarları korunur. Müzik, pause/menü/final bağlamına göre kısa miks geçişleri kullanır; focus/native pause/mute kaynakları durdurur. Kaynak sayısı sabit, eski altı cue korunur; aynı cue tekrarına ve yoğun üst üste çalmaya sınır getirildi.
- İlk hedef açıkça enerji; HowTo 10 nokta/6 görev/en az 4 ve 240/45/2 kurallarını kısa satırlarda açıklar. Son 60/20 sn uyarıları renk yanında metin ve `!` taşır. Finalde gerçek kaynaklara bağlı kısa neden eklenir.
- Oynanış değişmedi: 32×28 köy, üç stratejik karar, 10 node/oturumda 6/en fazla 2/en az 4, tek arıza 45/2, 240 aktif sn, 1.9×Shift, baskı/fog, üç final, menü/pause/restart/replay. Karar ekonomisi, görev/çağrı saatleri ve final eşikleri korunur.

Yeni görev/harita/NPC/envanter/crafting/save-load/pipeline eklenmedi. Suya özel ayrı akış katmanı ve ek görsel geçişler kapsam dışında kaldı. Sayılar oyun parametreleridir; iklim değişikliğinin çözüldüğü iddia edilmez.

## Hakan'ın ilk adımı

**Çalışan gerçek Unity projesini önce yedekle.** Temiz kaynak depoda `feature/final-polish` dalını al. UnityAssets/SonBirUmutDemo içeriğinin **tamamını** dış projenin Assets/SonBirUmutDemo klasörüne aktar: Scripts + Editor + **Audio ve üç WAV**. Eski .meta/Scenes/Generated silinmez. Unity 6.3 LTS, Built-in/URP ve Input Handling **Old veya Both**.

Console derlemesinden sonra **Tools > GameUP > Create Final Polish Scene**. Yeni `Assets/SonBirUmutDemo/Scenes/SonBirUmutFinalPolish.unity` ve `Generated/FinalPolish...` benzersiz numaralanır. Kaydet/kapat/yeniden aç; Play ile ana menüden başla. Araç global Player Settings'i değiştirmez.

| Menü | Profil |
| --- | --- |
| Create Final Polish Scene | Aynı Final Pacing oynanışı + özgün arka plan sesleri |
| Create Final Pacing Scene | Korunan 10 node/6 görev/4 asgari, kısa efektler |
| Create Release Candidate Scene | Korunan 5 görev/4 asgari |
| Create Playtest Demo Scene | Korunan Aşama 3 beş görevli profil |
| Create Crisis Demo Scene | Eski üç bakım/en az iki |
| Create Crisis Foundation Scene | Süre/baskı temeli |
| Create Demo Scene | Eski üç stratejik karar→final |

Eski sahne otomatik müzik almaz. Eksik WAV/import hatası uyarıyla kısa efektli kurulum oluşturur; runtime ses katmanı hatası temel simülasyonu durdurmaz. Sessiz modda kritik olaylar görsel/metinle anlaşılmalıdır.

[Son kabul ve Windows teslim listesi](docs/FINAL_ACCEPTANCE_TEST.md) · [Kurulum](docs/INTEGRATION.md) · [Ses/UX/dosya dökümü ve riskler](docs/FINAL_POLISH.md) · [Özgün sesler](UnityAssets/SonBirUmutDemo/Audio/README.md)

## Gerçekte çalıştırılanlar

**192 test grubu geçti, 0 başarısız**: önceki 178 aynı beklentilerle korundu, 14 yeni grup. Ses tercihi/mix/loop transport/cue güvenliği, uyarı sınırları, gerçek HowTo ve 27 rotanın final/cue uyumu doğrulandı. Gerçek AudioSource/GUI/dinleme testi değildir. 46 C# dosyası ×5 sembol C#9 sözdizimi kontrolü hatasız; Unity API/tür derlemesi değildir.

```sh
dotnet run --project Tests/LogicTests/LogicTests.csproj --configuration Release
python3 tools/generate_audio.py --check
```

Üç WAV'ın format/süre/genlik/RMS/DC ve döngü kenarı sayısal kontrolleri geçti. Yeni SDK/paket kurulmadı. [Test kaydı](docs/TEST_RESULTS.md) · [Mimari](docs/ARCHITECTURE.md).

[Korunan rota tahmini](docs/FINAL_PACING_BALANCE.md) hızlı 87–90/yeni oyuncu 155–175 aktif sn verir; gerçek oyuncu ölçümü değildir. Sesin duyulması/seviye/kalıcılık, gerçek 10 node erişimi/fog, iki çözünürlük ve Windows EXE ekip testi bekler. Scene List'te FinalPolish ilk/etkin; EXE/_Data/UnityPlayer.dll ve bütün yan dosyalar birlikte teslim edilmeli. Tam Unity proje yedeği Assets/Packages/ProjectSettings ve gerekli .meta içermeli.

Eski dallar/kişisel belgeler korunur. Kişisel medya, gizli bilgi ve build/cache Git'e eklenmez; merge veya force push yapılmaz. [Korunan Final Pacing tasarımı](docs/FINAL_PACING.md).
