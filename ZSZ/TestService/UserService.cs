using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestIService;

namespace TestService
{
    public class UserService : IUserService
    {
        ////当前Servie依赖于别的Service
        public INewsService newsService { get; set; }
        public bool CheckLogin(string name, string pwd)
        {
            newsService.AddNews(name,pwd);
            return true;
        }

        public bool CheckUserNameExists(string username)
        {
            return false;
        }
    }
}
