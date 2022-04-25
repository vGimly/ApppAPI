using System;
using NotesMinimalAPI.app;
using Microsoft.EntityFrameworkCore;

namespace NotesMinimalAPI.ctl
{
    public static partial class appController
    {
        #region Usluga
        public static async Task<List<usluga>> GetUslugi(appContext db)
        {
            return await db.Uslugas.Select(obj => new usluga(obj)).ToListAsync();
        }
        public static async Task<IResult> GetUsluga(uint id, appContext db)
        {
            return await db.Uslugas.FindAsync(id)
                    is Usluga obj
                        ? Results.Ok(new usluga(obj))
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdUsluga(uint id, usluga u, appContext db)
        {
            var obj = await db.Uslugas.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj.fill(u, false);
            int changed = await db.SaveChangesAsync();
            return Results.Ok("OK" + changed);
        }
        
        public static async Task<IResult> DelUsluga(uint id, appContext db)
        {
            var obj = await db.Uslugas.FindAsync(id);
            int changed = 0;
            if (obj is not null)
            {
                db.Uslugas.Remove(obj);
                changed = await db.SaveChangesAsync();
            }
            return Results.Ok("OK" + changed);
        }

        public static async Task<IResult> AddUsluga(usluga u, appContext db)
        {
            var U = new Usluga(u);
            db.Uslugas.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/usluga/{U.UslugaId}", new { OK = U.UslugaId });
        }

        #endregion

        #region Tariff
/*
        internal static async Task<IResult> GetTariffByDate(uint usl, string date_str, appContext db)
        {
            DateOnly date = DateOnly.Parse(date_str);
            return await db.Uslugas.FindAsync(usl) is Usluga u
                ? Results.Ok(u.Tariffs.OrderByDescending(t=>t.TDate).Where(t=>t.TDate <= date).Select(t=>t.Price).First())
                : Results.NotFound();
        }
*/
        public static async Task<List<tariff>> GetTariffs(appContext db)
        {
            return await db.Tariffs.Select(obj => new tariff(obj)).ToListAsync();
        }
        public static async Task<IResult> GetTariffsByUsluga(uint id, appContext db)
        {
//          return await db.Tariffs.Where(obj => obj.UslugaRef == id).Select(obj => new tariff(obj)).ToListAsync();
            return await db.Uslugas.FindAsync(id)
                is Usluga usl
                ? Results.Ok(usl.Tariffs.Select(obj => new tariff(obj)).ToList())
                : Results.NotFound(); 
        }
        public static async Task<IResult> GetTariff(uint id, appContext db)
        {
            return await db.Tariffs.FindAsync(id)
                    is Tariff obj
                        ? Results.Ok(new tariff(obj))
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdTariff(uint id, tariff u, appContext db)
        {
            var obj = await db.Tariffs.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj.fill(u,false);
            int changed = await db.SaveChangesAsync();
            return Results.Ok("OK" + changed);
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
            return Results.Ok("OK" + changed);
        }

        public static async Task<IResult> AddTariff(uint usl, tariff u, appContext db)
        {
            u.usluga_ref = usl;
            var U = new Tariff(u);
            db.Tariffs.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/tariff/{U.TariffId}", new { OK = U.TariffId });
        }
        #endregion

        #region Counter
        public static async Task<List<counter>> GetCounters(appContext db)
        {
            return await db.Counters.Select(obj => new counter(obj)).ToListAsync();
        }
        public static async Task<IResult> GetCountersByUsluga(uint id, appContext db)
        {
//          return await db.Counters.Where(obj => obj.UslugaRef == id).Select(obj => new counter(obj)).ToListAsync();
            return await db.Uslugas.FindAsync(id)
                is Usluga usl
                    ? Results.Ok(usl.Counters.Select(obj => new counter(obj)).ToList())
                    : Results.NotFound();
        }
        public static async Task<IResult> GetCounter(uint id, appContext db)
        {
            return await db.Counters.FindAsync(id)
                    is Counter obj
                        ? Results.Ok(new counter(obj))
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdCounter(uint id, counter u, appContext db)
        {
            var obj = await db.Counters.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj.fill(u, false);
            int changed = await db.SaveChangesAsync();
            return Results.Ok("OK" + changed);
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
            return Results.Ok("OK" + changed);
        }

        public static async Task<IResult> AddCounter(uint usl, counter u, appContext db)
        {
            u.usluga_ref = usl;
            var U = new Counter(u);
            db.Counters.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/counter/{U.CounterId}", new { OK = U.CounterId });
        }
        #endregion

        #region Measure
        public static async Task<List<measure>> GetMeasures(appContext db)
        {
            return await db.Measures.Select(obj => new measure(obj)).ToListAsync();
        }
        public static async Task<IResult> GetMeasuresByCounter(uint cnt, appContext db)
        {
            //          return await db.Measures.Where(obj => obj.UslugaRef == id).Select(obj => new measure(obj)).ToListAsync();
            return await db.Counters.FindAsync(cnt)
                is Counter usl
                    ? Results.Ok(usl.Measures.Select(obj => new measure(obj)).ToList())
                    : Results.NotFound();
        }
        public static async Task<IResult> GetMeasure(uint id, appContext db)
        {
            return await db.Measures.FindAsync(id)
                    is Measure obj
                        ? Results.Ok(new measure(obj))
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdMeasure(uint id, measure u, appContext db)
        {
            var obj = await db.Measures.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj.fill(u, false);
            int changed = await db.SaveChangesAsync();
            return Results.Ok("OK" + changed);
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
            return Results.Ok("OK" + changed);
        }

        public static async Task<IResult> AddMeasure(uint cnt, measure u, appContext db)
        {
            u.counter_ref = cnt;
            var U = new Measure(u);
            db.Measures.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/measure/{U.MId}", new { OK = U.MId });
        }
        #endregion

    }
}
