using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class HouseService : IHouseService
    {
        public HouseDTO[] GetAll()
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> houseBS = new BaseService<HouseEntity>(ctx);
                var houses = houseBS.GetAll()
                    .Include(nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region))
                    .Include(h => h.DecorateStatus)
                    .Include(h => h.HousePics)
                    .Include(h => h.RoomType)
                    .Include(h => h.Status)
                    .Include(h => h.Type);
                return houses.ToList().Select(h => ToDTO(h)).ToArray();
            }
        }

        #region MyRegion
        /*
        public long AddNew(HouseDTO house)
        {
            HouseEntity houseEntity = new HouseEntity();
            houseEntity.Address = house.Address;
            houseEntity.Area = house.Area;

            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AttachmentEntity> attBS
                    = new BaseService<AttachmentEntity>(ctx);
                var atts = attBS.GetAll().Where(a => house.AttachmentIds.Contains(a.Id));
						
                foreach (var att in atts)
                {
                    houseEntity.Attachments.Add(att);
                }
                houseEntity.CheckInDateTime = house.CheckInDateTime;
                houseEntity.CommunityId = house.CommunityId;
                houseEntity.CreateDateTime = house.CreateDateTime;
                houseEntity.DecorateStatusId = house.DecorateStatusId;
                houseEntity.Description = house.Description;
                houseEntity.Direction = house.Direction;
                houseEntity.FloorIndex = house.FloorIndex;
                //houseEntity.HousePics 新增后再单独添加
                houseEntity.LookableDateTime = house.LookableDateTime;
                houseEntity.MonthRent = house.MonthRent;
                houseEntity.OwnerName = house.OwnerName;
                houseEntity.OwnerPhoneNum = house.OwnerPhoneNum;
                houseEntity.RoomTypeId = house.RoomTypeId;
                houseEntity.StatusId = house.StatusId;
                houseEntity.TotalFloorCount = house.TotalFloorCount;
                houseEntity.TypeId = house.TypeId;
                ctx.Houses.Add(houseEntity);
                ctx.SaveChanges();
                return houseEntity.Id;
            }
        }
        */
        #endregion
        //优化的AddNew
        public long AddNew(HouseAddnewDTO house)
        {
            HouseEntity houseEntity = new HouseEntity();
            houseEntity.Address = house.Address;
            houseEntity.Area = house.Area;
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AttachmentEntity> attBS = new BaseService<AttachmentEntity>(ctx);
                //拿到house.AttachmentIds为主键的房屋配套设施
                var atts = attBS.GetAll().Where(a => house.AttachmentIds.Contains(a.Id));
                //houseEntity.Attachments = new List<AttachmentEntity>();
                foreach (var att in atts)
                {
                    houseEntity.Attachments.Add(att);
                }
                houseEntity.CheckinDateTime = house.CheckInDateTime;
                houseEntity.CommunityId = house.CommunityId;
                houseEntity.DecorateStatusId = house.DecorateStatusId;
                houseEntity.Description = house.Description;
                houseEntity.Direction = house.Direction;
                houseEntity.FloorIndex = house.FloorIndex;
                //houseEntity.HousePics 新增后再单独添加
                houseEntity.LookableDateTime = house.LookableDateTime;
                houseEntity.MonthRent = house.MonthRent;
                houseEntity.OwnerName = house.OwnerName;
                houseEntity.OwnerPhoneNum = house.OwnerPhoneNum;
                houseEntity.RoomTypeId = house.RoomTypeId;
                houseEntity.StatusId = house.StatusId;
                houseEntity.TotalFloorCount = house.TotalFloorCount;
                houseEntity.TypeId = house.TypeId;
                ctx.Houses.Add(houseEntity);
                ctx.SaveChanges();
                return houseEntity.Id;
            }
        }

        public long AddNewHousePic(HousePicDTO housePic)
        {
            HousepicEntity hpEntity = new HousepicEntity();
            hpEntity.HouseId = housePic.HouseId;
            hpEntity.ThumbUrl = housePic.ThumbUrl;
            hpEntity.Url = housePic.Url;
            using (MyDbContext ctx = new MyDbContext())
            {
                ctx.Housepics.Add(hpEntity);
                ctx.SaveChanges();
                return hpEntity.Id;
            }
        }

        public void DeleteHousePic(long housePicId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                /*
                //改变对象状态删除
                HousepicEntity entity = new HousepicEntity();
                entity.Id = housePicId;
                ctx.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
                */
                //先查询再删除
                var entity = ctx.Housepics.SingleOrDefault(p => p.IsDeleted == false && p.Id == housePicId);
                if (entity != null)
                {
                    ctx.Housepics.Remove(entity);
                    ctx.SaveChanges();
                }
            }
        }

       

        public HouseDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> houseBS = new BaseService<HouseEntity>(ctx);
                var house = houseBS.GetAll()
                    .Include(h => h.Attachments).Include(h => h.Communitie)
                    //Include("Community.Region.City");
                    .Include(nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region))
                    .Include(h => h.DecorateStatus)
                    .Include(h => h.HousePics)
                    .Include(h => h.RoomType)
                    .Include(h => h.Status)
                    .Include(h => h.Type)
                    .SingleOrDefault(h => h.Id == id);
                //.Where(h => h.Id == id).SingleOrDefault();
                if (house == null)
                {
                    return null;
                }
                return ToDTO(house);
            }
        }

        public long GetCount(long cityId, DateTime startDateTome, DateTime endDateTime)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> houseBs = new BaseService<HouseEntity>(ctx);
                return houseBs.GetAll().Count(h => h.Communitie.Region.CityId == cityId
                && h.CreateDateTime >= startDateTome
                && h.CreateDateTime <= endDateTime);
            }
        }

        public HouseDTO[] GetPagedData(long cityId, long typeId, int pageSize, int currentIndex)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> houseBS = new BaseService<HouseEntity>(ctx);
                var houses = houseBS.GetAll()
                    /*
                    .Include(h => h.Attachments).Include(h => h.Communitie)
                    .Include(h => nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(h => nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region))
                    */
                    //.Include("Communitie.Region.City")
                    .Include(nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region))
                    .Include(h => h.DecorateStatus)
                    .Include(h => h.HousePics)
                    .Include(h => h.RoomType)
                    .Include(h => h.Status)
                    .Include(h => h.Type)
                    .Where(h => h.Communitie.Region.CityId == cityId && h.TypeId == typeId)
                    .OrderByDescending(h => h.CreateDateTime)
                    .Skip(currentIndex).Take(pageSize);
                // .Where(h => h.Communitie.Region.CityId == cityId && h.TypeId == typeId);
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
                return houses.ToList().Select(h => ToDTO(h)).ToArray();
            }
        }

        public HousePicDTO[] GetPics(long houseId)
        {
            /*
            //从房子图片开始找
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HousepicEntity> bs = new BaseService<HousepicEntity>(ctx);
                return bs.GetAll().AsNoTracking().Where(p => p.HouseId == houseId)
                    .Select(p => new HousePicDTO
                    {
                        CreateDateTime = p.CreateDateTime,
                        HouseId = p.HouseId,
                        ThumbUrl = p.ThumbUrl,
                        Url = p.Url
                    }).ToArray();
            }
            */
            //从房子开始找
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                return bs.GetById(houseId).HousePics.Select(p => new HousePicDTO()
                {
                    CreateDateTime = p.CreateDateTime,
                    HouseId = p.HouseId,
                    Id = p.Id,
                    ThumbUrl = p.ThumbUrl,
                    Url = p.Url
                }).ToArray();
            }
        }

        public int GetTodayNewHouseCount(long cityId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                //距离定时发报表的24小时的间隔
                //房子创建时间在当前时间的24个小时就认为值今天的房源
                //两个时间相减得到TimeSpan类型的值
                //bs.GetAll().Where(h => h.Communitie.Region.CityId==cityId
                //            && (DateTime.Now - h.CreateDateTime).TotalHours <= 24);
                //EF翻译不了
                //return bs.GetAll().Count(h => h.Communitie.Region.CityId == cityId
                //          && (DateTime.Now - h.CreateDateTime).TotalHours <= 24);
                //count也支持过滤

                //First
                //SqlFunctions是SqlServer独有的函数
                // return bs.GetAll().Count(h => h.Communitie.Region.CityId == cityId 
                //&& SqlFunctions.DateDiff("hh",h.CreateDateTime,DateTime.Now) <= 24);

                //Second    
                //先算出来当前时间往前24小时是什么时间
                //跨数据库
                DateTime date24HourAgo = DateTime.Now.AddHours(-24);
                return bs.GetAll().Count(h => h.Communitie.Region.CityId == cityId
                       && h.CreateDateTime >= date24HourAgo);

            }
        }

        public long GetTotalCount(long cityId, long typeId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                return bs.GetAll().LongCount(h => h.Communitie.Region.CityId == cityId && h.TypeId == typeId);
            }
        }

        public void MarkDeleted(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                bs.MarkDeleted(id);
            }
        }

        public HouseSearchResult Search(HouseSearchOptions options)
        {
            //先获得所有未被软删除的房源信息
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                var items = bs.GetAll().Where(h => h.Communitie.Region.CityId == options.CityId
                                    && h.TypeId == options.TypeId);
                //regionId可为空,判断不为空再继续过滤
                //拼接查询语句
                //where有返回值的，一定要items = ....;拿到返回值之后重新赋值才有用
                if (options.RegionId != null)
                {
                    items = items.Where(t => t.Communitie.RegionId == options.RegionId);
                }
                if (options.StartMonthRent != null)
                {
                    items = items.Where(t => t.MonthRent >= options.StartMonthRent);
                }
                if (options.EndMonthRent != null)
                {
                    items = items.Where(t => t.MonthRent <= options.EndMonthRent);
                }
                if (!string.IsNullOrEmpty(options.Keywords))
                {
                    items = items.Where(t => t.Address.Contains(options.Keywords)
                            || t.Description.Contains(options.Keywords)
                            || t.Communitie.Name.Contains(options.Keywords)
                            || t.Communitie.Location.Contains(options.Keywords)
                            || t.Communitie.Traffic.Contains(options.Keywords));
                }

                //取出总条数
                //log4Net中查看数据库执行语句顺序，count是单独执行一条SQL语句
                //不会把数据取到内存中
                long totalcount = items.LongCount();

                //避免延迟加载
                items = items.Include(h => h.Attachments).Include(h => h.Communitie)
                    .Include(nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(nameof(HouseEntity.Communitie) + "." + nameof(CommunitieEntity.Region))
                    .Include(h => h.DecorateStatus)
                    .Include(h => h.HousePics)
                    .Include(h => h.RoomType)
                    .Include(h => h.Status)
                    .Include(h => h.Type).Include(h => h.Attachments);


                //查询出来之后排序
                switch (options.OrderByType)
                {
                    case HouseSearchOrderByType.AreaAsc:
                        items = items.OrderBy(t => t.Area);
                        break;
                    case HouseSearchOrderByType.AreaDesc:
                        items = items.OrderByDescending(t => t.Area);
                        break;
                    case HouseSearchOrderByType.CreateDateDesc:
                        items = items.OrderByDescending(t => t.CreateDateTime);
                        break;
                    case HouseSearchOrderByType.MonthRentAsc:
                        items = items.OrderBy(t => t.MonthRent);
                        break;
                    case HouseSearchOrderByType.MonthRentDesc:
                        items = items.OrderByDescending(t => t.MonthRent);
                        break;
                }

                //一定不要items.Where
                //而要items=items.Where();
                //OrderBy要在Skip和Take之前
                //给用户看的页码从1开始，程序中是从0开始

                //分页
                //假如pagesize=10
                //当前页是第1页，跳过0条，取pagesize条
                //          2        10        10
                items = items.Skip((options.CurrentIndex - 1) * options.PageSize)
                    .Take(options.PageSize);


                //返回结果，返回结果类型为HouseSearchResult
                //在这里遍历数据的时候才会真正查询
                HouseSearchResult searchResult = new HouseSearchResult();
                searchResult.totalCount = totalcount;
                List<HouseDTO> houses = new List<HouseDTO>();
                foreach (var item in items)
                {
                    houses.Add(ToDTO(item));
                }
                searchResult.result = houses.ToArray();
                return searchResult;
            }
        }

        public void Update(HouseDTO house)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                //先查询再更新
                HouseEntity entity = bs.GetById(house.Id);
                entity.Address = house.Address;
                entity.Area = house.Area;
                //2,3,4
                entity.Attachments.Clear();//先删再加
                //先清空配套设施再添加新的配套设施
                var atts = ctx.Attachments.Where(a => a.IsDeleted == false &&
                    house.AttachmentIds.Contains(a.Id));
                foreach (AttachmentEntity att in atts)
                {
                    entity.Attachments.Add(att);
                }
                //3,4,5
                entity.CheckinDateTime = house.CheckInDateTime;
                entity.CommunityId = house.CommunityId;
                entity.DecorateStatusId = house.DecorateStatusId;
                entity.Description = house.Description;
                entity.Direction = house.Direction;
                entity.FloorIndex = house.FloorIndex;
                entity.LookableDateTime = house.LookableDateTime;
                entity.MonthRent = house.MonthRent;
                entity.OwnerName = house.OwnerName;
                entity.OwnerPhoneNum = house.OwnerPhoneNum;
                entity.RoomTypeId = house.RoomTypeId;
                entity.StatusId = house.StatusId;
                entity.TotalFloorCount = house.TotalFloorCount;
                entity.TypeId = house.TypeId;
                ctx.SaveChanges();
            }
        }

        private HouseDTO ToDTO(HouseEntity entity)
        {
            HouseDTO dto = new HouseDTO();
            dto.Address = entity.Address;
            dto.Area = entity.Area;
            dto.AttachmentIds = entity.Attachments.Select(a => a.Id).ToArray();
            //从HouseEntity中的Attachenments查找到id,转为数组
            dto.CheckInDateTime = entity.CheckinDateTime;
            dto.CityId = entity.Communitie.Region.CityId;
            dto.CityName = entity.Communitie.Region.City.Name;
            dto.CommunityBuiltYear = entity.Communitie.BuiltYear;
            dto.CommunityId = entity.CommunityId;
            dto.CommunityLocation = entity.Communitie.Location;
            dto.CommunityName = entity.Communitie.Name;
            dto.CommunityTraffic = entity.Communitie.Traffic;
            dto.CreateDateTime = entity.CreateDateTime;
            dto.DecorateStatusId = entity.DecorateStatusId;
            dto.DecorateStatusName = entity.DecorateStatus.Name;
            dto.Description = entity.Description;
            dto.Direction = entity.Direction;
            //看第一张图
            var firstPic = entity.HousePics.FirstOrDefault();
            //先判断有没有图片
            if (firstPic != null)
            {
                dto.FirstThumbUrl = firstPic.ThumbUrl;
            }
            dto.FloorIndex = entity.FloorIndex;
            dto.Id = entity.Id;
            dto.LookableDateTime = entity.LookableDateTime;
            dto.MonthRent = entity.MonthRent;
            dto.OwnerName = entity.OwnerName;
            dto.OwnerPhoneNum = entity.OwnerPhoneNum;
            dto.RegionId = entity.Communitie.RegionId;
            dto.RegionName = entity.Communitie.Region.Name;
            dto.RoomTypeId = entity.RoomTypeId;
            dto.RoomTypeName = entity.RoomType.Name;
            dto.StatusId = entity.StatusId;
            dto.StatusName = entity.Status.Name;
            dto.TotalFloorCount = entity.TotalFloorCount;
            dto.TypeId = entity.TypeId;
            dto.TypeName = entity.Type.Name;
            return dto;
        }

    }
}
