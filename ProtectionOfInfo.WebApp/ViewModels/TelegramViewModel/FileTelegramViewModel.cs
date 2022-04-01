using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProtectionOfInfo.WebApp.ViewModels.TelegramViewModel
{
    public class FileTelegramViewModel
    {
        public string? Id { get; set; }

        public string? Url { get; set; }

        public bool IsImage { get; set; } = false;

        public string? FileName { get; set; }

        [Display(Name = "Файл")]
        public IFormFile? UploadedFile { get; set; }

        [Required(ErrorMessage = "Описание файла не задано")]
        [Display(Name = "Описание файла")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Значение не задано")]
        [Display(Name = "Для публикации?")]
        public bool IsPublication { get; set; } = false;

        [Required(ErrorMessage = "Название не задано")]
        [Display(Name = "Название")]
        public string? Value { get; set; }
    }
}
