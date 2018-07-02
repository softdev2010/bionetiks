using System;
using Newtonsoft.Json;

namespace FitnessApp.Models {
    public class NationalityModel {
        public string Nationality {get;set;}
        [JsonProperty("en_short_name")]
        public string Country {get;set;}
        [JsonProperty("num_code")]
        public string NumCode {get;set;}
        [JsonProperty("alpha_2_code")]
        public string Alpha2Code {get;set;}
        [JsonProperty("alpha_3_code")]
        public string Alpha3Code {get;set;}
    }
}