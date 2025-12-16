using System;
using System.Collections.Generic;

namespace AMMS.Infrastructure.Entities;

public partial class production
{
    public int prod_id { get; set; }

    public string? code { get; set; }

    public int? order_id { get; set; }

    public int? manager_id { get; set; }

    public DateTime? start_date { get; set; }

    public DateTime? end_date { get; set; }

    public string? status { get; set; }

    public int? product_type_id { get; set; }

    public virtual user? manager { get; set; }

    public virtual order? order { get; set; }

    public virtual product_type? product_type { get; set; }

    public virtual ICollection<task> tasks { get; set; } = new List<task>();
}
