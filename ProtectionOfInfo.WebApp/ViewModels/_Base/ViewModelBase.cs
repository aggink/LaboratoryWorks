using ProtectionOfInfo.WebApp.Data.Base.EntitiesBase;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels.Base
{
    public abstract class ViewModelBase : IViewModel
    {
        [Required(ErrorMessage = "Id сущности не найден.")]
        public Guid? Id { get; set; }
    }
}
