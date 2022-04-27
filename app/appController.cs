using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ApppAPI.app
{
    public static partial class appController
    {
        internal static CultureInfo provider = new CultureInfo("en-US");

    #region Usluga
        public static async Task<List<Usluga>> GetUslugi(appContext db)
        {
            return await db.Uslugas.ToListAsync();
        }

        public static async Task<IResult> GetUsluga(uint usl, appContext db)
        {
            return await db.Uslugas.FindAsync(usl)
                    is Usluga obj
                        ? Results.Ok(obj)
                        : Results.NotFound();
        }

        public static async Task<IResult> UpdUsluga(uint usl, Usluga u, appContext db)
        {
            var obj = await db.Uslugas.FindAsync(usl);
            if (obj is null) return Results.NotFound();
            obj ^= u;
            int changed = await db.SaveChangesAsync();
            return Results.Text("OK" + changed);
        }

        public static async Task<IResult> DelUsluga(uint usl, appContext db)
        {
            var obj = await db.Uslugas.FindAsync(usl);
            int changed = 0;
            if (obj is not null)
            {
                db.Uslugas.Remove(obj);
                changed = await db.SaveChangesAsync();
            }
            return Results.Text("OK" + changed);
        }

        public static async Task<IResult> AddUsluga(Usluga u, appContext db)
        {
            db.Uslugas.Add(u);
            await db.SaveChangesAsync();
            return Results.Created($"/usluga/{u.UslugaId}", new { OK = u.UslugaId });
        }

    #endregion

    #region Tariff
        internal static async Task<IResult> GetTariffByDate(uint usl, DateTime DTime, appContext db)
        {
          DateOnly date = DateOnly.FromDateTime(DTime);
            try
            {
                return Results.Ok(await db.Tariffs
                    .Where(t => t.UslugaRef == usl && t.TDate <= date)
                    .OrderByDescending(t => t.TDate)
                    .Select(obj => obj.Price)
                    .FirstAsync());
            }
            catch (Exception) { return Results.Text(""); }
        }

        public static async Task<List<Tariff>> GetTariffs(appContext db)
        {
            return await db.Tariffs.ToListAsync();
        }

        public static async Task<IResult> GetTariffsByUsluga(uint usl, appContext db)
        {
            return Results.Ok(await db.Tariffs.Where(obj => obj.UslugaRef == usl).ToListAsync());
        }

        public static async Task<IResult> GetTariff(uint id, appContext db)
        {
            return await db.Tariffs.FindAsync(id)
                    is Tariff obj
                        ? Results.Ok(obj)
                        : Results.NotFound();
        }

        public static async Task<IResult> UpdTariff(uint id, Tariff t, appContext db)
        {
            var obj = await db.Tariffs.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj ^= t;
            int changed = await db.SaveChangesAsync();
            return Results.Text("OK" + changed);
        }

        public static async Task<IResult> DelTariff(uint id, appContext db)
        {
            var obj = await db.Tariffs.FindAsync(id);
            int changed = 0;
            if (obj is not null)
            {
                db.Tariffs.Remove(obj);
                changed = await db.SaveChangesAsync();
            }
            return Results.Text("OK" + changed);
        }

        public static async Task<IResult> AddTariff(uint usl, Tariff t, appContext db)
        {
            db.Tariffs.Add(t);
            await db.SaveChangesAsync();
            return Results.Created($"/tariff/{t.TariffId}", new { OK = t.TariffId });
        }
    #endregion

    #region Counter
        public static async Task<List<Counter>> GetCounters(appContext db)
        {
            return await db.Counters.ToListAsync();
        }

        public static async Task<IResult> GetCountersByUsluga(uint usl, appContext db)
        {
            return Results.Ok(await db.Counters.Where(obj => obj.UslugaRef == usl).ToListAsync());
        }

        public static async Task<IResult> GetCounter(uint id, appContext db)
        {
            return await db.Counters.FindAsync(id)
                    is Counter obj
                        ? Results.Ok(obj)
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdCounter(uint id, Counter cnt, appContext db)
        {
            var obj = await db.Counters.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj ^= cnt;
            int changed = await db.SaveChangesAsync();
            return Results.Text("OK" + changed);
        }

        public static async Task<IResult> DelCounter(uint id, appContext db)
        {
            var obj = await db.Counters.FindAsync(id);
            int changed = 0;
            if (obj is not null)
            {
                db.Counters.Remove(obj);
                changed = await db.SaveChangesAsync();
            }
            return Results.Text("OK" + changed);
        }

        public static async Task<IResult> AddCounter(uint usl, Counter cnt, appContext db)
        {
            db.Counters.Add(cnt);
            await db.SaveChangesAsync();
            return Results.Created($"/counter/{cnt.CounterId}", new { OK = cnt.CounterId });
        }
    #endregion

    #region Measure
        public static async Task<List<Measure>> GetMeasures(appContext db)
        {
            return await db.Measures.ToListAsync();
        }

        public static async Task<IResult> GetMeasuresByCounter(uint cnt, appContext db)
        {
            return Results.Ok(await db.Measures.Where(obj => obj.CounterRef == cnt).ToListAsync());
        }

        public static async Task<IResult> GetMeasure(uint id, appContext db)
        {
            return await db.Measures.FindAsync(id)
                    is Measure obj
                        ? Results.Ok(obj)
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdMeasure(uint id, Measure m, appContext db)
        {
            var obj = await db.Measures.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj ^= m;
            int changed = await db.SaveChangesAsync();
            return Results.Text("OK" + changed);
        }

        public static async Task<IResult> DelMeasure(uint id, appContext db)
        {
            var obj = await db.Measures.FindAsync(id);
            int changed = 0;
            if (obj is not null)
            {
                db.Measures.Remove(obj);
                changed = await db.SaveChangesAsync();
            }
            return Results.Text("OK" + changed);
        }

        public static async Task<IResult> AddMeasure(uint cnt, Measure m, appContext db)
        {
            db.Measures.Add(m);
            await db.SaveChangesAsync();
            return Results.Created($"/measure/{m.MId}", new { OK = m.MId });
        }
    #endregion

    }
}
