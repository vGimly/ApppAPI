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
        public DateTime Alt { get; set; }

        public virtual ICollection<Counter> Counters { get; set; } = null!;
        public virtual ICollection<Tariff> Tariffs { get; set; } = null!;
        public Usluga fill(usluga u, bool full=true) {
            if (full) UslugaId = u.usluga_id;  UslugaName = u.usluga_name; return this; }
        public Usluga(usluga u) : this() { fill(u, true); }
    }
    public partial struct usluga {
        public uint usluga_id { get; set; }
        public string usluga_name { get; set; }
        public usluga(Usluga value) { usluga_id = value.UslugaId; usluga_name = value.UslugaName; }
    }

}
 