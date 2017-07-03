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
    public class RegionService : IRegionService
    {
        private RegionDTO ToDTO(RegionEntity region)
        {
            RegionDTO dto = new RegionDTO();
            dto.CityId = region.CityId;
            dto.CityName = region.City.Name;
            dto.CreateDateTime = region.CreateDateTime;
            dto.Id = region.Id;
            dto.Name = region.Name;
            return dto;
        }
        public RegionDTO[] GetAll(long cityId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RegionEntity> bs
                    = new BaseService<RegionEntity>(ctx);
                return bs.GetAll().Include(r => r.City)
                    .Where(r => r.CityId == cityId).ToList()
                    .Select(r => ToDTO(r)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
            }
        }

        public RegionDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RegionEntity> bs
                    = new BaseService<RegionEntity>(ctx);
                var region = bs.GetAll().Include(r => r.City)
                    .SingleOrDefault(r => r.Id == id);
                return region == null ? null : ToDTO(region);
            }
        }
    }
}
