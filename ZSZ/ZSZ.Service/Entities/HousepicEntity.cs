using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    public class HousepicEntity : BaseEntity
    {
        public long HouseId { get; set; }
        public string Url { get; set; }
        public string ThumbUrl { get; set; }
        public virtual HouseEntity House { get; set; }

    }
}
