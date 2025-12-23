using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMMS.Infrastructure.Entities;
[Table("product_type_process", Schema = "AMMS_DB")]

public partial class product_type_process
{
    public int process_id { get; set; }

    public int product_type_id { get; set; }

    public int seq_num { get; set; }

    public string process_name { get; set; } = null!;

    public string? machine { get; set; }

    public bool? is_active { get; set; }

    public virtual product_type product_type { get; set; } = null!;

    public virtual ICollection<task> tasks { get; set; } = new List<task>();
}
