using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ZSZ.CommonMVC
{
    public class ZSZSMSSender
    {
        public string UserName { get; set; }
        public string AppKey { get; set; }
        public ZSZSMSResult SendSMS(string templateId, string code, string phoneNum)
        {
            WebClient wc = new WebClient();
            string url = "http://sms.rupeng.cn/SendSms.ashx?userName=" +
                Uri.EscapeDataString(UserName) + "&appKey=" + Uri.EscapeDataString(AppKey) +
                "&templateId=" + Uri.EscapeDataString(templateId) + "&code=" + Uri.EscapeDataString(code) +
                "&phoneNum=" + Uri.EscapeDataString(phoneNum);
            wc.Encoding = Encoding.UTF8;
            string resp = wc.DownloadString(url);
            //发出一个http请求（Get）返回值为响应报文体
            //返回的结果是json,写一个对应的类就好了
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ZSZSMSResult result = jss.Deserialize<ZSZSMSResult>(resp);
            //将json对象反序列化为.net对象
            return result;
        }
    }
}
