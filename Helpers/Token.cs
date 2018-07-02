using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FitnessApp.Models;
using FitnessApp.Services.Intefaces;
using Newtonsoft.Json;

namespace FitnessApp.Helpers
{
    public class Tokens
    {
        public string Id {get;set;}
        [JsonProperty("access_token")]
        public string AccessToken {get;set;}
        [JsonProperty("expires_in")]
        public short ExpiresIn {get;set;}
        [JsonProperty("profile_completed")]
        public bool ProfileCompleted {get;set;}
        public static async Task<Tokens> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, 
            JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings, bool profileCompleted)
        {
            var response = new Tokens()
            {
                Id = identity.Claims.Single(c => c.Type == "id").Value,
                AccessToken = await jwtFactory.GenerateEncodedToken(userName, identity),
                ExpiresIn = (short)jwtOptions.ValidFor.TotalSeconds,
                ProfileCompleted = profileCompleted
            };

            return response;
        }
    }
}