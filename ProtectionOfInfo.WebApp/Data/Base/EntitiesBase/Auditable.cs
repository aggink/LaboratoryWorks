using System;

namespace ProtectionOfInfo.WebApp.Data.Base.EntitiesBase
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Auditable : Identity, IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = null!;
    }
}
