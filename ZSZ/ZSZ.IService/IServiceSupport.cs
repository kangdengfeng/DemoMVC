using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.IService
{
    //一个标识接口，所有服务都必须实现这个接口
    //从这个接口继承的都是Service,别的类就不管
    //这样注册到Global中的时候不是继承自这个接口的就不用管了
    //保证只要真正的服务实现类才会被注册到AntoFac
    public interface IServiceSupport
    {

    }
}
