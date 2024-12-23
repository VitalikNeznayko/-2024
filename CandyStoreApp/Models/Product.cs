using System;
using System.Collections.Generic;

namespace CandyStoreApp.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string? ProductName { get; set; }

    public int? IdCategory { get; set; }

    public int? IdSupplier { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public string? ProductDescription { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }

    public virtual Supplier? IdSupplierNavigation { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
