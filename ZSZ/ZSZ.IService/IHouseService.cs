using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;

namespace ZSZ.IService
{
    public interface IHouseService : IServiceSupport
    {
        HouseDTO[] GetAll();
        //新增房源，返回房源id
        //long AddNew(HouseDTO house);
        long AddNew(HouseAddnewDTO house);
        //添加房源图片
        long AddNewHousePic(HousePicDTO housePic);

        //软删除房源图片
        void DeleteHousePic(long housePicId);
        HouseDTO GetById(long id);
        //获取cityId的城市下，起止时间内房屋的访问数量
        long GetCount(long cityId, DateTime startDateTome, DateTime endDateTime);
        //分页获取typeId这种房源类别下cityId这个城市中房源
        HouseDTO[] GetPagedData(long cityId, long typeId, int pageSize, int currentIndex);
        //得到房源的所有图片
        HousePicDTO[] GetPics(long houseId);
        //获取typeId这种房源类别下cityId这个城市中房源的总数量
        long GetTotalCount(long cityId, long typeId);
        //软删除
        void MarkDeleted(long id);
        //更新房源，房源的附件先删除再新增
        void Update(HouseDTO house);
        //搜索，返回值包含：总条数和HouseDTO[] 两个属性
        HouseSearchResult Search(HouseSearchOptions options);















        /// <summary>
        /// 得到cityId这个城市今天的新增房源的数量
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        int GetTodayNewHouseCount(long cityId);
    }
}
