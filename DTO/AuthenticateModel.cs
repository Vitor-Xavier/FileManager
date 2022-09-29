using System.ComponentModel.DataAnnotations;

namespace FileManager.DTO
{
    public class AuthenticateModel
    {
        /// <summary>
        /// Nome de usuário
        /// </summary>
        /// <example>vitorxs</example>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Senha
        /// </summary>
        /// <example>1234</example>
        [Required]
        public string Password { get; set; }
    }
}
