using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.Service;

namespace ServiceTests
{
    [TestClass]
    public class HouseAppintmentServiceUnit
    {
        HouseAppointmentService houseAppSvc = new HouseAppointmentService();
        HouseService houseSvc = new HouseService();
        [TestMethod]
        public void TestHouseApp1()
        {
            HouseAddnewDTO newDto = new HouseAddnewDTO();
            newDto.Address = "8号楼";
            newDto.Area = 80;
            newDto.AttachmentIds = new long[] { 1, 2 };
            newDto.CheckInDateTime = DateTime.Now;
            newDto.CommunityId = 2;
            newDto.DecorateStatusId = 5;
            newDto.Description = "房东好人";
            newDto.Direction = "朝南";
            newDto.FloorIndex = 9;
            newDto.TotalFloorCount = 16;
            newDto.LookableDateTime = DateTime.Now;
            newDto.MonthRent = 6200;
            newDto.OwnerName = "刘老师";
            newDto.OwnerPhoneNum = "18918918189";
            newDto.RoomTypeId = 7;
            newDto.StatusId = 10;
            newDto.TypeId = 11;

            long houseId = houseSvc.AddNew(newDto);

            var h = houseSvc.GetById(houseId);
            Assert.AreEqual(h.Address, newDto.Address);
            Assert.AreEqual(h.AttachmentIds, newDto.AttachmentIds);
            CollectionAssert.AreEqual(h.AttachmentIds, newDto.AttachmentIds);
            Assert.AreEqual(h.CityName, "北京");
            Assert.AreEqual(h.DecorateStatusName, "精装修");

            long pic1 = houseSvc.AddNewHousePic(new HousePicDTO { HouseId = houseId, ThumbUrl = "suo.jpg", Url = "url1.jpg" });
            long pic2 = houseSvc.AddNewHousePic(new HousePicDTO { HouseId = houseId, ThumbUrl = "suo22.jpg", Url = "ur22.jpg" });

            CollectionAssert.AreEqual(houseSvc.GetPics(houseId).Select(p => p.Id).ToArray(), new long[] { pic1, pic2 });

        }
    }
}
