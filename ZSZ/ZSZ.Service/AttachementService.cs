using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class AttachementService:IAttachmentService
    {
        public AttachementDTO[] GetAll()
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<AttachmentEntity> bs = new BaseService<AttachmentEntity>(ctx);
                var items = bs.GetAll().AsNoTracking();
                //asnotracking数据只用于显示
                //return items.Select(a => ToDTO(a)).ToArray();
                return items.ToList().Select(a => ToDTO(a)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
            }
        }

        public AttachementDTO[] GetAttachments(long houseId)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                var house = bs.GetAll().Include(a => a.Attachments).AsNoTracking()
                    .SingleOrDefault(h => h.Id == houseId);
                if (house==null)
                {
                    throw new ArgumentException("houseId"+house+"不存在");
                }
                return house.Attachments.ToList().Select(a => ToDTO(a)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
               // return house.Attachments.Select(a => ToDTO(a)).ToArray();
            }
        }
        private AttachementDTO ToDTO(AttachmentEntity att)
        {
            AttachementDTO dto = new AttachementDTO();
            dto.CreateDateTime = att.CreateDateTime;
            dto.IconName = att.IconName;
            dto.Id = att.Id;
            dto.Name = att.Name;
            return dto;
        }
    }
}
