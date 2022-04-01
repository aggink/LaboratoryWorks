using ProtectionOfInfo.WebApp.Data.Entities.ChatEntities;
using System;

namespace ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities
{
    public class FileTelegram : File
    {
        public Guid? MessageId { get; set; }
        public string? Description { get; set; }
        public bool IsImage { get; set; } = false;
        public bool IsPublication { get; set; } = false;
        public string? Value { get; set; }
        public MessageTelegram? Message { get; set; }
    }
}