using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class CoummunityService : ICommunityService
    {
        /*
        private CommunityDTO ToDTO(CommunitieEntity ce)
        {
            CommunityDTO ct = new CommunityDTO();
            ct.BuiltYear = ce.BuiltYear;
            return ct;
        }
        */
        public CommunityDTO[] GetByRegionId(long regionId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<CommunitieEntity> bs = new BaseService<CommunitieEntity>(ctx);
                var cities = bs.GetAll().AsNoTracking().Where(c=>c.RegionId==regionId);
                return cities.Select(c => new CommunityDTO
                {
                    BuiltYear = c.BuiltYear,
                    CreateDateTime = c.CreateDateTime,
                    Id = c.Id,
                    Location = c.Location,
                    Name = c.Name,
                    RegionId = c.RegionId,
                    Traffic = c.Traffic
                }).ToArray();
            }
        }
    }
}
