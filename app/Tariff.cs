namespace ApppAPI.app
{
    public partial class Tariff
    {
        public Tariff() { }
        public uint TariffId { get; set; }
        public uint UslugaRef { get; set; }
        public DateOnly TDate { get; set; }
        public decimal Price { get; set; }
        public DateTime Alt { get; set; }
        public virtual Usluga UslugaRefNavigation { get; set; } = null!;
        internal Tariff fill(tariff t, bool full = false) { UslugaRef = t.usluga_ref; TDate = DateOnly.FromDateTime(t.t_date); Price = t.price;
            if (full) TariffId = t.tariff_id;
            return this;
        }
        public Tariff(tariff u) : this() { fill(u, true); }

    }
    public partial struct tariff
    {
        public uint tariff_id { get; set; }
        public uint usluga_ref  { get; set; }
        public DateTime t_date { get; set; }
        public decimal price { get; set; }
        public tariff(Tariff value) { tariff_id = value.TariffId; usluga_ref = value.UslugaRef; price = value.Price;
            t_date = value.TDate.ToDateTime(TimeOnly.MinValue); }
    }

}
