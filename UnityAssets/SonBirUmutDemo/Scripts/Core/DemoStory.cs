using SonBirUmutDemo.Decisions;

namespace SonBirUmutDemo.Core
{
    // Edit narrative here; no diagnosis, causal medical claim or automatic cure is modeled.
    public static class DemoStory
    {
        public const string Title = "Son Bir Umut";
        public const string Introduction = "Ailemin yaşadığı köye döndüğümde suyun azaldığını, toprağın kuruduğunu gördüm. " +
            "Babam da bir süredir rahatsız; nedenini henüz bilmiyoruz. Onun için duyduğum kaygı, köyde neler değiştiğini anlama isteğimi artırıyor.\n\n" +
            "Artan sıcaklıklar ve kuraklık köyün su kaynaklarını zorluyor. Önce enerji istasyonunu, sonra su deposunu ve son olarak tarlayla doğal alanı araştır. " +
            "Sınırlı kaynaklarla vereceğin üç karar birbirini etkileyecek.";
        public const string Controls = "WASD ile dolaş. İşaretli noktaya yaklaşınca E'ye bas. Üç karttan birini seç. Esc ile kartları kapatabilirsin.";
        public const string BalanceNote = "Değerler örnek oyun puanlarıdır. Kuraklık dışsal baskıdır; emisyon anlık sıcaklık artışı oluşturmaz.";
        public const string Closing = "Köy için attığın adımlar bir başlangıç. Başka kararlar, başka bir gelecek mümkün.";

        public static string PointName(string id)
        {
            if (id == DecisionCatalog.EnergyStationId) return "1. Enerji istasyonu";
            if (id == DecisionCatalog.WaterStationId) return "2. Su deposu";
            if (id == DecisionCatalog.FarmStationId) return "3. Tarım ve doğal alan";
            return "Karar noktası";
        }

        public static string Question(string id)
        {
            if (id == DecisionCatalog.EnergyStationId) return "Köyün enerjisini nasıl sağlayacaksın?";
            if (id == DecisionCatalog.WaterStationId) return "Enerjiyi ve bütçeyi su için nasıl kullanacaksın?";
            return "Kalan suyla hangi geleceğe öncelik vereceksin?";
        }
    }
}
