using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ApppAPI.app
{
    public static partial class appController
    {
        internal static CultureInfo provider = new CultureInfo("en-US");

        #region Usluga
        public static async Task<List<usluga>> GetUslugi(appContext db)
        {
            return await db.Uslugas.Select(obj => new usluga(obj)).ToListAsync();
        }
        public static async Task<IResult> GetUsluga(uint usl, appContext db)
        {
            return await db.Uslugas.FindAsync(usl)
                    is Usluga obj
                        ? Results.Ok(new usluga(obj))
                        : Results.NotFound();
        }

        public static async Task<IResult> UpdUsluga(uint usl, HttpContext ctx, appContext db)
        //        public static async Task<IResult> UpdUsluga(uint usl, [FromForm(Name = "usluga-name")] string? UslugaName, appContext db)
        {
            var obj = await db.Uslugas.FindAsync(usl);
            if (obj is null) return Results.NotFound();
            obj.UslugaName = ctx.Request.Form["usluga-name"];
            //if (UslugaName is not null) obj.UslugaName = UslugaName;
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

        //public static async Task<IResult> AddUsluga([FromForm(Name = "usluga-name")] string? UslugaName, appContext db)
        public static async Task<IResult> AddUsluga(HttpContext ctx, appContext db)
        {
            //var U = new Usluga { UslugaName = UslugaName };
            var U = new Usluga { UslugaName = ctx.Request.Form["usluga-name"] };
            db.Uslugas.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/usluga/{U.UslugaId}", new { OK = U.UslugaId });
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
                    .Select(obj => obj.Price).FirstAsync());
            }
            catch (Exception) { return Results.Text(""); }
        }
        public static async Task<List<tariff>> GetTariffs(appContext db)
        {
            return await db.Tariffs.Select(obj => new tariff(obj)).ToListAsync();
        }
        public static async Task<IResult> GetTariffsByUsluga(uint usl, appContext db)
        {
            return Results.Ok(await db.Tariffs.Where(obj => obj.UslugaRef == usl).Select(obj => new tariff(obj)).ToListAsync());
            //return await db.Uslugas.FindAsync(id)
            //    is Usluga usl
            //    ? Results.Ok(usl.Tariffs.Select(obj => new tariff(obj)).ToList())
            //    : Results.NotFound(); 
        }
        public static async Task<IResult> GetTariff(uint id, appContext db)
        {
            return await db.Tariffs.FindAsync(id)
                    is Tariff obj
                        ? Results.Ok(new tariff(obj))
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdTariff(uint id, HttpContext ctx,
            //[FromForm(Name = "tariff-start")] DateTime t_date,
            //[FromForm(Name = "tariff-value")] decimal price,
            appContext db)
        {
            var obj = await db.Tariffs.FindAsync(id);
            if (obj is null) return Results.NotFound();

            //obj.fill(new tariff { t_date = t_date, price=price }, false);
            obj.fill(new tariff
            {
                t_date = DateTime.Parse(ctx.Request.Form["tariff-start"]),
                price = decimal.Parse(ctx.Request.Form["tariff-value"], provider),
            }, false);

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

        //public static async Task<IResult> AddTariff(uint usl,
        //                [FromForm(Name = "tariff-start")] DateTime t_date,
        //                [FromForm(Name = "tariff-value")] decimal price,
        //                appContext db)
        public static async Task<IResult> AddTariff(uint usl, HttpContext ctx, appContext db)
        {
            var U = new Tariff().fill(new tariff { usluga_ref = usl,
                t_date = DateTime.Parse(ctx.Request.Form["tariff-start"]),
                price=decimal.Parse(ctx.Request.Form["tariff-value"], provider),
            });
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
        public static async Task<IResult> GetCountersByUsluga(uint usl, appContext db)
        {
            return Results.Ok(await db.Counters.Where(obj => obj.UslugaRef == usl).Select(obj => new counter(obj)).ToListAsync());
            //return await db.Uslugas.FindAsync(usl)
            //    is Usluga u
            //        ? Results.Ok(u.Counters.Select(obj => new counter(obj)).ToList())
            //        : Results.NotFound();
        }
        public static async Task<IResult> GetCounter(uint id, appContext db)
        {
            return await db.Counters.FindAsync(id)
                    is Counter obj
                        ? Results.Ok(new counter(obj))
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdCounter(uint id,
            //[FromForm(Name = "counter-name")] string counter_name,
            //[FromForm(Name = "serial")] string serial,
            //[FromForm(Name = "digits")] byte digits,
            //[FromForm(Name = "precise")] byte precise,
            HttpContext ctx,
            appContext db)
        {
            var obj = await db.Counters.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj.fill(new counter
            {
                counter_name = ctx.Request.Form["counter-name"],
                serial = ctx.Request.Form["serial"],
                digits = byte.Parse(ctx.Request.Form["digits"]),
                precise = byte.Parse(ctx.Request.Form["precise"]),

                //counter_name = counter_name,
                //serial = serial,
                //digits = digits,
                //precise = precise,
            }, false);
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

        public static async Task<IResult> AddCounter(uint usl, 
            //[FromForm(Name = "counter-name")] string counter_name,
            //[FromForm(Name = "serial")] string serial,
            //[FromForm(Name = "digits")] byte digits,
            //[FromForm(Name = "precise")] byte precise,
            HttpContext ctx,
            appContext db)
        {
            var U = new Counter(new counter
            {
                usluga_ref = usl,
                //counter_name = counter_name, serial=serial, digits=digits, precise=precise,
                counter_name = ctx.Request.Form["counter-name"],
                serial = ctx.Request.Form["serial"],
                digits = byte.Parse(ctx.Request.Form["digits"]),
                precise = byte.Parse(ctx.Request.Form["precise"]),
            } );
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
            return Results.Ok(await db.Measures.Where(obj => obj.CounterRef == cnt).Select(obj => new measure(obj)).ToListAsync());
            //return await db.Counters.FindAsync(cnt)
            //    is Counter usl
            //        ? Results.Ok(usl.Measures.Select(obj => new measure(obj)).ToList())
            //        : Results.NotFound();
        }
        public static async Task<IResult> GetMeasure(uint id, appContext db)
        {
            return await db.Measures.FindAsync(id)
                    is Measure obj
                        ? Results.Ok(new measure(obj))
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdMeasure(uint id,
            HttpContext ctx,
            //[FromForm(Name = "m-date")] DateTime m_date,
            //[FromForm(Name = "value")] decimal value,
            appContext db)
        {
            var obj = await db.Measures.FindAsync(id);
            if (obj is null) return Results.NotFound();
            obj.fill(new measure {
                m_date = DateTime.Parse(ctx.Request.Form["m-date"]),
                value = decimal.Parse(ctx.Request.Form["value"], provider)
            //m_date=m_date, value=value
        }, false);
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

        public static async Task<IResult> AddMeasure(uint cnt, HttpContext ctx,
            //[FromForm(Name = "m-date")] DateTime m_date,
            //[FromForm(Name = "value")] decimal value,
            appContext db)
        {
            var U = new Measure().fill(new measure { counter_ref = cnt,
                m_date = DateTime.Parse(ctx.Request.Form["m-date"]),
                value=decimal.Parse(ctx.Request.Form["value"], provider)
            });
            db.Measures.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/measure/{U.MId}", new { OK = U.MId });
        }
        #endregion

    }
}
