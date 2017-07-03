using PlainElastic.Net;
using PlainElastic.Net.Queries;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esTest2verycd
{
    class Program
    {
        static void Main(string[] args)
        {
            ////模糊查询
            //ElasticConnection client = new ElasticConnection("localhost", 9200);
            //SearchCommand cmd = new SearchCommand("verycd", "items");//要查询的数据库，表名字
            //var query = new QueryBuilder<VerycdItem>()
            //.Query(b =>
            //b.Bool(m =>
            ////并且关系
            //m.Must(t =>
            ////分词的最小单位或关系查询
            //t.QueryString(t1 => t1.DefaultField("content").Query("成龙"))//title字段必须含有“美女”
            //)
            //)
            //)
            //.Build();
            //var result = client.Post(cmd, query);
            //var serializer = new JsonNetSerializer();
            //var searchResult = serializer.ToSearchResult<VerycdItem>(result);
            ////searchResult.hits.total;//一共有多少匹配结果
            ////searchResult.Documents;//当前页的查询结果
            //foreach (var doc in searchResult.Documents)
            //{
            //    Console.WriteLine(doc.title);//2,一朵花,18
            //}
            //Console.ReadKey();

            

                    //精确查询
                    ElasticConnection client = new ElasticConnection("localhost", 9200);
                        SearchCommand cmd = new SearchCommand("verycd", "items");//要查询的数据库，表名字
                        var query = new QueryBuilder<VerycdItem>()
                        .Query(b =>
                        b.Bool(m =>
                        //并且关系
                        m.Must(t =>
                        //分词的最小单位或关系查询
                        t.QueryString(t1 => t1.DefaultField("content").Query("成龙"))//title字段必须含有“成龙”
                        ).Must(t => t.QueryString(t1=>t1.DefaultField("category2").Query("纪录")))//默认返回最匹配的10条
                        )
                        ).Size(100)//返回匹配的100条
                        .Build();
                        var result = client.Post(cmd, query);
                        var serializer = new JsonNetSerializer();
                        var searchResult = serializer.ToSearchResult<VerycdItem>(result);
                        //searchResult.hits.total;//一共有多少匹配结果
                        //searchResult.Documents;//当前页的查询结果
                        foreach (var doc in searchResult.Documents)
                        {
                            Console.WriteLine(doc.title+","+doc.category1+","+doc.category2);//查询结果
                        }
                        Console.ReadKey();
            //must:必须   should:或者  mustnot:必须没有  
            
        }
        static void Main2(string[] args)
        {
            
            //读取VeryCD数据
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=E:\DemoMVC\电驴数据库（全部电影）\verycd.sqlite3.db"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from verycd";
                    //Dateset会把所有数据都填到内存中
                    //DataReader
                    using (var reader = cmd.ExecuteReader())
                    {
                        //创建与SQLLite的连接
                        ElasticConnection client = new ElasticConnection("localhost", 9200);
                        var serializer = new JsonNetSerializer();
                        while (reader.Read())
                        {
                            long verycdId = reader.GetInt64(reader.GetOrdinal("verycdid"));
                            string title = reader.GetString(reader.GetOrdinal("title"));
                            string status = reader.GetString(reader.GetOrdinal("status"));
                            string brief = reader.GetString(reader.GetOrdinal("brief"));
                            string pubtime = reader.GetString(reader.GetOrdinal("pubtime"));
                            string category1 = reader.GetString(reader.GetOrdinal("category1"));
                            string category2 = reader.GetString(reader.GetOrdinal("category2"));
                            string ed2k = reader.GetString(reader.GetOrdinal("ed2k"));
                            string content = reader.GetString(reader.GetOrdinal("content"));
                            string related = reader.GetString(reader.GetOrdinal("related"));

                            VerycdItem item = new VerycdItem();
                            item.verycdid = verycdId;
                            item.title = title;
                            item.status = status;
                            item.brief = brief;
                            item.pubtime = pubtime;
                            item.category1 = category1;
                            item.category2 = category2;
                            item.ed2k = ed2k;
                            item.content = content;
                            item.related = related;

                            Console.WriteLine("当前读取到id=" + verycdId);

                            //准备写入数据
                            //写入数据前先建立与SQLLite的连接，不要在循环里建立连接，每次查询都连接一次效率低
                            //写入数据
                            //第一个参数相当于“数据库”，第二个参数相当于“表”，第三个参数相当于“主键”
                            IndexCommand indexcmd = new IndexCommand("verycd", "items", verycdId.ToString());
                            //不用手动创建数据库，es会自动分配空间用zsz命名
                            //Put()第二个参数是要插入的数据
                            OperationResult result = client.Put(indexcmd, serializer.Serialize(item));//把对象序列化成json放入Elastic中返回结果
                            var indexResult = serializer.ToIndexResult(result.Result);//把json字符串解析成字符串
                            if (indexResult.created)
                            {
                                Console.WriteLine("创建了");
                            }
                            else
                            {
                                Console.WriteLine("没创建" + indexResult.error);
                            }
                        }
                        
                    }
                }
            }

                Console.ReadKey();
        }
    }
}
