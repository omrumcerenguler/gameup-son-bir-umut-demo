using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;

namespace SonBirUmutDemo.Core
{
    // Central narrative only; no NPC, illness or separate story quest.
    public static class DemoStory
    {
        public const string Title = "Son Bir Umut: Kuraklıktan Önce";
        public const string MainGoal = "Kuraklık gelmeden köyün temel ihtiyaçlarını koru.";
        public const string Introduction = "Yıllar sonra köyüme döndüm. Yağışlar azalmış, su deposu boşalmaya başlamış. " +
            "Önümüzdeki haftalarda daha ağır bir kuraklık bekleniyor.\n\n" +
            "Ailemin ve komşularımızın geleceği için enerji, su ve doğal yaşam hakkında üç karar vermeliyim. " +
            "Ama bir sorunu çözmek, başka bir kaynağı tüketebilir.\n\n" + MainGoal;
        public const string Controls = "WASD ile dolaş, Sol Shift ile koş. İşaretli noktada E'ye bas. Üç karttan birini seç; Esc ile kapat. Kart açıkken hareket ve sayaç durur.";
        public const string ReleaseTitle = "SON BİR UMUT:\nKURAKLIKTAN ÖNCE";
        public const string ReleaseSubtitle = "KRİZ NÖBETİ";
        public const string CrisisIntroduction = "Azalan yağışlar köyün su ve yaşam kaynaklarını zorluyor. " +
            "Enerji, su ve tarım kararlarını ver; altyapıyı onar ve kriz baskısını düşür.";
        public static string CrisisGoal(GameRunState run) =>
            $"{run.Crisis.Tasks.Count} bakım görevinden en az {run.Crisis.RequiredCount} tanesini tamamla. Su hattındaki arızaya müdahale et; " +
            $"baskıyı {run.CrisisRules.StableThreshold:0} veya altında {run.CrisisRules.StableDuration:0} saniye koru.";
        public static string HowTo(GameRunState run) => "WASD ile yürü, Sol Shift ile koş.\n\n" +
            "Enerji → su → tarım noktalarında E ile karar ver.\n\n" +
            $"Arızada depoya koş; E'yi {run.CrisisRules.RepairDuration:0} saniye basılı tut. Bırakırsan onarım sıfırlanır.\n\n" +
            CrisisGoal(run) + "\n\nKalan süre üst şeritte. Esc ile duraklat.";
        public static string EndingBrief(EndingCategory category, bool stabilized)
        {
            if (category == EndingCategory.Rebirth) return "Köyün temel ihtiyaçlarını korudun. Kurak döneme daha dayanıklı giriyorsunuz.";
            if (category == EndingCategory.PartialTransition) return stabilized ?
                "Nöbeti dengeledin; köyün uzun vadeli dönüşümünde hâlâ açıklar var." :
                "Bazı ihtiyaçlar korundu, ancak kriz nöbeti istikrara kavuşmadı.";
            return stabilized ? "Nöbeti dengeledin, ancak kalan kaynaklar kurak döneme yetmedi." :
                "Su ve bitki yaşamı kuraklığa hazır değil. Başka kararlarla yeniden deneyebilirsin.";
        }
        public const string FoundationIntroduction = "Köyün kuraklığa hazırlığı için enerji, su ve tarım hakkında üç karar ver. " +
            "Bu temel sahnede aktif nöbet süresi ve kriz baskısını izleyebilirsin. Kart açıkken sayaç durur. Süre sonunda mevcut hazırlığın değerlendirilir.";
        public const string CrisisControls = "WASD yürü • Sol Shift koş • E karar/bakım • Arızada E basılı tut. " +
            "Bırakmak veya uzaklaşmak onarımı sıfırlar; onarım sırasında sayaç işler.";
        public const string BalanceNote = "Değerler örnek oyun puanlarıdır. Kuraklık dışsal baskıdır; emisyon anlık sıcaklık artışı oluşturmaz.";
        public const string Closing = "Köy için attığın adımlar bir başlangıç. Başka kararlar, başka bir gelecek mümkün.";

        public static string CardBenefit(string id)
        {
            switch (id)
            {
                case DecisionCatalog.Solar: return "Temiz enerji kapasitesi +35";
                case DecisionCatalog.Diesel: return "Güçlü enerji kapasitesi +50";
                case DecisionCatalog.Conservation: return "8 suyu korur, yatırım istemez";
                case DecisionCatalog.Pump: return "Enerjin kadar su kazan (en çok 50)";
                case DecisionCatalog.Efficiency: return "18 su kazan, tüketimi yarıya indir";
                case DecisionCatalog.Continue: return "Bütçeyi sonraki karara sakla";
                case DecisionCatalog.Protect: return "Suya bağlı doğal yaşam desteği";
                case DecisionCatalog.Irrigate: return "Bitkilere hızlı sulama desteği";
                default: return "Kurak dönem için suyu koru";
            }
        }
        public static string CardCost(string id)
        {
            switch (id)
            {
                case DecisionCatalog.Solar: return "Yüksek ilk yatırım";
                case DecisionCatalog.Diesel: return "3 su, emisyon +18";
                case DecisionCatalog.Conservation: return "Enerji kapasitesi -5";
                case DecisionCatalog.Pump: return "En az 10 enerji gerekir";
                case DecisionCatalog.Efficiency: return "Pompadan az su kazanımı";
                case DecisionCatalog.Continue: return "Kaçaklardan 5 su kaybı";
                case DecisionCatalog.Protect: return "Yeni su üretmez";
                case DecisionCatalog.Irrigate: return "20 su harcar";
                default: return "Sulamayı erteler";
            }
        }

        public static string ShortPointName(string id)
        {
            if (id == DecisionCatalog.EnergyStationId) return "Enerji istasyonu";
            if (id == DecisionCatalog.WaterStationId) return "Su deposu";
            return "Tarım alanı";
        }

        public static string PointName(string id)
        {
            if (id == DecisionCatalog.EnergyStationId) return "1. Enerji istasyonu";
            if (id == DecisionCatalog.WaterStationId) return "2. Su deposu";
            if (id == DecisionCatalog.FarmStationId) return "3. Tarım ve doğal alan";
            return "Karar noktası";
        }

        public static string EventTitle(string id)
        {
            if (id == DecisionCatalog.EnergyStationId) return "Elektrik mi, kaynak mı?";
            if (id == DecisionCatalog.WaterStationId) return "Depo tükeniyor";
            return "Son su payı";
        }

        public static string Question(string id)
        {
            if (id == DecisionCatalog.EnergyStationId) return "Köyün enerjisini nasıl sağlayacaksın?";
            if (id == DecisionCatalog.WaterStationId) return "Enerjiyi ve bütçeyi su için nasıl kullanacaksın?";
            return "Kalan suyla hangi geleceğe öncelik vereceksin?";
        }

        public static string Problem(string id)
        {
            if (id == DecisionCatalog.EnergyStationId) return "Su pompaları enerjiye ihtiyaç duyuyor; kapasiteyi artırırken bütçeyi, suyu ve emisyonu da düşünmelisin.";
            if (id == DecisionCatalog.WaterStationId) return "Depo azalıyor; mevcut enerji ve bütçeyle tarıma yetecek suyu nasıl koruyacaksın?";
            return "Kalan su hem bugünün ürünlerine hem kurak dönemin rezervine gerekiyor; hangi ihtiyaca öncelik vereceksin?";
        }
    }
}
