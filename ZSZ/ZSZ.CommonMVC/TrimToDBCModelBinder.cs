using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ZSZ.CommonMVC
{
    public class TrimToDBCModelBinder : DefaultModelBinder
    {
        //override父类的方法
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            #region 原来的
            /*
            //调用父类的方法，返回值是object,返回绑定的值
            object value = base.BindModel(controllerContext, bindingContext);
            //判断是否是字符串，是字符串就进行去空格处理
            if (value is string)
            {
                string strvalue = (string)value;
                strvalue = ToDBC(strvalue).Trim();
                return strvalue;
            }
            else
            {
                return value;
            }
            */
            #endregion
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            //string[] strs = (string[])valueResult.RawValue;
            if (valueResult==null)
            {
                //为null不知道怎么处理，让父类去处理
                return base.BindModel(controllerContext,bindingContext);
                //throw new Exception("没有给参数"+bindingContext.ModelName+"赋值");
            }
            string rawValue = valueResult.AttemptedValue;
            if (rawValue == null)
            {
                return null;
            }
            else
            {
                string value = ToDBC(rawValue.Trim());
                //把value转换成bindingContext.ModelType类型
                object finalValue = Convert.ChangeType(value,bindingContext.ModelType);
                return finalValue;
                //return ToBdc(rawValue.Trim());
            }
        }

        /// <summary> 全角转半角的函数(DBC case) </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        private static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }
    }
}
