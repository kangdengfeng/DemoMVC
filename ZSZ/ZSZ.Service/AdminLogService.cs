using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class AdminLogService : IAdminLogService
    {
        public long AddNew(long adminUserId, string message)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                AdminLogEntity log = new AdminLogEntity() { AdminUserId = adminUserId, Msg = message };
              ctx.AdminLogs.Add(log);
                ctx.SaveChanges();
                return log.Id;
            }
        }

        public AdminLogDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminLogEntity> bs = new BaseService<AdminLogEntity>(ctx);
                var log = bs.GetById(id);
                if (log==null)
                {
                    return null;
                }
                AdminLogDTO dto = new AdminLogDTO();
                dto.AdminUserId = log.AdminUserId;
                dto.AdminUserName = log.AdminUser.Name;
                dto.AdminUserPhoneNum = log.AdminUser.PhoneNum;
                dto.CreateDateTime = log.CreateDateTime;
                dto.Id = log.Id;
                dto.Message = log.Msg;
                return dto;
            }
        }

        //public AdminLogDTO GetById(long id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
