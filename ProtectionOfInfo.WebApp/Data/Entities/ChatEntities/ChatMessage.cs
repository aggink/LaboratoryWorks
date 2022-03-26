using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System;

namespace ProtectionOfInfo.WebApp.Data.Entities.ChatEntities
{
    public class ChatMessage : Auditable
    {
        public Guid UserId { get; set; }
        public bool IsImage { get; set; } 
        public string? Message { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; } 
        public string? Extension { get; set; } 
        public byte[]? Data { get; set; } 
    }
}
