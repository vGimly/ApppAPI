using System.Text.Json.Serialization;

namespace ApppAPI.app
{
    public partial class Measure
    {
        public uint MId { get; set; }
        public uint CounterRef { get; set; }
        public DateTime MDate { get; set; }
        public decimal Value { get; set; }
        
        [JsonIgnore]
        public DateTime Alt { get; set; }
        
        [JsonIgnore]
        public virtual Counter CounterRefNavigation { get; set; } = null!;
        public static Measure operator ^ (Measure a, Measure b) { a.MDate=b.MDate; a.Value=b.Value; return a; }
    }
}
