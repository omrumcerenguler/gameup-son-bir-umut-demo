# Özgün kısa ses atmosferi

Bu üç dosya yalnız Python standart kütüphaneleriyle `tools/generate_audio.py` tarafından üretildi. Dış kayıt, ses bankası, üçüncü taraf paket, vokal veya başka bir oyunun/filmin müziği kullanılmadı. Menü parçası özgün kısa arpej düzenlemesidir; basit akorların kullanımı bir şarkı uyarlaması değildir.

| Dosya | İçerik | Süre | Boyut | PCM tepe / RMS |
| --- | --- | ---: | ---: | --- |
| HopefulMenu.wav | Yumuşak, hafif melankolik dört arpej | 12 sn | 529244 bayt | 0.141235 / 0.051329 |
| VillageWind.wav | Periyodik, yavaş değişen rüzgâr dokusu | 8 sn | 352844 bayt | 0.225800 / 0.045558 |
| CrisisUndertone.wav | Çok düşük endüstriyel uğultu/doku | 8 sn | 352844 bayt | 0.128754 / 0.028169 |

Hepsi mono, 22050 Hz, signed 16-bit little-endian PCM WAV. Toplam **1234932 bayt**; Unity float PCM'e açarsa yaklaşık 2.35 MiB ham döngü verisi (nesne/efekt ek yükü hariç). Büyük müzik paketi/streaming bağımlılığı yoktur. Bölgeye özel su akışı eklenmedi; küçük rüzgâr ve baskı katmanıyla yetinildi.

Doku frekansları tam döngülere oturur. Menü notalarının kuyrukları döngü başına dairesel eklenir; her dosyada 15 ms yumuşak kenar uygulanır. Baş/son örnekleri sıfır, birleşim farkı 0; kenar eğimi/DC/genlik/format sayısal kontrolden geçti. **Dinleme testi değildir; birleşimde duyulabilir değişim olup olmadığı, müzikal kalite ve cihaz seviyesi Unity'de dinlenmelidir.** Rahatsız ederse Müzik düzeyini 0 yap; efekt/ortam bağımsız kalır.

Repo kökünden mevcut Python3 ile:

```sh
python3 tools/generate_audio.py --check
```

Bu komut dosya değiştirmez. `--check` olmadan yalnız yukarıdaki üç isim yeniden üretilir; mevcut dosyaların üzerine yazar. Yeniden üretmek bu teslimin kurulumu için gerekmez.

Windows'ta yalnız **Create Final Polish Scene** aracı üç WAV'ın kendi AudioImporter ayarlarını PCM/DecompressOnLoad/PreserveSampleRate/preload/mono yapar; global Player/Audio/pipeline ayarlarına dokunmaz. Mevcut .meta GUID'leri silinmez. Diğer altı sahne menüsü arka plan sesi eklemez. Eksik WAV/import hatasında uyarıyla yalnız mevcut kısa efektli Final Pacing sahnesi oluşturulur; runtime katman hatası temel simülasyonu durdurmaz.

Dört sabit AudioSource: mevcut efekt kaynağı + menü müziği/rüzgâr/baskı için üç döngü. Tek AudioListener kameradadır. Runtime yeni source oluşturmaz. Normal restart/menu geçişleri döngüyü tekrar Play etmez; kısa miks geçişi ve eski kısa efekt temizliği yapılır. Mute/focus/native pause/disable kaynakları durdurur; dönüşte bir kez başlatılır. Eski altı runtime efekti SoundSynthesis üzerinden korunur.
