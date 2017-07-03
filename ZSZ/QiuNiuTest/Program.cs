using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qiniu.Util;
using Qiniu.IO.Model;
using Qiniu.IO;
using Qiniu.Http;
using Qiniu.Common;

namespace QiuNiuTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Qiniu.Common.Config.SetZone(ZoneID.CN_South, true);
            Mac mac = new Mac("2lYOOiMvton1t_i2faEFLnwG3JIMZyuj5gT9mfRP", "ulQBqveHpijWlqIGWe_ObX4ahiNnYAKyYx5M-BMl");
            // 生成(上传)凭证时需要使用此Mac
            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey
            
            string bucket = "zsztest";
            string saveKey = "upload/gf/a.jpg";
           //string saveKey = "upload/gf/a.jpg";
            string localFile = "E:\\DemoMVC\\img\\1.jpg";

            Qiniu.Common.Config.AutoZone("2lYOOiMvton1t_i2faEFLnwG3JIMZyuj5gT9mfRP", bucket, true);
           // 上传策略，参见 
           // https://developer.qiniu.com/kodo/manual/put-policy
           PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            putPolicy.DeleteAfterDays = 1;
            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            
            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);
            UploadManager um = new UploadManager();
            HttpResult result = um.UploadFile(localFile, saveKey, token);
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
