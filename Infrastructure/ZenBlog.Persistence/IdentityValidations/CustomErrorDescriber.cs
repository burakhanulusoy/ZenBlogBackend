using Microsoft.AspNetCore.Identity;

namespace ZenBlog.Persistence.IdentityValidations
{
    public class CustomErrorDescriber : IdentityErrorDescriber
    {
        // --- ÞÝFRE HATALARI (PASSWORD ERRORS) ---

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError { Code = "PasswordRequiresDigit", Description = "Þifreniz en az bir rakam ('0'-'9') içermelidir." };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError { Code = "PasswordRequiresLower", Description = "Þifreniz en az bir küçük harf ('a'-'z') içermelidir." };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError { Code = "PasswordRequiresUpper", Description = "Þifreniz en az bir büyük harf ('A'-'Z') içermelidir." };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError { Code = "PasswordRequiresNonAlphanumeric", Description = "Þifreniz en az bir özel karakter (örn. @, #, !, *, vb.) içermelidir." };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError { Code = "PasswordTooShort", Description = $"Þifreniz en az {length} karakter uzunluðunda olmalýdýr." };
        }
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = "PasswordRequiresUniqueChars",
                Description = $"Þifreniz en az {uniqueChars} farklý (benzersiz) karakter içermelidir."
            };
        }


        //public override IdentityError PasswordMismatch()
        //{
        //    return new IdentityError { Code = "PasswordMismatch", Description = "Girdiðiniz þifreler birbiriyle uyuþmuyor." };
        //}

        // --- KULLANICI HATALARI (USER ERRORS) ---

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError { Code = "DuplicateUserName", Description = $"'{userName}' kullanýcý adý zaten alýnmýþ. Lütfen farklý bir kullanýcý adý deneyin." };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError { Code = "DuplicateEmail", Description = $"'{email}' e-posta adresi zaten kullanýmda." };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError { Code = "InvalidUserName", Description = $"'{userName}' geçersiz bir kullanýcý adý. Kullanýcý adlarý sadece harf ve rakam içerebilir." };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError { Code = "InvalidEmail", Description = $"'{email}' geçersiz bir e-posta adresi formatý." };
        }

        // --- ROL HATALARI (ROLE ERRORS) ---

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError { Code = "DuplicateRoleName", Description = $"'{role}' adýnda bir rol zaten mevcut." };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError { Code = "InvalidRoleName", Description = $"'{role}' geçersiz bir rol adý." };
        }
    }
}
