using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    public class RoleEntity : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<PermissionEntity> Perminssions { get; set; } = new List<PermissionEntity>();

        //怎么不用virtual???
        public ICollection<AdminUserEntity> AdminUsers { get; set; } = new List<AdminUserEntity>();
    }
}
