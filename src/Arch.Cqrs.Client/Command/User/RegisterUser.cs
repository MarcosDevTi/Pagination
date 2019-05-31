using System.ComponentModel.DataAnnotations;

namespace Arch.Cqrs.Client.Command.User
{
    public class RegisterUser : Infra.Shared.Cqrs.Command.Command
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 8")]
        public string Password { get; set; }
    }
}
