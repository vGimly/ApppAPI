using Microsoft.EntityFrameworkCore;

namespace ApppAPI.app
{
    public partial class appContext : DbContext
    {
        public appContext()
        {
        }

        public appContext(DbContextOptions<appContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Counter> Counters { get; set; } = null!;
        public virtual DbSet<Counter_with_initials> Counter2 { get; set; } = null!;
        public virtual DbSet<Measure> Measures { get; set; } = null!;
        public virtual DbSet<Tariff> Tariffs { get; set; } = null!;
        public virtual DbSet<Usluga> Uslugas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            { throw new Exception("Must be configured in Program.cs"); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.ToTable("counters")
                    .Ignore(e => e.Max)
                    .Ignore(e => e.InitValue)
                    .Ignore(e => e.StartDate);

                entity.HasIndex(e => new { e.CounterName, e.Serial }, "counter_name_serial")
                    .IsUnique()
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 128, 64 });

                entity.HasIndex(e => e.UslugaRef, "tariff_ref");

                entity.Property(e => e.CounterId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("counter_id");

                entity.Property(e => e.Alt)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("alt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CounterName)
                    .HasColumnType("text")
                    .HasColumnName("counter_name");

                entity.Property(e => e.Digits)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasColumnName("digits");

                entity.Property(e => e.Precise)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasColumnName("precise");

                entity.Property(e => e.Serial)
                    .HasColumnType("text")
                    .HasColumnName("serial");

                entity.Property(e => e.UslugaRef)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("usluga_ref");

                entity.HasOne(d => d.UslugaRefNavigation)
                    .WithMany(p => p.Counters)
                    .HasForeignKey(d => d.UslugaRef)
                    .OnDelete(DeleteBehavior.ClientNoAction)
                    .HasConstraintName("usluga");
            });

            modelBuilder.Entity<Counter_with_initials>(entity =>
            {
                entity.ToTable("counter_with_initials");

                entity.HasIndex(e => new { e.CounterName, e.Serial }, "counter_name_serial")
                    .IsUnique()
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 128, 64 });

                entity.HasIndex(e => e.UslugaRef, "tariff_ref");

                entity.Property(e => e.CounterId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("counter_id");

                entity.Property(e => e.Alt)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("alt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CounterName)
                    .HasColumnType("text")
                    .HasColumnName("counter_name");

                entity.Property(e => e.Digits)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasColumnName("digits");

                entity.Property(e => e.Precise)
                    .HasColumnType("tinyint(3) unsigned")
                    .HasColumnName("precise");

                entity.Property(e => e.Serial)
                    .HasColumnType("text")
                    .HasColumnName("serial");

                entity.Property(e => e.UslugaRef)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("usluga_ref");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date");

                entity.Property(e => e.InitValue)
                    .HasColumnType("decimal(20,6) unsigned")
                    .HasColumnName("init_value");

            });

