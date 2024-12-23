namespace CandyStoreApp.Models
{
    public class ProductSalesByRating
    {
        public int ChooseRating { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
