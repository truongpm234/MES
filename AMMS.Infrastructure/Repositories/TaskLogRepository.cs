using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Repositories
{
    public class TaskLogRepository : ITaskLogRepository
    {
        private readonly AppDbContext _db;
        public TaskLogRepository(AppDbContext db) => _db = db;

        public Task AddAsync(task_log log)
        {
            _db.task_logs.Add(log);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
