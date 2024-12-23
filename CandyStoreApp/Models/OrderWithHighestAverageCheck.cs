namespace CandyStoreApp.Models
{
    public class OrderWithHighestAverageCheck
    {
        public int IdOrder { get; set; }
        public string ClientName { get; set; }
        public decimal AverageCheck { get; set; }
    }
}
