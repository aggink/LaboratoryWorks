using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities
{
    public class MessageTelegram : Auditable
    {
        public Guid UserId { get; set; }
        public string? MessageFromUser { get; set; }
        public string? MessageFromBot { get; set; }
        public UserTelegram? User {get; set;}
        public ICollection<FileTelegram>? Files { get; set; }
    }
}
