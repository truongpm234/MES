using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Suppliers
{
    public class SupplierLiteDto
    {
        public int SupplierId { get; init; }
        public string Name { get; init; } = null!;
        public string? ContactPerson { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public string? MainMaterialType { get; init; }
        public decimal Rating { get; init; }
    }
}
