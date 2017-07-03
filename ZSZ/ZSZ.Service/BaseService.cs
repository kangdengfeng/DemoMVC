using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    //CommonService
    //其它层不能调用
    //IQueryable等是EF的对象，别的层不能调用
    //实体操作类
    //泛型约束
    //class BaseService<T>:IServiceSupport where T : BaseEntity??????
    class BaseService<T> where T : BaseEntity
    {
        //dbcontext传递给下面的方法使用，下面的方法决定不了什么时候关闭
        //延迟加载,遍历的时候才会加载数据
        //在方法里面using,就把连接关闭了
        //延迟加载就取不到数据了
        private MyDbContext ctx;
        public BaseService(MyDbContext ctx1)
        {
            this.ctx = ctx1;
        }
        //通用的增删改查代码
        //如果返回IEnumerable，之后的操作就会在内存中操作
        //对服务器压力大

        /// <summary>
        /// 获取所有没有被软删除的数据
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
            return ctx.Set<T>().Where(e => e.IsDeleted == false);
        }

        /// <summary>
        /// 获取总数据条数
        /// </summary>
        /// <returns></returns>
        public long GetTotalCount()
        {
            return GetAll().LongCount();
            //最终编译为select count的SQL
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<T> GetPageData(int startIndex, int count)
        {
            //EF中对数据分页获取需要先排序(Orderby)
            return GetAll().OrderBy(e => e.CreateDateTime)
                .Skip(startIndex).Take(count);
        }

        /// <summary>
        /// 根据Id查询.如果找不到返回null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById(long id)
        {
            //GetAll已经包含一个where,过滤掉了被软删除的数据
            return GetAll().Where(e => e.Id == id).SingleOrDefault();
            //id查出来的数据只有一条single，Default表示查不到数据的时候不会抛异常
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        public void MarkDeleted(long id)
        {
            var data = GetById(id);
            data.IsDeleted = true;
            ctx.SaveChanges();
        }
    }
}
