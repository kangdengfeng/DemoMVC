using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Memcachedtest
{
    class Program
    {
        static void Main(string[] args)
        {
            MemcachedClientConfiguration config = new MemcachedClientConfiguration();
            //Memcached的配置

            //config.Servers.Add(new IPEndPoint(IPAddress.Loopback, 11211));
            config.Servers.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11211));
            //要连接到哪台服务器,可以连接到多台服务器  
            //11211  是Memcached默认端口

            config.Protocol = MemcachedProtocol.Binary;
            //Binary 通过什么方式（Binary）序列化这个类,要求这个类必须标记为可序列化
            //默认是Binary，text是用json保存


            MemcachedClient client = new MemcachedClient(config);
            var p = new Person { Id = 3, Name = "kdf" };

            //保存到缓存中
            //client.Store(Enyim.Caching.Memcached.StoreMode.Set, "p" + p.Id, p);
            //DateTime dt = DateTime.Now.AddSeconds(3);
            //client.Store(StoreMode.Set, "p" + p.Id, p, DateTime.Now.AddSeconds(3));//会报错
            client.Store(StoreMode.Set, "p" + p.Id, p, TimeSpan.FromSeconds(3));
            //还可以指定第四个参数指定数据的过期时间。
            //"p" + p.Id为key,p为value
            //DateTime.Now.AddSeconds(10) 缓存10秒钟

            //Thread.Sleep(2000);


            Person p1 = client.Get<Person>("p3");
            ////内置缓存取数据：HttpContext.Cache[cachekey];
            //Console.WriteLine(p1.Name);

            //Thread.Sleep(2000);
            //p1 = client.Get<Person>("p3");

            Console.WriteLine(p1.Name);
            Console.ReadKey();
        }
    }
}
