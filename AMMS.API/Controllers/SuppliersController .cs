using AMMS.Application.Interfaces;
using AMMS.Shared.DTOs.Suppliers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service)
        {
            _service = service;
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
        {
            var result = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("{id:int}/detail")]
        public async Task<IActionResult> GetSupplierDetail(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _service.GetSupplierDetailWithMaterialsAsync(id, page, pageSize, ct);
            if (result == null) return NotFound(new { message = "Supplier not found", supplierId = id });

            return Ok(result);
        }
    }
}
