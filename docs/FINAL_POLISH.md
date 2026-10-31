# Final Polish — ses, okunabilirlik ve teslim hazırlığı

Dal `feature/final-polish`; korunan kaynak `feature/final-pacing / 747c08afa2ba9ace9e8fdee77d9f0b83bc670c4f`. Bu çalışma Unity/Windows testine hazırlıktır; doğrulanmış Windows teslimi değildir. Mac'te Unity Editor yoktur.

## Korunan oynanış

GameRunState, FinalPacingCatalog, MaintenanceTask/CrisisProgress, ReleaseFlow, CrisisRules, EmergencyIncident, GameRunSettings, karar/sonuç formülleri, motor, dünya ve görev geometrisi değiştirilmedi.32×28,10 node/oturumda6/en fazla2/en az4,240aktif sn,45/2 arıza,1.9sprint ve üç final aynı. Eski RC 5/4 ve diğer modlar kaynak dal uçlarıyla da korunur.

## Küçük özgün atmosfer

[Audio klasörü](../UnityAssets/SonBirUmutDemo/Audio/README.md): HopefulMenu.wav 12 sn, VillageWind.wav 8 sn, CrisisUndertone.wav 8 sn. Özgün Python standart kütüphane sentezi; mono 22050 Hz/16-bit PCM, toplam 1234932 bayt. Dış melodi/kayıt/paket/vokal yoktur. Su bölgesine özel ayrı akış katmanı eklenmedi. Müzikal kalite, sesin duyulması ve karışım henüz dinlenmedi; teknik PCM doğrulaması kalite garantisi değildir.

DemoAudioManager'a serileştirilmiş üç döngü referansı eklenir; yalnız yeni **Final Polish** sahne menüsü bunları bağlar. Eski sahneler mevcut kısa efektlerle çalışır. Dört sabit AudioSource, bir AudioListener; runtime source/clip çoğaltması yoktur. Runtime altı kısa efektin kendi AudioClip'lerini bir kez üretmesi korunur; import edilmiş WAV asset'leri manager tarafından Destroy edilmez.

| Bağlam | Ses sunumu |
| --- | --- |
| Ana menü / HowTo / ana menü ayarları / Intro | Sakin menü müziği |
| Oynanış | Rüzgâr; 55–100 baskıda kademeli çok hafif uğultu |
| Kart | Düşürülmüş rüzgâr, uğultu yok |
| Kullanıcı pause / pause ayarları | Hafif menü teması, çok düşük rüzgâr, uğultu yok |
| Yeniden Doğuş | Hafif umutlu tema/rüzgâr, uğultu yok |
| Eksik Dönüşüm | Çok hafif tema/rüzgâr, sınırlı baskı dokusu |
| Kaybedilen Gelecek | Rüzgâr/sınırlı baskı dokusu; umutlu tema yok |
| Mute / focus yok / native pause / disable | Kaynaklar durur; dönüşte tek başlatma |

Miks hedefleri yaklaşık 0.33 sn içinde yumuşar. Yeni oyun/restart/menu, kısa efektleri ve eski gerilim kazancını temizler; müzik/rüzgârı yeni bağlama yumuşatırken döngü playhead'lerini yeniden başlatmaz. Uygulama arka plana geçtiğinde durdurma anlıktır; geri dönüş fade ile olur. Ses fiziği/iklim hesabı yoktur; baskı boyutsuz oyun göstergesidir, emisyon bu ses karışımının girdisi değildir.

## Gerçek ayarlar ve efekt güvenliği

Ana ses, Müzik, Efekt ve ortam, Tüm sesleri kapat. Her ilgili AudioSource'un volume/mute değerine uygulanır; müzik hazır değilse kontrol pasiftir, ses eksikliği kısa açıklamayla görünür. Ana master `.65`, yeni müzik `.55`, efekt `1` varsayılır. Mevcut `SonBirUmut.Release.Audio.Volume.v1` ve `Mute.v1` değiştirilmez. Yeni `Music.v1`/`Effects.v1` aynı önek altında saklanır; eksik/geçersiz kayıt güvenli varsayılanla okunur. Eski ayarlar sıfırlanmaz; mute düzeyleri kaybettirmez. Toggle/menüden çıkış/focus/native pause/disable/kapanışta kirli tercihler saklanır; sliderda sürekli disk yazımı yoktur.

Altı kısa efekt korunur: menü, karar, bakım, acil, olumlu final, olumsuz final. SoundEventGate aynı cue/kare ve 75 ms tekrarını engeller, en fazla iki kısa voice'a izin verir. Acil/final eski kısa efektleri keserek öncelik alır; kaynak sayısı artmaz. Menü/IMGUI'nin mevcut tek komut/kare koruması sürer. Efekt sesinin0 olması rüzgâr/gerilimi de susturur; müzik bağımsız kalır. Master 0/mute hepsini susturur. Gerçekte duyulma, mix/headroom ve PlayerPrefs disk kalıcılığı Windows'ta denenmeli.

