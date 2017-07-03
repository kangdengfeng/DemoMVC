using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class PermissionService : IPermissionService
    {
        public void AddPermIds(long roleId, long[] permIds)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RoleEntity> roleBS = new BaseService<RoleEntity>(ctx);
                var role = roleBS.GetById(roleId);
                if (role == null)
                {
                    throw new ArgumentException("roleId不存在" + roleId);
                }
                BaseService<PermissionEntity> permBS = new BaseService<PermissionEntity>(ctx);
                var perms = permBS.GetAll().Where(p => permIds.Contains(p.Id)).ToArray();
                foreach (var perm in perms)
                {
                    role.Perminssions.Add(perm);
                }
                ctx.SaveChanges();
            }
        }

        public long AddPermission(string permName, string description)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<PermissionEntity> perBS = new BaseService<PermissionEntity>(ctx);
                bool exists = perBS.GetAll().Any(p => p.Name == permName);
                if (exists)
                {
                    throw new ArgumentException("权限项已经存在");
                }
                PermissionEntity permEntity = new PermissionEntity();
                permEntity.Description = description;
                permEntity.Name = permName;
                ctx.Permissions.Add(permEntity);
                ctx.SaveChanges();
                return permEntity.Id;
            }
        }

        public PermissionDTO[] GetAll()
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                return bs.GetAll().ToList().Select(p => ToDTO(p)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
            }
        }

        public PermissionDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                var pe = bs.GetById(id);
                return pe == null ? null : ToDTO(pe);
            }
        }

        public PermissionDTO GetByName(string name)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                var pe = bs.GetAll().SingleOrDefault(p => p.Name == name);
                return pe == null ? null : ToDTO(pe);
            }
        }

        public PermissionDTO[] GetByRoleId(long roleId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                return bs.GetById(roleId).Perminssions.ToList().Select(p => ToDTO(p)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
            }
        }

        public void MarkDeleted(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                bs.MarkDeleted(id);
            }
        }

        //
        public void UpdatePermIds(long roleId, long[] permIds)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RoleEntity> roleBS
                    = new BaseService<RoleEntity>(ctx);
                var role = roleBS.GetById(roleId);
                if (role == null)
                {
                    throw new ArgumentException("roleId不存在" + roleId);
                }
                role.Perminssions.Clear();
                BaseService<PermissionEntity> permBS
                    = new BaseService<PermissionEntity>(ctx);
                var perms = permBS.GetAll()
                    .Where(p => permIds.Contains(p.Id)).ToList();
                foreach (var perm in perms)
                {
                    role.Perminssions.Add(perm);
                }
                ctx.SaveChanges();
            }
        }

        public void UpdatePermIds(long id, string permName, string description)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                var perm = bs.GetById(id);
                if (perm == null)
                {
                    throw new ArgumentException("id不存在" + id);
                }
                perm.Name = permName;
                perm.Description = description;
                ctx.SaveChanges();
            }
        }

        private PermissionDTO ToDTO(PermissionEntity p)
        {
            PermissionDTO dto = new PermissionDTO();
            dto.CreateDateTime = p.CreateDateTime;
            dto.Description = p.Description;
            dto.Id = p.Id;
            dto.Name = p.Name;
            return dto;
        }
    }
}
