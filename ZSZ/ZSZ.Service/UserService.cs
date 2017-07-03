using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Common;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class UserService : IUserService
    {
        public long AddNew(string phoneNum, string password)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<UserEntity> userBS = new BaseService<UserEntity>(ctx);
                //检查手机号不能重复
                bool exists = userBS.GetAll().Any(u => u.PhoneNum == phoneNum);
                if (exists)
                {
                    throw new ArgumentException("手机号已经存在");
                }
                UserEntity uentity = new UserEntity();
                uentity.PhoneNum = phoneNum;
                string salt = CommonHelper.CreateVerifyCode(5);
                string pwdHash = CommonHelper.CalcMD5(salt + password);
                uentity.PasswordHash = pwdHash;
                uentity.PasswordSalt = salt;
                ctx.Users.Add(uentity);
                ctx.SaveChanges();
                return uentity.Id;
            }
        }

        public bool CheckLogin(string phoneNum, string password)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<UserEntity> ue = new BaseService<UserEntity>(ctx);
                var user = ue.GetAll().SingleOrDefault(u => u.PhoneNum == phoneNum);
                if (user == null)
                {
                    return false;
                }
                else
                {
                    string dbPwdHash = user.PasswordHash;
                    string salt = user.PasswordSalt;
                    string userPwdHash = CommonHelper.CalcMD5(salt + password);
                    return dbPwdHash == userPwdHash;
                }

            }
        }
        private UserDTO ToDTO(UserEntity user)
        {
            UserDTO dto = new UserDTO();
            dto.CityId = user.CityId;
            dto.CreateDateTime = user.CreateDateTime;
            dto.Id = user.Id;
            dto.LastLoginErrorDateTime = user.LastLoginErrorDateTime;
            dto.LoginErrorTimes = user.LoginErrorTimes;
            dto.PhoneNum = user.PhoneNum;
            return dto;
        }
        public UserDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetById(id);
                return user == null ? null : ToDTO(user);
            }
        }

        public UserDTO GetByPhoneNum(string phoneNum)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetAll().SingleOrDefault(u => u.PhoneNum == phoneNum);
                return user == null ? null : ToDTO(user);
            }
        }

        public void SetUserCityId(long userId, long cityId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetById(userId);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在");
                }
                user.CityId = cityId;
                ctx.SaveChanges();
            }
        }

        public void UpdatePwd(long userId, string newPassword)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                //检查手机号不能重复
                var user = bs.GetById(userId);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在");
                }
                string salt = user.PasswordSalt;// CommonHelper.CreateVerifyCode(5);
                string pwdHash = CommonHelper.CalcMD5(salt + newPassword);
                user.PasswordHash = pwdHash;
                user.PasswordSalt = salt;
                ctx.SaveChanges();
            }
        }

        public void IncrLoginError(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                //检查手机号不能重复
                var user = bs.GetById(id);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在");
                }
                user.LoginErrorTimes++;
                user.LastLoginErrorDateTime = DateTime.Now;
                ctx.SaveChanges();
            }
        }

        public void ResetLoginError(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                //检查手机号不能重复
                var user = bs.GetById(id);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在");
                }
                user.LoginErrorTimes = 0;
                user.LastLoginErrorDateTime = null;
                ctx.SaveChanges();
            }
        }

        public bool IsLocked(long id)
        {
            //判断是否已经锁定
            var user = GetById(id);
           //错误登录次数>=5，最后一次登陆错误时间在30分钟之内
            return (user.LoginErrorTimes >= 5 
                && user.LastLoginErrorDateTime > DateTime.Now.AddMinutes(-30));

        }
    }
}
