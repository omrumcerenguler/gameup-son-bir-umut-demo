using SonBirUmutDemo.Decisions;

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
        public const string CrisisIntroduction = "Kuraklık köyün kaynaklarını zorluyor. Enerji, su ve tarım noktalarını sırayla araştır; üç stratejik karar ver.\n\n" +
            "Bu deneysel altyapıda sayaç yalnız serbest dolaşırken işler. Üç kararın ardından kriz aşamasında köyü inceleyebilirsin. Süre bitince hazırlığının sonucu gösterilir.\n\n" +
            "Bakım müdahaleleri ve acil arıza sonraki aşamada eklenecek. Kriz baskısı gerçek iklim ölçümü değildir.";
        public const string BalanceNote = "Değerler örnek oyun puanlarıdır. Kuraklık dışsal baskıdır; emisyon anlık sıcaklık artışı oluşturmaz.";
        public const string Closing = "Köy için attığın adımlar bir başlangıç. Başka kararlar, başka bir gelecek mümkün.";

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
