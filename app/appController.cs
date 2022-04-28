using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace ApppAPI.app
{
    public static partial class appController
    {
        #region Usluga
        public static async Task<IResult> GetUslugi(appContext db)
        {
            return Results.Ok(await db.Uslugas.ToListAsync());
        }

        public static async Task<IResult> GetUsluga(uint usl, appContext db)
        {
            return await db.Uslugas.FindAsync(usl)
                    is Usluga u
                        ? Results.Ok(u)
                        : Results.NotFound();
        }

        public static async Task<IResult> UpdUsluga(uint usl, Usluga U, appContext db)
        {
            var obj = await db.Uslugas.FindAsync(usl);
            if (obj is null) return Results.NotFound();
            obj ^= U;
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

        public static async Task<IResult> AddUsluga(Usluga U, appContext db)
        {
            db.Uslugas.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/usluga/{U.UslugaId}", new { OK = U.UslugaId });
        }

        #endregion

        #region Tariff
        internal static async Task<IResult> GetTariffByDate(uint usl, DateTime DTime, appContext db)
        {
            try
            {
                return Results.Ok(await db.Tariffs
                    .Where(t => t.UslugaRef == usl && t.TDate <= DTime)
                    .OrderByDescending(t => t.TDate)
                    .Select(obj => obj.Price).FirstAsync());
            }
            catch (Exception) { return Results.Text(""); }
        }

        public static async Task<List<Tariff>> GetTariffs(appContext db)
        {
            return await db.Tariffs.ToListAsync();
        }
        public static async Task<IResult> GetTariffsByUsluga(uint usl, appContext db)
        {
            return Results.Ok(await db.Tariffs.Where(obj => obj.UslugaRef == usl)
//                .Select(t => new { t.TariffId, t.TDate, t.UslugaRef, t.Price})
                .ToListAsync());
        }
        public static async Task<IResult> GetTariff(uint id, appContext db)
        {
            return await db.Tariffs.FindAsync(id)
                    is Tariff t
                        ? Results.Ok(t) // new { t.TariffId, t.TDate, t.UslugaRef, t.Price })
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

        public static async Task<IResult> AddTariff(uint usl, Tariff U, appContext db)
        {
            db.Tariffs.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/tariff/{U.TariffId}", new { OK = U.TariffId });
        }
        #endregion

        #region Counter
        public static async Task<IResult> GetCounters(appContext db)
        {
            return Results.Ok(await db.Counter2
//                .Select(u => new { u.CounterId, u.UslugaRef, u.CounterName, u.Serial, u.Precise, u.Digits })
                .ToListAsync());
        }
        public static async Task<IResult> GetCountersByUsluga(uint usl, appContext db)
        {
            return Results.Ok(await
                (from c in db.Counter2
                 where c.UslugaRef == usl
                 /*
                                  let M = (from m in db.Measures
                                           orderby m.MId ascending
                                           where m.CounterRef == c.CounterId
                                           select new Counter { StartDate = m.MDate, InitValue = m.Value }
                                           ).FirstOrDefault(new Counter { StartDate = DateTime.MinValue, InitValue = 0 })
                                  select new Counter
                                  {
                                      CounterId = c.CounterId,
                                      CounterName = c.CounterName,
                                      UslugaRef = c.UslugaRef,
                                      Digits = c.Digits,
                                      Precise = c.Precise,
                                      Serial=c.Serial,
                                      InitValue=M.InitValue,
                                      StartDate=M.StartDate
                                  }
                 */
                select c
                )
                .ToListAsync());
        }
        public static async Task<IResult> GetCounter(uint id, appContext db)
        {
            return await db.Counters.FindAsync(id)
                    is Counter u
                        ? Results.Ok(u)
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdCounter(uint id, Counter t, appContext db)
        {
            try
            {
                if (await db.Verify(t))
                    throw new Exception($"Не удастся изменить разрядность счётчика из-за наличия измерений с большей разрядностью {t.Digits}");

                var obj = await db.Counters.FindAsync(id);
                if (obj is null) return Results.NotFound();
                obj ^= t;
                obj.Verify();
                int changed = await db.SaveChangesAsync();
                return Results.Text("OK" + changed);
            }catch (Exception ex){ return Results.BadRequest(ex.Message); }
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

        //public static async Task<T> Deserialize<T>(Stream s)
        //{
        //    return await JsonSerializer.DeserializeAsync<T>(s); // ,new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        //    var U = await Deserialize<Counter>(ctx.Request.Body);
        //}
        public static async Task<IResult> AddCounter(uint usl, Counter U, appContext db)
        {
            try
            {
                var M = new Measure { MDate = U.StartDate, Value = U.InitValue };
                U.Measures.Add(M);
                U.Verify();
                db.Counters.Add(U);
                await db.SaveChangesAsync();
                return Results.Created($"/counter/{U.CounterId}", new { OK = U.CounterId, mea = M.MId });
            } catch (Exception ex) { return Results.BadRequest(ex.Message); }
        }
        #endregion

        #region Measure
        public static async Task<IResult> GetMeasures(appContext db)
        {
            return Results.Ok(await db.Measures
                //.Select(m => new {m.MId, m.CounterRef, m.MDate, m.Value})
                .ToListAsync()
                .ConfigureAwait(false)
                );
        }

        public static async Task<IResult> GetMeasuresByCounter(uint cnt, appContext db)
        {
            var query = from m in db.Measures
                        where m.CounterRef == cnt
                        join s in (
                            from p in db.Measures
                            group p by p.CounterRef into g
                            select new { CounterRef = g.Key, InitValue = g.Min(x => x.Value) } // InitDate = g.Min(x => x.MDate), 
                        ) on m.CounterRef equals s.CounterRef
                        select new { m.CounterRef, m.MId, m.MDate, m.Value, s.InitValue,
                            Tariff = (from t in db.Tariffs join c in db.Counters on t.UslugaRef equals c.UslugaRef
                                      where c.CounterId == cnt && t.TDate <= m.MDate
                                      orderby t.TDate descending select t.Price).FirstOrDefault()
                        };
            return Results.Ok(await query.ToListAsync());
        }
/*
    public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>
    (this IEnumerable<TSource> source,
     Func<TSource, TSource, TResult> projection)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    yield break;
                }
                TSource previous = iterator.Current;
                while (iterator.MoveNext())
                {
                    yield return projection(previous, iterator.Current);
                    previous = iterator.Current;
                }
            }
        }
*/

        public static async Task<IResult> GetMoneyByCounter(uint cnt, appContext db)
        {
            var query = await (from m in db.Measures
                               where m.CounterRef == cnt
                               orderby m.MDate ascending
                               select new
                               {
                                   m.Value,
                                   Tariff = (from t in db.Tariffs
                                             join c in db.Counters on t.UslugaRef equals c.UslugaRef
                                             where c.CounterId == cnt && t.TDate <= m.MDate
                                             orderby t.TDate descending
                                             select t.Price).FirstOrDefault()
                               }).ToListAsync();

//            var res=query.SelectWithPrevious((prev, cur) =>
//                            new { cur.Value, money = (cur.Value - prev.Value) * cur.Tariff, cur.Tariff } );
            decimal last = 0;
            //          var res = query.Aggregate(last, (acc, it) => { last += (it.Value - acc) * it.Tariff; return it.Value; }, (acc) => (last));
            var res=query.Skip(1).Zip(query, (curr, prev) => last += (curr.Value - prev.Value) * curr.Tariff).LastOrDefault(0);
            return Results.Ok(res);
        }
        public static async Task<IResult> GetMeasure(uint id, appContext db)
        {
            return await db.Measures.FindAsync(id)
                    is Measure m
                        ? Results.Ok(new { m.MId, m.CounterRef, m.MDate, m.Value })
                        : Results.NotFound();
        }
        public static async Task<IResult> UpdMeasure(uint id, Measure U, appContext db)
        {
            try
            {
                var obj = await db.Measures.FindAsync(id);
                if (obj is null) return Results.NotFound();
                obj ^= U;
                db.Verify(obj);
                int changed = await db.SaveChangesAsync();
                return Results.Text("OK" + changed);
            }
            catch (Exception ex) { return Results.BadRequest(ex.Message); }
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

        public static async Task<IResult> AddMeasure(uint cnt, Measure U, appContext db)
        {
            try {
                db.Verify(U);
            db.Measures.Add(U);
            await db.SaveChangesAsync();
            return Results.Created($"/measure/{U.MId}", new { OK = U.MId });
            } catch (Exception ex) { return Results.BadRequest(ex.Message); }

        }
        #endregion

    }
}
