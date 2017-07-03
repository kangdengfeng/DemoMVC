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
    public class RoleService : IRoleService
    {
        public long AddNew(string roleName)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RoleEntity> roleBS
                    = new BaseService<RoleEntity>(ctx);
                bool exists = roleBS.GetAll().Any(r => r.Name == roleName);
                //正常情况不应该执行这个异常，因为UI层应该把这些情况处理好
                //这里只是“把好最后一关”
                if (exists)
                {
                    throw new ArgumentException("角色名字已经存在" + roleName);
                }
                RoleEntity role = new RoleEntity();
                role.Name = roleName;
                ctx.Roles.Add(role);
                ctx.SaveChanges();
                return role.Id;
            }
        }

        public void AddRoleIds(long adminUserId, long[] roleIds)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> userBS = new BaseService<AdminUserEntity>(ctx);
                var user = userBS.GetById(adminUserId);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在" + adminUserId);
                }
                BaseService<RoleEntity> roleBS = new BaseService<RoleEntity>(ctx);
                var roles = roleBS.GetAll().Where(r => roleIds.Contains(r.Id)).ToArray();
                foreach (var role in roles)
                {
                    user.Roles.Add(role);
                }
                ctx.SaveChanges();
            }
        }
        private RoleDTO ToDTO(RoleEntity en)
        {
            RoleDTO dto = new RoleDTO();
            dto.CreateDateTime = en.CreateDateTime;
            dto.Id = en.Id;
            dto.Name = en.Name;
            return dto;
        }
        public RoleDTO[] GetAll()
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RoleEntity> roleBS = new BaseService<RoleEntity>(ctx);
                return roleBS.GetAll().ToList().Select(p => ToDTO(p)).ToArray();
            }
        }

        public RoleDTO[] GetByAdminUserId(long adminUserId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> ubs = new BaseService<AdminUserEntity>(ctx);
                var user = ubs.GetById(adminUserId);
                if (user == null)
                {
                    throw new ArgumentException("不存在的管理员" + adminUserId);
                }
                return user.Roles.ToList().Select(r => ToDTO(r)).ToArray();
            }
        }

        public RoleDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                var role = bs.GetById(id);
                return role == null ? null : ToDTO(role);
            }
        }

        public RoleDTO GetByName(string name)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                var role = bs.GetAll().SingleOrDefault(r => r.Name == name);
                return role == null ? null : ToDTO(role);
            }
        }

        public void MarkDeleted(long roleId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                bs.MarkDeleted(roleId);
            }
        }

        public void Update(long roleId, string roleName)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                //判断修改的内容是否重复，是否和当前一样
                BaseService<RoleEntity> roleBS = new BaseService<RoleEntity>(ctx);
                bool exists = roleBS.GetAll().Any(r => r.Name == roleName && r.Id != roleId);
                //正常情况不应该执行这个异常，因为UI层应该把这些情况处理好(ajax)
                if (exists)
                {
                    throw new ArgumentException("");
                }
                RoleEntity role = new RoleEntity();
                role.Id = roleId;
                ctx.Entry(role).State = System.Data.Entity.EntityState.Unchanged;
                role.Name = roleName;
                ctx.SaveChanges();
            }
        }

        public void UpdateRoleIds(long adminUserId, long[] roleIds)
        {
            //先删再加
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> abs = new BaseService<AdminUserEntity>(ctx);
                var user = abs.GetById(adminUserId);
                if (user==null)
                {
                    throw new ArgumentException("用户不存在"+adminUserId);
                }
                user.Roles.Clear();
                BaseService<RoleEntity> roleBS = new BaseService<RoleEntity>(ctx);
                var roles = roleBS.GetAll().Where(r => roleIds.Contains(r.Id)).ToArray();
                foreach (var role in roles)
                {
                    user.Roles.Add(role);
                }
                ctx.SaveChanges();
            }
        }
    }
}
