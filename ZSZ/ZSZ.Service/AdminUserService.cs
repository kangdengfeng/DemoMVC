using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Common;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class AdminUserService : IAdminUserService
    {
        public long AddAdminUser(string name, string phoneNum, string password, string email, long? cityId)
        {
            AdminUserEntity user = new AdminUserEntity();
            user.Name = name;
            user.PhoneNum = phoneNum;
            user.Email = email;
            user.CityId = cityId;
            string salt = CommonHelper.CreateVerifyCode(5);//取5位字符作为盐
            user.PasswordSalt = salt;
            string pwdHash = CommonHelper.CalcMD5(salt + password);
            user.PasswordHash = pwdHash;
            using (MyDbContext ctx = new MyDbContext())
            {
                //要判断手机号是否已经存在
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                bool exists = bs.GetAll().Any(e => e.PhoneNum == phoneNum);
                if (exists == true)
                {
                    throw new ArgumentException("手机号已经存在");
                }
                ctx.AdminUsers.Add(user);
                ctx.SaveChanges();
                return user.Id;
            }
        }

        public bool CheckLogin(string phoneNum, string password)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                var user = bs.GetAll().SingleOrDefault(u => u.PhoneNum == phoneNum);
                if (user == null)//手机号不存在
                {
                    return false;//只要告诉登录失败就好了
                }
                string dbHash = user.PasswordHash;
                string userHash = CommonHelper.CalcMD5(user.PasswordSalt + password);
                //比较数据库中的PasswordHash是否和MD5(salt+用户输入密码)一致
                return userHash == dbHash;
            }
        }

        //获得所有管理员
        public AdminUserDTO[] GetAll()
        {
            //using System.Data.Entity;才能在IQuerable中用Include
            //关联属性会引发延迟加载，通过Include避免延迟加载
            //asnotracking:取出来的数据只是显示，不用修改可稍微调高性能
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                return bs.GetAll().Include(u => u.City).AsNoTracking()
                    .ToList().Select(u => ToDto(u)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
            }
        }




        private AdminUserDTO ToDto(AdminUserEntity user)
        {
            AdminUserDTO dto = new AdminUserDTO();
            dto.RoleId = user.Roles.Select(u => u.Id).ToArray();
            dto.CityId = user.CityId;
            if (user.City != null)
            {
                dto.CityName = user.City.Name;//需要Include提升性能
                //总部（北京）、上海分公司、广州分公司、北京分公司
            }
            else
            {
                dto.CityName = "总部";
            }

            dto.CreateDateTime = user.CreateDateTime;
            dto.Email = user.Email;
            dto.Id = user.Id;
            dto.LastLoginErrorDateTime = user.LastLoginErrorDateTime;
            dto.LoginErrorTimes = user.LoginErroeTimes;
            dto.Name = user.Name;
            dto.PhoneNum = user.PhoneNum;
            dto.RoleName = user.Roles.Select(u => u.Name).ToArray();
            return dto;
        }
        //获得某一城市管理员
        //如果为null就获取总部管理员
        //否则就是某个地区的
        public AdminUserDTO[] GetAll(long? cityId)
        {
            //如果ciityid为null会翻译成city is null??;city=3
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                var all = bs.GetAll().Include(u => u.City)
                    .AsNoTracking().Where(u => u.CityId == cityId);
                return all.ToList().Select(u => ToDto(u)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
            }
        }


        public AdminUserDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                //这里不能用bs.GetById(id); 因为无法Include、AsNoTracking()等
                //bs.GetById()返回的是Entity
                //var user = bs.GetById(id); 用include就不能用GetById
                var user = bs.GetAll().Include(u => u.City).Include(u=>u.Roles)
                    //.AsNoTracking().SingleOrDefault(u=>u.Id==id);
                    .AsNoTracking().Where(u => u.Id == id).SingleOrDefault();
                if (user == null)
                {
                    return null;
                    //不抛异常
                    //让调用的人处理为null怎么办
                }
                return ToDto(user);
            }
        }

        public AdminUserDTO GetByPhoneNum(string phoneNum)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                var users = bs.GetAll().Include(u => u.City)
                    .AsNoTracking().Where(u => u.PhoneNum == phoneNum);
                //统计查询出来的数据
                int count = users.Count();
                if (count <= 0)
                {
                    return null;
                }
                else if (count == 1)
                {
                    return ToDto(users.Single());
                }
                else
                {
                    throw new ApplicationException("找到多个手机号为" + phoneNum + "的管理员");
                }
            }
        }
        //HasPermission(5,"User.Add")判断5这个管理员有没有User.Add权限

        public bool HasPermission(long adminUserId, string permissionName)
        {
            //用户查看属于哪个角色，再看角色有哪些权限
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                //var user = bs.GetById(adminUserId);
                //使用了导航属性，可能会有延迟加载
                var user = bs.GetAll().Include(u => u.Roles)
                    .AsNoTracking().SingleOrDefault(u => u.Id == adminUserId);
                if (user == null)
                {
                    throw new ArgumentException("找不到Id=" + adminUserId + "的用户");
                }
                //用户和角色是多对多关系
                //通过user查询角色，通过角色查询角色权限
                //每个role都有一个Permission属性
                //selectmany遍历roles的每一个role
                //然后把每个role的Permissions放到一个集合中
                //var per = user.Roles.SelectMany(r => r.Perminssions).ToList();
                //bool result = per.Any(p => p.Description == permissionName);
                //return result;
                return user.Roles.SelectMany(r => r.Perminssions).Any(p => p.Description == permissionName);
            }
        }



        public void MarkDeleted(long adminUserId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                bs.MarkDeleted(adminUserId);
            }
        }

        //记录登录错误次数
        public void RecordLoginError(long id)
        {
            throw new NotImplementedException();
        }

        //重置登录错误次数
        public void ResetLoginError(long id)
        {
            throw new NotImplementedException();
        }

        public void UpdateAdminUser(long id, string name, string phoneNum, string password, string email, long? cityId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                //先查询再更新
                var user = bs.GetById(id);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在");
                }
                //EF会判断内容有没有改变,内容变了的会更新，内容不变的不更新
                user.Name = name;
                user.PhoneNum = phoneNum;
                user.PasswordHash = CommonHelper.CalcMD5(user.PasswordSalt + password);
                user.Email = email;
                user.CityId = cityId;
                ctx.SaveChanges();
            }
        }
    }
}
