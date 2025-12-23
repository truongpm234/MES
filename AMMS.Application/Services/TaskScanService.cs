using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.Productions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Services
{
    public class TaskScanService : ITaskScanService
    {
        private readonly ITaskQrTokenService _tokenSvc;
        private readonly ITaskRepository _taskRepo;
        private readonly ITaskLogRepository _logRepo;

        public TaskScanService(
            ITaskQrTokenService tokenSvc,
            ITaskRepository taskRepo,
            ITaskLogRepository logRepo)
        {
            _tokenSvc = tokenSvc;
            _taskRepo = taskRepo;
            _logRepo = logRepo;
        }

        public async Task<ScanTaskResult> ScanFinishAsync(ScanTaskRequest req)
        {
            if (!_tokenSvc.TryValidate(req.token, out var taskId, out var reason))
                throw new ArgumentException(reason);

            var t = await _taskRepo.GetByIdAsync(taskId)
                ?? throw new Exception("Task not found");

            // cho scan khi Ready hoặc InProgress
            if (!string.Equals(t.status, "Ready", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(t.status, "InProgress", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Task status '{t.status}' is not scannable");

            var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            // 1) update task
            t.status = "Finished";
            t.end_time = now;

            // 2) insert log
            var log = new task_log
            {
                task_id = t.task_id,
                scanner_id = req.scanner_id,
                scanned_code = req.token,
                action_type = "Finish",
                qty_good = req.qty_good ?? 0,
                qty_bad = req.qty_bad ?? 0,
                operator_id = req.operator_id,
                log_time = now
            };

            await _logRepo.AddAsync(log);

            // 3) open next
            if (t.prod_id.HasValue && t.seq_num.HasValue)
            {
                var next = await _taskRepo.GetNextTaskAsync(t.prod_id.Value, t.seq_num.Value);
                if (next != null && string.Equals(next.status, "Unassigned", StringComparison.OrdinalIgnoreCase))
                    next.status = "Ready";
            }

            await _taskRepo.SaveChangesAsync();
            await _logRepo.SaveChangesAsync();

            return new ScanTaskResult
            {
                task_id = t.task_id,
                prod_id = t.prod_id,
                message = "Scanned & progressed"
            };
        }
    }
}
