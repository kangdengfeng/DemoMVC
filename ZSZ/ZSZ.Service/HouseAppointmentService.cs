using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class HouseAppointmentService : IHouseAppointmentService
    {
        public long AddNew(long? userId, string name, string phoneNum, long houseId, DateTime visitDate)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                HouseAppointmentEntity houseApp = new HouseAppointmentEntity();
                houseApp.HouseId = houseId;
                houseApp.Name = name;
                houseApp.PhoneNum = phoneNum;
                houseApp.Status = "未处理";
                houseApp.UserId = userId;
                houseApp.VisitDateTime = visitDate;
                ctx.HouseAppointments.Add(houseApp);
                ctx.SaveChanges();
                return houseApp.Id;
            }
        }

        //后台抢单处理
        public bool Follow(long adminUserId, long houseAppointmentId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseAppointmentEntity> bs =
                    new BaseService<HouseAppointmentEntity>(ctx);
                var app = bs.GetById(houseAppointmentId);
                if (app == null)
                {
                    throw new ArgumentException("不存在的订单id");
                }
                //FollowAdminUserId不为null，说明要么是自己已经抢过，要么是已经早早的
                //被别人抢了
                if (app.FollowAdminUserId != null)
                {
                    return app.FollowAdminUserId == adminUserId;
                    /*
                    if(app.FollowAdminUserId==adminUserId)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }*/
                }
                //如果FollowAdminUserId为null，说明有抢的机会
                app.FollowAdminUserId = adminUserId;
                try
                {
                    ctx.SaveChanges();//有可能出现异常
                    return true;
                }//如果抛出DbUpdateConcurrencyException说明抢单失败（乐观锁）
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            }
        }
        private HouseAppointmentDTO ToDTO(HouseAppointmentEntity houseApp)
        {
            HouseAppointmentDTO dto = new HouseAppointmentDTO();
            dto.CommunityName = houseApp.House.Communitie.Name;
            dto.CreateDateTime = houseApp.CreateDateTime;
            dto.FollowAdminUserId = houseApp.FollowAdminUserId;
            //已经有人跟踪，获取跟踪的用户名
            if (houseApp.FollowAdminUser != null)
            {
                dto.FollowAdminUserName = houseApp.FollowAdminUser.Name;
            }
            dto.FollowDateTime = houseApp.FollowDateTime;
            dto.HouseId = houseApp.HouseId;
            dto.Id = houseApp.Id;
            dto.Name = houseApp.Name;
            dto.PhoneNum = houseApp.PhoneNum;
            dto.RegionName = houseApp.House.Communitie.Region.Name;
            dto.Status = houseApp.Status;
            dto.UserId = houseApp.UserId;
            dto.VisitDate = houseApp.VisitDateTime;
            dto.HouseAdress = houseApp.House.Address;
            return dto;
        }
        //根据Id获取预约看房的DTO
        public HouseAppointmentDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseAppointmentEntity> bs = new BaseService<HouseAppointmentEntity>(ctx);
                var houseApp = bs.GetAll().Include(h => h.House)
                    //Include("House.Community")
                    .Include(nameof(HouseAppointmentEntity.House) + "." + nameof(HouseEntity.Communitie))
                    .Include(a => a.FollowAdminUser)
                    //Include("House.Community.Region")
                    .Include(nameof(HouseAppointmentEntity.House) + "." + nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region))
                    .AsNoTracking().SingleOrDefault(s => s.Id == id);
                if (houseApp == null)
                {
                    return null;
                }
                return ToDTO(houseApp);
            }
        }

        public HouseAppointmentDTO[] GetPagedData(long cityId, string status, int pageSize, int currentIndex)
        {
            //Skip之前一定要orderby
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseAppointmentEntity> bs
                    = new BaseService<HouseAppointmentEntity>(ctx);
                var apps = bs.GetAll()
                    //lambda表达式的Include
                    .Include(h => h.House)
                    //EF的Include
                    .Include(nameof(HouseAppointmentEntity.House) + "." + nameof(HouseEntity.Communitie))
                    .Include(nameof(HouseAppointmentEntity.House) + "." + nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region))
                    .AsNoTracking()
                    .Where(c => c.House.Communitie.Region.CityId == cityId && c.Status == status)
                    .OrderBy(a => a.CreateDateTime)
                    .Skip(currentIndex).Take(pageSize);
                return apps.ToList().Select(a => ToDTO(a)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
            }
        }

        public long GetTotalCount(long cityId, string status)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseAppointmentEntity> bs
                    = new BaseService<HouseAppointmentEntity>(ctx);
                var count = bs.GetAll().LongCount(c => c.House.Communitie.Region.CityId == cityId && c.Status == status);
                return count;
            }
        }
    }
}
