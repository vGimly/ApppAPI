using System.Text.Json.Serialization;

namespace ApppAPI.app
{
    public partial class Counter
    {
        public Counter()
        {
            Measures = new HashSet<Measure>();
        }

        public uint CounterId { get; set; }
        public uint UslugaRef { get; set; }
        public string CounterName { get; set; } = null!;
        public string Serial { get; set; } = null!;
        public byte Digits { get; set; }
        public byte Precise { get; set; }
        
        [JsonIgnore]
        public DateTime Alt { get; set; }

        //        [JsonIgnore]
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        //        [JsonIgnore]
        public decimal InitValue { get; set; } = 0;
        
        [JsonIgnore]
        public virtual Usluga UslugaRefNavigation { get; set; } = null!;
        
        [JsonIgnore]
        public virtual ICollection<Measure> Measures { get; set; }
        
        public static Counter operator ^(Counter a, Counter b)
        {
            a.CounterName = b.CounterName;
            a.Serial = b.Serial;
            a.Digits = b.Digits;
            a.Precise = b.Precise;
            return a;
        }

        [JsonIgnore]
        public decimal Max { get { return (decimal)Math.Pow(10, (double)Digits); } }

        public void Verify()
        {
            if (Digits < 1 || Digits > 10)
                throw new Exception($"Неверное значение Разрядности: {Digits} надо между 1 и 10");

            if (Precise < 0 || Precise > 10)
                throw new Exception($"Неверное значение Точности: {Precise} надо между 0 и 10");

            foreach (Measure m in Measures) m.Verify();
        }
    }
    public partial class Counter_with_initials : Counter
    {
    }
}
