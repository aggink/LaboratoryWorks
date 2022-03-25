using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System;

namespace ProtectionOfInfo.WebApp.Data.Entities.ChatEntities
{
    public class ChatMessage : Auditable
    {
        public Guid UserId { get; set; }
        public Guid? FileId { get; set; }
        public string? Message { get; set; }
        public FileDescription? File { get; set; }
    }
}
