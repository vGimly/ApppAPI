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

        public virtual Usluga UslugaRefNavigation { get; set; } = null!;
        public virtual ICollection<Measure> Measures { get; set; }
        public Counter fill(counter value, bool full=true)
        {
            if (full) { 
                CounterId = value.counter_id;
                UslugaRef = value.usluga_ref;
            }
            CounterName = value.counter_name;
            Serial = value.serial;
            Digits = value.digits;
            Precise = value.precise;
            return this;
        }
        public Counter(counter c) : this() { fill(c, true); }

    }
    public partial struct counter
    {
        public uint counter_id { get; set; }
        public uint usluga_ref { get; set; }
        public string counter_name { get; set; } = null!;
        public string serial { get; set; } = null!;
        public byte digits { get; set; }
        public byte precise { get; set; }
        public counter(Counter value)
        {
            counter_id = value.CounterId;
            usluga_ref = value.UslugaRef;
            counter_name = value.CounterName;
            serial = value.Serial;
            digits = value.Digits;
            precise = value.Precise;
        }

    }
}
