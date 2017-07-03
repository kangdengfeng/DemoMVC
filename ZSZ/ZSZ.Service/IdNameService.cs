using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class IdNameService : IIdNameService
    {
        public long AddNew(string typeName, string name)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                IdNameEntity idName =
                    new IdNameEntity { Name = name, TypeName = typeName };

                //todo:检查重复性
                ctx.IdNames.Add(idName);
                ctx.SaveChanges();
                return idName.Id;
            }
        }

        private IdNameDTO ToDTO(IdNameEntity entity)
        {
            IdNameDTO dto = new IdNameDTO();
            dto.CreateDateTime = entity.CreateDateTime;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.TypeName = entity.TypeName;
            return dto;
        }

        public IdNameDTO[] GetAll(string typeName)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<IdNameEntity> bs
                    = new BaseService<IdNameEntity>(ctx);
                return bs.GetAll().Where(e => e.TypeName == typeName)
                    .ToList().Select(e => ToDTO(e)).ToArray();
                //EF有可能翻译不成SQL语句，通过ToList拿到内存中操作。
            }
        }

        public IdNameDTO GetById(long id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<IdNameEntity> bs
                    = new BaseService<IdNameEntity>(ctx);
                return ToDTO(bs.GetById(id));
            }
        }
    }
}
