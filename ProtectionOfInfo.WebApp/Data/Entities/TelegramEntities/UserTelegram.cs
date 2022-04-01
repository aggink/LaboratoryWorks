using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities
{
    public class UserTelegram : Auditable
    {
        public long ChatId { get; set; }
        public string NickName { get; set; } = null!;
        public ICollection<MessageTelegram>? Messages { get; set; }
    }
}
