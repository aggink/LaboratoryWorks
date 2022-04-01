namespace ProtectionOfInfo.WebApp.Data.Entities
{
    public class Valute
    {
        public string ID { get; set; } = null!;
        public int NumCode { get; set; }
        public string CharCode { get; set; } = null!;
        public decimal Nominal { get; set; }
        public string Name { get; set; } = null!;
        public decimal Value { get; set; }
    }
}
