using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.PasswordValidatorsService
{
    public class PasswordValidatorService : IMyPasswordValidatorService
    {
        public PasswordValidatorService() { }

        // Чередование цифр, букв и снова цифр.
        public IdentityResult Validate(string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            var charArray = password.ToCharArray();
            if (!char.IsDigit(charArray[0]))
            {
                errors.Add(new IdentityError
                {
                    Description = "Пароль не соответствует требованиям: чередование цифр, букв и снова цифр!"
                });
            }

            if (!char.IsDigit(charArray[charArray.Length - 1]))
            {
                errors.Add(new IdentityError
                {
                    Description = "Пароль не соответствует требованиям: чередование цифр, букв и снова цифр!"
                });
            }

            var pattern = "^(?!.*(?:[A-Za-z]{2,}|[0-9]{2,}))[A-Za-z0-9]+$";
            if (!Regex.IsMatch(password, pattern))
            {
                errors.Add(new IdentityError
                {
                    Description = "Пароль не соответствует требованиям: чередование цифр, букв и снова цифр!"
                });
            }

            return errors.Count == 0 ?
                    IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
