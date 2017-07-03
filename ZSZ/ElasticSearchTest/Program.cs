using PlainElastic.Net;
using PlainElastic.Net.Queries;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //查询
            ElasticConnection client = new ElasticConnection("localhost", 9200);
            SearchCommand cmd = new SearchCommand("zsz", "persons");//要查询的数据库，表名字
            var query = new QueryBuilder<Person>()
            .Query(b =>
            b.Bool(m =>
            //并且关系
            m.Must(t =>
            //分词的最小单位或关系查询
            t.QueryString(t1 => t1.DefaultField("Name").Query("花"))//Name字段必须含有“花”
            )
            )
            )
            /*
            //分页
            .From(0)
            .Size(10)
            //排序
            // .Sort(c => c.Field("age", SortDirection.desc))
            //添加高亮
            .Highlight(h => h
            .PreTags("<b>")
            .PostTags("</b>")
            .Fields(
            f => f.FieldName("Name").Order(HighlightOrder.score)
            )
            )
            */
            .Build();
            var result = client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var searchResult = serializer.ToSearchResult<Person>(result);
            //searchResult.hits.total;//一共有多少匹配结果
            //searchResult.Documents;//当前页的查询结果
            foreach (var doc in searchResult.Documents)
            {
                Console.WriteLine(doc.Id+","+doc.Name+","+doc.Age);//2,一朵花,18
            }
            Console.ReadKey();

        }
        static void Main2(string[] args)
        {
            //插入数据
            /*
            Person p1 = new Person();
            p1.Id = 1;
            p1.Age = 10;
            p1.Name = "啦啦啦啦啦";
            p1.Description = "描述";
            */
            Person p1 = new Person();
            p1.Id = 2;
            p1.Age = 18;
            p1.Name = "一朵花";
            p1.Description = "买买买";
            ElasticConnection client = new ElasticConnection("localhost", 9200);
            var serializer = new JsonNetSerializer();
            //第一个参数相当于“数据库”，第二个参数相当于“表”，第三个参数相当于“主键”
            IndexCommand cmd = new IndexCommand("zsz", "persons", p1.Id.ToString());
            //不用手动创建数据库，es会自动分配空间用zsz命名
            //Put()第二个参数是要插入的数据
            OperationResult result = client.Put(cmd, serializer.Serialize(p1));//把对象序列化成json放入Elastic中返回结果
            var indexResult = serializer.ToIndexResult(result.Result);//把json字符串解析成字符串
            if (indexResult.created)
            {
                Console.WriteLine("创建了");
            }
            else
            {
                Console.WriteLine("没创建" + indexResult.error);
            }
            Console.ReadKey();
        }



    }
}
