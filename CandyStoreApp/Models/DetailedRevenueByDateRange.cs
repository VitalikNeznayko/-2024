namespace CandyStoreApp.Models
{
    public class DetailedRevenueByDateRange
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string CategoryName { get; set; }
        public decimal RevenueByCategory { get; set; }
    }
}
