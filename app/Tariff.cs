using System.Text.Json.Serialization;

namespace ApppAPI.app
{
    public partial class Tariff
    {
        public uint TariffId { get; set; }
        public uint UslugaRef { get; set; }
        public DateTime TDate { get; set; }
        public decimal Price { get; set; }
        
        [JsonIgnore]
        public DateTime Alt { get; set; }
        
        [JsonIgnore]
        public virtual Usluga UslugaRefNavigation { get; set; } = null!;
	public static Tariff operator ^ (Tariff a, Tariff b){ a.TDate=b.TDate; a.Price=b.Price; return a; }
    }
}
