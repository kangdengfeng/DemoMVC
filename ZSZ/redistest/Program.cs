using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace redistest
{
    class Program
    {
        static void Main(string[] args)
        {
            PooledRedisClientManager redisMgr = new PooledRedisClientManager("127.0.0.1");
            using (IRedisClient redisClient = redisMgr.GetClient())
            {
                var p = new Person { Id = 3, Name = "yzk" };
                redisClient.Set("p", p);
                var p1 = redisClient.Get<Person>("p");
                //return Content(p1.Name);
                Console.WriteLine(p1.Name);
                Console.ReadKey();
            }
        }
    }
}
