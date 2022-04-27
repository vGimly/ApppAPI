using System.Text.Json.Serialization;

namespace ApppAPI.app
{
    public partial class Usluga
    {
        public Usluga()
        {
            Counters = new HashSet<Counter>();
            Tariffs = new HashSet<Tariff>();
        }
        
        public uint UslugaId { get; set; }
        public string UslugaName { get; set; } = null!;
        
        [JsonIgnore]
        public DateTime Alt { get; set; }

        [JsonIgnore] 
        public virtual ICollection<Counter> Counters { get; set; } = null!;
        
        [JsonIgnore] 
        public virtual ICollection<Tariff> Tariffs { get; set; } = null!;
	public static Usluga operator ^ (Usluga a, Usluga b){a.UslugaName=b.UslugaName;return a;}
    }
}
