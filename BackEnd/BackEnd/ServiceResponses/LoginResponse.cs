using BackEnd.Models.FrontEndModels;

namespace BackEnd.Responses
{
    public class LoginResponse
    {
        /// <summary>
        /// O token JWT gerado para autenticação.
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// Os detalhes do utilizador autenticado.
        /// </summary>
        public User user { get; set; }
    }
}