            modelBuilder.Entity<Measure>(entity =>
            {
                entity.HasKey(e => e.MId)
                    .HasName("PRIMARY");

                entity.ToTable("measures");

                entity.HasIndex(e => e.CounterRef, "counter_ref");

                entity.HasIndex(e => new { e.CounterRef, e.MDate }, "counter_ref_m_date")
                    .IsUnique();

                entity.Property(e => e.MId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("m_id");

                entity.Property(e => e.Alt)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("alt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CounterRef)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("counter_ref");

                entity.Property(e => e.MDate)
                    .HasColumnName("m_date");

                entity.Property(e => e.Value)
                    .HasColumnType("decimal(20,6) unsigned")
                    .HasColumnName("value");

                entity.HasOne(d => d.CounterRefNavigation)
                    .WithMany(p => p.Measures)
                    .HasForeignKey(d => d.CounterRef)
                    .OnDelete(DeleteBehavior.ClientNoAction)
                    .HasConstraintName("counter");
            });

            modelBuilder.Entity<Tariff>(entity =>
            {
                entity.ToTable("tariff");

                entity.HasIndex(e => new { e.UslugaRef, e.TDate }, "usluga_ref_t_date")
                    .IsUnique();

                entity.Property(e => e.TariffId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("tariff_id");

                entity.Property(e => e.Alt)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("alt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(20,6) unsigned")
                    .HasColumnName("price");

                entity.Property(e => e.TDate)
                    .HasColumnName("t_date");

                entity.Property(e => e.UslugaRef)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("usluga_ref");

                entity.HasOne(d => d.UslugaRefNavigation)
                    .WithMany(p => p.Tariffs)
                    .HasForeignKey(d => d.UslugaRef)
                    .OnDelete(DeleteBehavior.ClientNoAction)
                    .HasConstraintName("usluga1");
            });

            modelBuilder.Entity<Usluga>(entity =>
            {
                entity.ToTable("usluga");

                entity.HasKey(e => e.UslugaId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.UslugaName, "usluga_name")
                    .IsUnique()
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 64 });

                entity.Property(e => e.UslugaId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("usluga_id");

                entity.Property(e => e.Alt)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("alt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UslugaName)
                    .HasColumnType("text")
                    .HasColumnName("usluga_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        
// Нельзя изменить разрядность счётчика, если уже есть показания больше
// [По-хорошему вообще счётчик менять нельзя - но мы суперадмин]
        internal async Task<bool> Verify(Counter t)
        {
            return await Measures.Where(m => m.CounterRef == t.CounterId).MaxAsync(m => m.Value).ContinueWith(a => a.Result >= t.Max );
        }

        // Нельзя добавить (изменить можно) дату измерения ДО начального показания
        // Нельзя добавить (или изменить) дату измерения ПОСЛЕ сегодня
        // Нельзя добавить (или изменить) дату измерения когда такая дата уже есть (уникальный индекс SQL не позволит - пропускаем?) (при изменении найденная дата может быть - мы)
        // Нельзя добавить показание вне диапазона по датам. [ (Д1,И1) (Д,И) (Д2,И2) - если Д > Д1, Д < Д2, то И >= И1 и И <= И]
        // Нельзя добавить (или изменить) показание вне разрядности счётчика.

        /* cmpd cmpv isme 
         *   0    ~    f   > DateSame
         *  -1    1    f   > BiggerThenPrev
         *   1   -1    f   > LowerThenNext
         * 
         * 
         * a c R  
         * 0 0 0
         * 0 1 1
         * 1 0 0
         * 1 1 0
         * !a && c

         * c && (n.isMe || ((n.cmpd != 0) && (n.cmpd * n.cmpv >= 0)))
         * c && n.isMe || c && n.cmpd != 0 && n.cmpd * n.cmpv >= 0
         */

        internal void Verify(Measure t)
        {
            if (t.CounterRefNavigation is null)
                t.CounterRefNavigation = Counters.Where(c => c.CounterId == t.CounterRef).FirstOrDefault();
            t.Verify();

            var q = Measures.Where(m => m.CounterRef == t.CounterRef)
                .OrderBy(m => m.MDate)
                .Select(m => new { isMe = t.MId == m.MId, cmpd = t.MDate.CompareTo(m.MDate), cmpv = t.Value.CompareTo(m.Value), m.MDate, m.Value });

            var f = q.First();
            if (f.cmpd < 0 && (t.MId == 0 || !f.isMe))
                throw new Exception($"Попытка добавления измерения {t.MDate:d} до его ввода в эксплуатацию {f.MDate:d}");
            foreach (var n in q)
            {
                if (n.isMe) continue;
                if (n.cmpd == 0)
                        throw new Exception($"Попытка добавления измерения: На дату {t.MDate:d} показания уже есть");
                if (n.cmpd < 0 && n.cmpv > 0)
                    throw new Exception($"Попытка добавления измерения: Для даты {t.MDate:d} есть ранние показания {n.MDate:d}/{n.Value} < {t.Value}");
                if (n.cmpd > 0 && n.cmpv < 0)
                    throw new Exception($"Попытка добавления измерения: Для даты {t.MDate:d} есть последующие показания {n.MDate:d}/{n.Value} > {t.Value}");
            }
            //            if (!q.ToList().Aggregate(true, (c, n) => c && n.isMe || c && n.cmpd != 0 && n.cmpd * n.cmpv >= 0))
            //                throw new Exception($"Попытка добавления измерения {t.MDate} вне условий");
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
