using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 锁
{
    class Program
    {
        //纯EF的写法
        //必须在Entity先把rowversion配置byte[] row
        //必须在config中指定IsRowVersion
        static void Main(string[] args)
        {
            string bf = Console.ReadLine();
            using (MyDbContext ctx = new MyDbContext())
            {
                ctx.Database.Log = (sql) =>
                {
                    Console.WriteLine(sql);
                };
                var g = ctx.Girls.First();
                if (!string.IsNullOrEmpty(g.BFName))
                {
                    if (g.BFName == bf)
                    {
                        Console.WriteLine("早已经是你的人了呀，还抢啥？");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("来晚了，早就被别人抢走了");
                        Console.ReadKey();
                        return;
                    }
                }
                Console.WriteLine("点击任意键，开抢（模拟耗时等待并发）");
                Console.ReadKey();
                g.BFName = bf;
                try
                {
                    ctx.SaveChanges();
                    Console.WriteLine("抢媳妇成功");
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("抢媳妇失败");
                }
            }
            Console.ReadKey();
        }
            static void Main4(string[] args)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                Console.WriteLine("请输入你的名字");
                string bf = Console.ReadLine();
                var g = ctx.Database.SqlQuery<Girl>("select * from T_Girls where id=1").Single();
                byte[] rowver = g.rowver;
                if (!string.IsNullOrEmpty(g.BFName))
                {
                    if (g.BFName == bf)
                    {
                        Console.WriteLine("早已经是你的人了呀，还抢啥？");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("来晚了，早就被别人抢走了");
                        Console.ReadKey();
                        return;
                    }
                }
                Console.WriteLine("点击任意键，开抢（模拟耗时等待并发）");
                Console.ReadKey();
                Thread.Sleep(3000);
                int affectRow = ctx.Database.ExecuteSqlCommand("update T_Girls set BFName={0} where id=1 and RowVer={1}",
                    bf, rowver);
                if (affectRow == 0)
                {
                    Console.WriteLine("抢媳妇失败");
                }
                else if (affectRow == 1)
                {
                    Console.WriteLine("抢媳妇成功");
                }
                else
                {
                    Console.WriteLine("什么鬼");
                }
            }

            Console.ReadKey();
        }
        static void Main3(string[] args)
        {
          
        }
        static void Main1(string[] args)
        {
            Console.WriteLine("请输入您的名字：");
            string myname = Console.ReadLine();
            string connstr = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                //创建一个事务，要么全部成功，要么全部失败
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        Console.WriteLine("开始查询");
                        using (var selectCmd = conn.CreateCommand())
                        {
                            selectCmd.Transaction = tx;
                            selectCmd.CommandText = "select * from T_Girls with(xlock,ROWLOCK) where id=1";//对这行数据加上排他锁，即行锁
                            using (var reader = selectCmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    Console.WriteLine("没有id为1的女孩");
                                    return;
                                }
                                string bf = null;
                                if (!reader.IsDBNull(reader.GetOrdinal("BFName")))
                                {
                                    bf = reader.GetString(reader.GetOrdinal("BFName"));
                                }
                                if (!string.IsNullOrEmpty(bf))//已经有男朋友
                                {
                                    if (bf == myname)
                                    {
                                        Console.WriteLine("早已经是我的人了");
                                    }
                                    else
                                    {
                                        Console.WriteLine("早已经被" + bf + "抢走了");
                                    }
                                    Console.ReadKey();
                                    return;
                                }
                                //如果 bf==null，则继续向下抢
                            }
                            Console.WriteLine("查询完成，开始 update");
                            using (var updateCmd = conn.CreateCommand())
                            {
                                updateCmd.Transaction = tx;
                                updateCmd.CommandText = "Update T_Girls set BFName=@bf where id=1";
                                updateCmd.Parameters.Add(new SqlParameter("@bf", myname));
                                updateCmd.ExecuteNonQuery();
                            }
                            Console.WriteLine("结束 Update");
                            Console.WriteLine("按任意键结束事务");
                            Console.ReadKey();
                        }
                        tx.Commit();//事务结束，锁才解开
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        tx.Rollback();
                    }
                }
            }
        }
    }
}
