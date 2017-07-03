using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.CommonMVC
{
    public class Pager
    {

        /// <summary>
        /// 每一页数据的条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总数据条数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 显示出来的页码的最多个数
        /// </summary>
        public int MaxPagerCount { get; set; }
        /// <summary>
        /// 当前的页码(从1开始算起始页)
        /// </summary>
        public int PageIndex { get; set;}
        /// <summary>
        /// 链接的格式，约定其中页码用{pn}占位符
        /// </summary>
        public string UrlPattern { get; set; }
        /// <summary>
        /// 当前的页码的样式名字
        /// </summary>
        public string CurrentPageClassName { get; set; }
        public string GetPagerHtml()
        {
            StringBuilder html = new StringBuilder();
           
            //首页、末页、上一页、下一页、跳转到N页

            //总页数=总数据条数/每页数据条数
            int pageCount = (int)Math.Ceiling(TotalCount*1.0/PageSize);
            //显示出来的页码的其实页码
            int startPageIndex = Math.Max(1,PageIndex-MaxPagerCount/2);
            //显示出来的页码的结束页码
            int endPageIndex = Math.Min(pageCount,startPageIndex+MaxPagerCount-1);

            html.Append("<ul>");
            for (int i = startPageIndex; i <= endPageIndex; i++)
            {
                //判断是不是当前页
                if (i==PageIndex)
                {
                    html.Append("<li class='").Append(CurrentPageClassName).Append("'>").Append(i).Append("</li>");
                }
                else
                {
                    html.Append("<li><a href='").Append(UrlPattern.Replace("{pn}", i.ToString()))
                        .Append("'>").Append(i).Append("</a>")
                        .Append("</li>");
                }
            }
            
            html.Append("</ul>");
            return html.ToString();
        }
    }
}
