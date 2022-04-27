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
        public DateTime Alt { get; set; }

        public DateTime StartDate { get; set; }
        public decimal InitValue { get; set; }
        public virtual Usluga UslugaRefNavigation { get; set; } = null!;
        public virtual ICollection<Measure> Measures { get; set; }
        public static Counter operator ^(Counter a, Counter b)
        {
            a.CounterName = b.CounterName;
            a.Serial = b.Serial;
            a.Digits = b.Digits;
            a.Precise = b.Precise;
            return a;
        }
    }
}
