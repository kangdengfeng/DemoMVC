using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZSZ.IService;

namespace ZSZ.FrontWeb
{
    public class MemcachedMg
    {
        private MemcachedClient client;
        public static MemcachedMg Instance { get; private set; }
        //外界不能调用set方法,只有类内部能set

        //构造器，不支持6.0语法的情况下,保证有且只有一个实例
        //支持6.0可以直接public static MemcachedMg Instance { get; private set; }=new MemcachedMg();
        static MemcachedMg()
        {
            Instance = new MemcachedMg();
        }
        private MemcachedMg()
        {
            //获取配置信息
            var settingService =
                DependencyResolver.Current.GetService<ISettingService>();
            string[] servers
                = settingService.GetValue("MemCachedServers").Split(';');


            MemcachedClientConfiguration config = new MemcachedClientConfiguration();
            //Memcached的配置
            foreach (var server in servers)//获取所有的服务器地址并添加到Memcached
            {
                config.Servers.Add(new IPEndPoint(IPAddress.Parse(server), 11211));
            }
            //config.Servers.Add(new IPEndPoint(IPAddress.Loopback, 11211));
            //config.Servers.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11211));
            //要连接到哪台服务器,可以连接到多台服务器  
            //11211  是Memcached默认端口

            config.Protocol = MemcachedProtocol.Binary;
            //Binary 通过什么方式（Binary）序列化这个类,要求这个类必须标记为可序列化
            //默认是Binary，text是用json保存


            client = new MemcachedClient(config);
        }

        public void SetValue(string key, object value, TimeSpan expires)
        {
            if (!value.GetType().IsSerializable)//判断类是否可序列化
            {
                throw new ArgumentException("value必须是可序列化的对象");
            }
            client.Store(StoreMode.Set, key, value, expires);
        }
        public object GetValue(string key)
        {
            return client.Get(key);
        }
        //public T GetValue<T>(string key)
        //{
        //    return client.Get<T>(key);
        //}
        //这里如果用泛型
        //HouseController调用的时候也要用泛型
        //HouseIndexViewModel model
        //    = MemcachedMg.Instance.GetValue<HouseIndexViewModel>(cachekey);
    }
}