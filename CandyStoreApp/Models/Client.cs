using System;
using System.Collections.Generic;

namespace CandyStoreApp.Models;

public partial class Client
{
    public int IdClient { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AddressClient { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
