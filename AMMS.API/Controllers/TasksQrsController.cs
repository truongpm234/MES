using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace AMMS.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksQrController : ControllerBase
    {
        private readonly ITaskRepository _taskRepo;
        private readonly ITaskQrTokenService _tokenSvc;

        public TasksQrController(ITaskRepository taskRepo, ITaskQrTokenService tokenSvc)
        {
            _taskRepo = taskRepo;
            _tokenSvc = tokenSvc;
        }

        [HttpGet("{taskId:int}/qr")]
        public async Task<IActionResult> GetQr(int taskId)
        {
            var t = await _taskRepo.GetByIdAsync(taskId);
            if (t == null) return NotFound();

            var token = _tokenSvc.CreateToken(taskId, TimeSpan.FromDays(2));
            var url = $"{Request.Scheme}://{Request.Host}/api/tasks/scan?token={Uri.EscapeDataString(token)}";

            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qr = new PngByteQRCode(data);
            var png = qr.GetGraphic(20);

            return File(png, "image/png");
        }
    }

}
