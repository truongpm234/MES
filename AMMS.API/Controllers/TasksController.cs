using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMMS.API.Controllers
{
    using AMMS.Application.Interfaces;
    using AMMS.Shared.DTOs.Productions;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/tasks")]
    public class TasksScanController : ControllerBase
    {
        private readonly ITaskScanService _scanSvc;
        public TasksScanController(ITaskScanService scanSvc) => _scanSvc = scanSvc;

        [HttpPost("scan")]
        public async Task<IActionResult> Scan([FromBody] ScanTaskRequest req)
        {
            var result = await _scanSvc.ScanFinishAsync(req);
            return Ok(result);
        }
    }

}
