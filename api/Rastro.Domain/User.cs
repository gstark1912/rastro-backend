using Rastro.Domain.Infra;

namespace Rastro.Domain
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
