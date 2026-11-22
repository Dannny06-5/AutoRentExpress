
using Microsoft.AspNetCore.Identity;

namespace AutoRentExpress.Security
{
    public class BCryptPasswordHasher : IPasswordHasher<IdentityUser>
    {
        public string HashPassword(
            IdentityUser user,
            string password)
        {
            ArgumentNullException.ThrowIfNull(password);

            return BCrypt.Net.BCrypt.HashPassword(
                password,
                workFactor: 12);
        }

        public PasswordVerificationResult VerifyHashedPassword(
            IdentityUser user,
            string hashedPassword,
            string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword) ||
                providedPassword == null)
            {
                return PasswordVerificationResult.Failed;
            }

            try
            {
                var valido = BCrypt.Net.BCrypt.Verify(
                    providedPassword,
                    hashedPassword);

                return valido
                    ? PasswordVerificationResult.Success
                    : PasswordVerificationResult.Failed;
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }
}
