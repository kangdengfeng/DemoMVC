using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    public class AdminUserEntity:BaseEntity
    {
        public string Name { get; set; }
        public string PhoneNum { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public long? CityId { get; set; }
        public int LoginErroeTimes { get; set; }
        public DateTime? LastLoginErrorDateTime { get; set; }
        public virtual ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
        public virtual CitiesEntity City { get; set; }
    }
}