Eksik WAV'lar/import hatası eski kısa efektli Final Pacing kurulumuna düşer. Runtime efekt üretimi ve atmosfer hazırlığı ayrı yakalanır; birinin hatası diğerini veya oyunu durdurmaz. `Available`/`AtmosphereAvailable` hazırlık durumudur, donanım testi değildir. Audio device kapanıp açılması, uzun suspend ve gerçek Unity hata davranışı manuel test bekler.

## Küçük UX düzeltmeleri

- HowTo gerçek10/6/4,240sn,45/2, üç karar ve tuşları kısa satırlarda açıklar; eski RC sayıları dinamik5/4 kalır. İlk Intro enerji hedefini açık söyler.
- Kaynak incelemesinde son60sn yalnız renkle belirtiliyordu. Artık60/20 uyarılarında metin + `!` + renk vardır. Hedef etiketi bu şeritten aşağı sıkıştırılır; acil panel önceliği sürer.
- Finalin mevcut kısa açıklamasına, yeni sahnede gerçek sonuç kaynaklarından bir neden eklenir: su/bitki/emisyon/enerji/tampon veya istikrarsız nöbet. Hesap/kategori değişmez; rastgele başarı mesajı yoktur. Rebirth olumlu, diğer iki kategori olumsuz kısa cue kullanır.
- Ses kapalıyken acil panel/yön/geri sayım, son süre metni, bakım onayı/bildirimi ve final başlığı bağımsız çalışır. Yeni büyük panel, minimap, görev listesi veya UI altyapısı yoktur.

Doğrulanmış Unity crash/fizik hatası bulunmuş gibi raporlanmaz. Çift cue/üst üste ses için koruma eklendi; önceki sürümde duyulmuş bir patlama ölçümü yoktur. Görsel kontrast/font/fog/taşma yalnız kaynak düzeyinde kontrol edildi.

## Sahne ve API sınırı

**Tools > GameUP > Create Final Polish Scene** → benzersiz `Assets/SonBirUmutDemo/Scenes/SonBirUmutFinalPolish.unity` ve `Generated/FinalPolish...`. FinalPacing profilini aynen kullanır; eski altı menü/path korunur. Play/Prefab/kirli sahne/input/pipeline korumaları sürer. AudioImporter yalnız üç yeni WAV'ın ayarını değiştirir; global Player Settings veya başka proje varlıkları değişmez.

Unity 6.3 resmi [AudioImporterSampleSettings](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/audioimportersamplesettings), [AudioImporter](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/audioimporter) ve [AudioSource.volume](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/audiosource/volume) referansları kontrol edildi. Bu Unity API derlemesi değildir.

## Gerçek testler ve kalan risk

**192 geçti, 0 başarısız**: önceki 178 korundu, 14 yeni grup. Ses düzeyi/mute, loop transport tek başlatma/durdurma, mix bağlamları/pressure, cue dedup/voice limiti/öncelik,60/20 sınırları, gerçekHowTo ve 27 rotanın final/cue/neden uyumu test edildi. Eski görev/rota/arıza/restart/RC regresyonları aynı beklentilerle çalıştı. AudioSource yönetimi **saf model düzeyinde** doğrulandı; gerçek Unity kaynak sayısı/ses testi değildir.

`python3 tools/generate_audio.py --check`: üç WAV'ın format/süre/genlik/RMS/DC/baş-son örnek/eğim kontrolleri geçti. 46Unity C#×5 Editor/player input sembol birleşimi C#9 parser kontrolü: 0 hata; API/tür derlemesi değil. Yeni SDK/paket kurulmadı.

Unity API derlemesi, WAV importer/serileştirme, gerçek source/Listener sayısı, dinleme/döngü birleşimi/fade/cihaz/mute/prefs, GUI/iki çözünürlük, fiziksel10 node erişimi, gerçek tempo/fog ve Windows EXE **doğrulanmadı**. P2 görsel efektler eklenmedi; küçük ses geçişi dışında kapsam genişlemedi. [Hakan'ın son kabul listesi](FINAL_ACCEPTANCE_TEST.md) henüz işaretlenmedi.

## Dosya dökümü

Yeni: Simulation/AudioPresentation.cs; Editor/DemoPolishAudioBuilder.cs; Tests/LogicTests/FinalPolishTests.cs; tools/generate_audio.py; Audio/HopefulMenu.wav, VillageWind.wav, CrisisUndertone.wav ve Audio/README.md; docs/FINAL_POLISH.md, FINAL_ACCEPTANCE_TEST.md.

Güncellenen: SoundPreferences.cs, DemoAudioManager.cs, DemoGameController.cs, DemoStory.cs, DemoHud.cs, DemoSceneBuilder.cs, test Program.cs/csproj ve README.md, UNITY_TEST_CHECKLIST.md, docs/ARCHITECTURE.md, INTEGRATION.md, TEST_RESULTS.md. Audio/Scripts/Editor yolları UnityAssets/SonBirUmutDemo altındadır. Kaynak oynanış kuralları/eski test dosyaları/kişisel medya değişmedi.
