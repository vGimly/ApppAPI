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
        public virtual DbSet<Measure> Measures { get; set; } = null!;
        public virtual DbSet<Tariff> Tariffs { get; set; } = null!;
        public virtual DbSet<Usluga> Uslugas { get; set; } = null!;

        public object? Tbl(Type T) { switch(T.Name) {
                case nameof(Counter): return Counters;
                case nameof(Measure): return Measures;
                case nameof(Tariff): return Tariffs;
                case nameof(Usluga): return Uslugas;
                default: return null; } } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//		var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//              optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
			// Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.29-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.ToTable("counters");

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

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
