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
    public class CityService : ICityService
    {

        /// <summary>
        /// 新增城市，返回新增的城市的ID
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public long AddNew(string cityName)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<CitiesEntity> bs = new BaseService<CitiesEntity>(ctx);
                //判断是否已经存在cityname名字的城市
                //where(c=>c.Name==cityName).Count()>0,数据大时效率略低
                //只是判断是否存在用any比where再count效率高
                bool exists = bs.GetAll().Any(c=>c.Name==cityName);
                if (exists)
                {
                    throw new ArgumentException("城市已经存在");
                }
                CitiesEntity city = new CitiesEntity();
                city.Name = cityName;
                ctx.Cities.Add(city);
                ctx.SaveChanges();
                //EF对自动增长的列SaveChanges之后，会把自动增长的列的值赋值回去
                return city.Id;
            }
        }
        private CityDTO ToDTO(CitiesEntity ce)
        {
            CityDTO cdto = new CityDTO();
            cdto.CreateDateTime = ce.CreateDateTime;
            cdto.Id = ce.Id;
            cdto.Name = ce.Name;
            return cdto;
        }

        public CityDTO[] GetAll()
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<CitiesEntity> bs = new BaseService<CitiesEntity>(ctx);
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
                //return bs.GetAll().AsNoTracking() .Select(c => ToDTO(c)).ToArray();
                return bs.GetAll().AsNoTracking().ToList().Select(c => ToDTO(c)).ToArray();
            }
        }

        public CityDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<CitiesEntity> bs = new BaseService<CitiesEntity>(ctx);
                var city = bs.GetById(id);
                if (city==null)
                {
                    return null;
                }
                return ToDTO(city);
            }
        }
    }
}
