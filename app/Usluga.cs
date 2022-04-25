//using System;
//using System.Collections.Generic;

namespace NotesMinimalAPI.app
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

        public virtual ICollection<Counter> Counters { get; set; }
        public virtual ICollection<Tariff> Tariffs { get; set; }
        public void fill(usluga u, bool full) { if (full) UslugaId = u.usluga_id;  UslugaName = u.usluga_name; }
        public Usluga(usluga u) { fill(u, true); }
    }
    public partial struct usluga {
        public uint usluga_id { get; set; }
        public string usluga_name { get; set; }
        public usluga(Usluga value) { usluga_id = value.UslugaId; usluga_name = value.UslugaName; }
    }

}
