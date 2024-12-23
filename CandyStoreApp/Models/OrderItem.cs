using System;
using System.Collections.Generic;

namespace CandyStoreApp.Models;

public partial class OrderItem
{
    public int IdOrderItem { get; set; }

    public int? IdOrder { get; set; }

    public int? IdProduct { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Order? IdOrderNavigation { get; set; }

    public virtual Product? IdProductNavigation { get; set; }


}

