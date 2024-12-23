using System;
using System.Collections.Generic;

namespace CandyStoreApp.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public int? IdClient { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalCost { get; set; }

    public string? OrderStatus { get; set; }

    public virtual Client? IdClientNavigation { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
