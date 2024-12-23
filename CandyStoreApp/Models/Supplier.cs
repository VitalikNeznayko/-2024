using System;
using System.Collections.Generic;

namespace CandyStoreApp.Models;

public partial class Supplier
{
    public int IdSupplier { get; set; }

    public string? SupplierName { get; set; }

    public string? ContactPerson { get; set; }

    public string? PhoneNumber { get; set; }

    public string? SupplierAddress { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
