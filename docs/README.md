# [G-Helper (GHelper)](https://github.com/seerge/g-helper)

![https://img.shields.io/github/downloads/seerge/g-helper/total.svg](https://img.shields.io/github/downloads/seerge/g-helper/total.svg)

![https://img.shields.io/github/release/seerge/g-helper.svg](https://img.shields.io/github/release/seerge/g-helper.svg)

[https://badgen.net/github/license/seerge/g-helper](https://badgen.net/github/license/seerge/g-helper)

![https://img.shields.io/github/stars/seerge/g-helper.svg?style=social&label=Star&maxAge=2592000](https://img.shields.io/github/stars/seerge/g-helper.svg?style=social&label=Star&maxAge=2592000)

## Asus ROG Zephyrus G14, G15, Flow X13, Flow X16 ve diğer modeller için açık kaynaklı Armory Crate alternatifi

Fazladan yük ve gereksiz hizmetler olmadan Armory Crate ile yapabileceğiniz neredeyse her şeyi yapmanızı sağlayan küçük bir yardımcı program.

### :gift: Temel avantajları

1. Sorunsuz ve otomatik GPU geçişleri (tüm uygulamaların kapatılmasını falan istemeden)
2. Tüm performans modları tamamen özelleştirilebilir (fan eğrileri ve PPT'lerle birlikte)
3. Çok hafif ve neredeyse hiç kaynak tüketmiyor, sadece çalıştırılacak tek bir exe dosyası

### [:floppy_disk: En son sürümü indirin](https://github.com/seerge/g-helper/releases/latest/download/GHelper.zip)

Bu uygulamayı beğendiyseniz, lütfen Github'da :star: ile işaretleyin ve arkadaşlarınıza bahsedin!

![https://raw.githubusercontent.com/seerge/g-helper/main/docs/screenshot.png](https://raw.githubusercontent.com/seerge/g-helper/main/docs/screenshot.png)

### :zap: Temel özellikler

1. Dahili **Performans modları**: Sessiz - Dengeli - Turbo (varsayılan fan eğrileriyle)
2. **GPU modları**: Eco - Standard - Ultimate
3. Dizüstü bilgisayar ekran yenileme hızı 60hz veya 120hz (modelin bağlı olarak 144hz vb. olabilir) ile display overdrive (OD)
4. Her performans modu için varsayılan ve özel fan profilleri
5. Her performans modu için güç sınırları (PPT)
6. CPU turbo boost modu
7. Klavye arkadan aydınlatmalı animasyon ve renkler
8. Benim tarafımdan bazı düzenlemelerle [Starlight](https://github.com/vddCore/Starlight) sayesinde Anime matrix kontrolü (animasyonlu GIF'leri içerir)
9. FN+F5 performans modlarını döndürür, FN+F4 klavye animasyon modlarını döndürür
10. M3 ve M4 tuşları için tuş atamaları
11. Pil sağlığını korumak için pil şarj sınırı
12. CPU / GPU sıcaklığını, fan hızlarını ve batarya boşalma hızını izleyin

### :apple: Şarjda veya prize takılıyken modların otomatik olarak değiştirilmesi

- Performans modları (uygulama, son kullanılan modu veya prize takılıyken kullandığı son modu hatırlar)
- GPU modları (pilde ekonomi, prize takılıyken standart)
- Ekran yenileme hızı (pilde 60hz, prize takılıyken 120+ hz)

Otomatik geçişleri ve kısayol tuşlarını çalışır durumda tutmak için uygulamanın arka planda çalışmaya devam etmesi gerekiyor. Herhangi bir kaynak tüketmez.

### :rocket: Performans Modları

Modlar, bios'ta depolandığı gibi Armory Crate ile aynıdır, varsayılan fan eğrileriyle birlikte

1. Sessiz (minimal veya hiç fan açmaz, toplam 70W PPT, CPU'ya kadar 45W PPT) + Windows'taki en iyi güç verimliliği ayarı
2. Dengeli (dengeli fanlar, toplam 100W PPT, CPU'ya kadar 45W PPT) + Windows'ta Dengeli ayarı
3. Turbo (yoğun fanlar, toplam 125W PPT, CPU'ya kadar 80W PPT) + Windows'ta En İyi Performans ayarı

Diğer modeller için PPT'ler G14 2022 için gösterilir, çünkü bios'ta ayarlanmışlardır.

### :video_game: GPU Modları

1. Eco mod: sadece düşük güç tüketimli entegre GPU etkin, iGPU ekrana görüntüyü verir
2. Standart mod (Windows Hybrid): iGPU ve dGPU etkin, iGPU yerleşik ekrana görüntü verir. dGpu çalışır. Windows'ta "Windows Hybrid" ayarı 
3. Ultimate mod: iGPU ve dGPU etkin, ancak dGPU yerleşik ekrana sürer (yalnızca 2022 modellerinde desteklenir)

### :question: SSS

### M4 / Rog tuşuna her bastığımda Armory Crate kurulumu açılış penceresi nasıl durdurulur?

BIOS'a gidin (açılışta F2), Gelişmiş Ayarlar'ı (F8) açın ve "Armory Control Interface"yi devre dışı bırakın

### Neden Ultimate GPU modu benim dizüstü bilgisayarımda mevcut değil?

Ultimate modu, yalnızca 2022'de (ve muhtemelen 2022+ diğer modellerde) donanım tarafından destekleniyor

### Uygulama başlatılmıyor / veya çöktü, ne yapmalıyım?

Başlat menüsünden "Olay Görüntüleyicisi"ni açın, Windows Günlükleri -> Uygulama'ya gidin ve G-Helper'ı belirten son Hataları kontrol edin. Bir tane görürseniz, lütfen bu hatanın tüm ayrıntıları ile birlikte bir [yeni konu](https://github.com/seerge/g-helper/issues) açın.

---

### Nasıl yüklenir

1. **[Sürümler sayfasından](https://github.com/seerge/g-helper/releases)** en son sürümü indirin
2. Seçtiğiniz bir klasöre çıkartın
3. **GHelper.exe**'yi çalıştırın

### Bağımlılıklar

- Uygulama için [.NET7](https://dotnet.microsoft.com/en-us/download) yüklü olmalıdır. Muhtemelen zaten yüklüdür. Aksi takdirde [buradan indirebilirsiniz](https://dotnet.microsoft.com/en-us/download).
- Temel dizüstü bilgisayar kısayol tuşlarını (örneğin ekran veya klavye parlaklığı ayarlamaları) çalışır durumda tuttuğu için "Asus Optimization Service" çalışır durumda bırakmanızı öneriyorum. MyASUS uygulaması yüklediyseniz (veya yüklemişseniz) bu hizmet muhtemelen MyASUS kaldırıldıktan sonra bile hala çalışıyor olacaktır. Bu, [Asus Sistem Kontrol Arayüzü](https://www.asus.com/support/FAQ/1047338/)nün bir parçasıdır. Kurabilir ve daha sonra gereksiz hizmetleri devre dışı bırakabilir / kaldırabilirsiniz [bu bat dosyasını](https://raw.githubusercontent.com/seerge/g-helper/main/debloat.bat) yönetici olarak çalıştırarak gereksiz her hizmeti durdurabilirsiniz.
- Bu uygulamayı Armory Crate ile birlikte kullanmanız önerilmez. Kendi kaldırma aracını kullanarak [kendi kaldırma aracını kullanarak](https://dlcdnets.asus.com/pub/ASUS/mb/14Utilities/Armoury_Crate_Uninstall_Tool.zip?model=armoury%20crate) kaldırabilirsiniz. Her ihtimale karşı, her zaman geri yükleyebilirsiniz.

Not: Çalıştırmak için yönetici izinlerine ihtiyaç yoktur!

---

Asus Zephyrus G14 2022 (AMD Radeon iGPU ve dGPU ile) için tasarlanmış ve geliştirilmiştir. Ancak ilgili ve desteklenen özellikler için G14 2021 ve 2020, G15, X FLOW ve diğer ROG modelleri için potansiyel olarak çalışabilir ve çalışmalıdır.

Henüz uygulamayı imzalamak için bir Microsoft sertifikam yok, bu nedenle Windows Defender'dan bir uyarı alırsanız (Windows PC'nizi korudu), Daha Fazla Bilgi -> Yine çalıştır'ı tıklayın. Alternatif olarak, Visual Studio kullanarak projeyi kendiniz derleyebilir ve çalıştırabilirsiniz :)

Ayarlar dosyası `%AppData%\\GHelper`'da saklanır

---

Debloating (.bat dosyasını çalıştırmak), pilinizi korumaya ve dizüstü bilgisayarınızı biraz daha serin tutmanıza yardımcı olur

![https://raw.githubusercontent.com/seerge/g-helper/main/docs/screenshots/screen-5w.png](https://raw.githubusercontent.com/seerge/g-helper/main/docs/screenshots/screen-5w.png)

---

**Reddetmeler**
"ROG", "TUF" ve "Armoury Crate", AsusTek Computer, Inc.'e ait ticari markalardır. Bunlara veya AsusTek Computer'a ait diğer varlıklarla ilgili hiçbir iddiam yoktur. Bunları yalnızca bilgilendirme amaçlı kullanıyorum.

YAZILIM "OLDUĞU GİBİ" SUNULMAKTADIR, HERHANGİ BİR TÜRDE, AÇIK VEYA ZIMNİ GARANTİ OLMADAN, SATILABİLİRLİK, BELİRLİ BİR AMACA UYGUNLUK VE İHLAL ETMEME GARANTİLERİ DE DAHİLDİR ANCAK BUNLARLA SINIRLI OLMAMAKTADIR, BU YAZILIMIN KULLANIMI SİSTEM İSTİKRARSIZLIĞINA VEYA ARIZASINA NEDEN OLABİLİR. SORUMLULUK SİZE AİTTİR.
