using System;

namespace ProtectionOfInfo.WebApp.Data.Base.EntitiesBase
{
    public abstract class Identity : IGuidId
    {
        public Guid Id { get; set; }
    }
}
