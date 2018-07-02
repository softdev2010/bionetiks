using System;
using Newtonsoft.Json;

namespace FitnessApp.Helpers
{
    public class GoogleUserData
    {
        public string Id { get; set; }
        public string Email { get; set; }
        [JsonProperty("given_name")]
        public string FirstName { get; set; }
        [JsonProperty("family_name")]
        public string LastName { get; set; }
        public string Gender { get; set; }
        [JsonProperty("picture")]
        public string Picture { get; set; }
    }
}
