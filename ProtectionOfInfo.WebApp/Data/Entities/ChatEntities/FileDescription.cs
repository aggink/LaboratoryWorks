using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System;

namespace ProtectionOfInfo.WebApp.Data.Entities.ChatEntities
{
    public class FileDescription : Auditable
    {
        public Guid UserId { get; set; }
        public string ContentType { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public byte[] Data { get; set; } = null!;
    }
}
