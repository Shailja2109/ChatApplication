using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ChatApplication2.ParameterModels
{
    public class ExternalAuthDto
    {
        public const string PROVIDER = "google";

        [JsonProperty("idToken")]
        [Required]
        public string IdToken { get; set; }

    }
}
