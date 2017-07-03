using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.DTO
{
    //只传用得到的，用不到的不传了
    public class HouseAddnewDTO:BaseDTO
    {
        public long CommunityId { get; set; }
        public long RoomTypeId { get; set; }
        public String Address { get; set; }
        public int MonthRent { get; set; }
        public long StatusId { get; set; }
        public decimal Area { get; set; }
        public long DecorateStatusId { get; set; }
        public int TotalFloorCount { get; set; }
        public int FloorIndex { get; set; }
        public long TypeId { get; set; }
        public String Direction { get; set; }
        public DateTime LookableDateTime { get; set; }
        public DateTime CheckInDateTime { get; set; }
        public String OwnerName { get; set; }
        public String OwnerPhoneNum { get; set; }
        public String Description { get; set; }
        public long[] AttachmentIds { get; set; }
    }
}
