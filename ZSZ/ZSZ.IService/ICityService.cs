using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;

namespace ZSZ.IService
{
    public interface ICityService:IServiceSupport
    {
        /// <summary>
        /// 新增城市，返回新增的城市的ID
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        long AddNew(string cityName);
        //根据ID获取城市DTO
        CityDTO GetById(long id);
        //获取所有城市
        CityDTO[] GetAll();
    }
}
