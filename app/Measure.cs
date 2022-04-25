namespace ApppAPI.app
{
    public partial class Measure
    {
        public Measure() { }
        public uint MId { get; set; }
        public uint CounterRef { get; set; }
        public DateOnly MDate { get; set; }
        public decimal Value { get; set; }
        public DateTime Alt { get; set; }
        public virtual Counter CounterRefNavigation { get; set; } = null!;
        public Measure fill(measure m, bool full=true)
        {
            if (full) {
                MId = m.m_id;
                CounterRef = m.counter_ref;
            }
            MDate = DateOnly.FromDateTime(m.m_date);
            Value = m.value;
            return this;
        }
        public Measure(measure m) : this() { fill(m, true); }
    }
    public partial struct measure
    {
        public uint m_id { get; set; }
        public uint counter_ref { get; set; }
        public DateTime m_date { get; set; }
        public decimal value { get; set; }
        public measure (Measure val)
        {
            m_id = val.MId;
            counter_ref= val.CounterRef;
            value = val.Value;
            m_date = val.MDate.ToDateTime(TimeOnly.MinValue);
        }
    }

}
