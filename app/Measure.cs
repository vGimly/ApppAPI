namespace ApppAPI.app
{
    public partial class Measure
    {
        public uint MId { get; set; }
        public uint CounterRef { get; set; }
        public DateOnly MDate { get; set; }
        public decimal Value { get; set; }
        public DateTime Alt { get; set; }
        public virtual Counter CounterRefNavigation { get; set; } = null!;
        public static Measure operator ^ (Measure a, Measure b) { a.MDate=b.MDate; a.Value=b.Value; return a; }
    }
}
