using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;

namespace ZSZ.IService
{
    public interface IAttachmentService:IServiceSupport
    {
        //获取所有的设施
        AttachementDTO[] GetAll();

        //获取房子houseId有用的设施
        AttachementDTO[] GetAttachments(long houseId);
    }
}
