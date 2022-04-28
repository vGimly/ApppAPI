using System.Text.Json.Serialization;

namespace ApppAPI.app
{
    public partial class Measure
    {
        public uint MId { get; set; }
        public uint CounterRef { get; set; }
        public DateTime MDate { get; set; }
        public decimal Value { get; set; }
        
        [JsonIgnore]
        public DateTime Alt { get; set; }
        
        [JsonIgnore]
        public virtual Counter CounterRefNavigation { get; set; } = null!;
        public static Measure operator ^ (Measure a, Measure b) { a.MDate=b.MDate; a.Value=b.Value; return a; }

        public static DateTime EndOfTheDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddTicks(-1);
        }
        public void Verify()
        {
            if (Value >= CounterRefNavigation.Max)
            {
                throw new Exception($"Значение: {Value} не соответсвует Разрядности счётчика {CounterRefNavigation.Digits}");
            }
            else if (Value < 0)
            {
                throw new Exception($"Значение: {Value} отрицательно");
            }
            if (MDate > Measure.EndOfTheDay(DateTime.Now))
                throw new Exception($"Дата измерения из будущего: {MDate:d}");
        }
    }
}
