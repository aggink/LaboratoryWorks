using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Data.Entities.ChatEntities
{
    public class File : Auditable
    {
        public string ContentType { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public byte[] Data { get; set; } = null!;
    }
}
