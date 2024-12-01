using System.ComponentModel.DataAnnotations;

namespace BackEnd.ServiceResponses
{
    public class GoogleSignInRequest
    {
        [Required]
        public string IdToken { get; set; }
    }
}
