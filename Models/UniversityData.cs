using System.Text.Json.Serialization;

namespace ComponentTest.Models
{
    public class UniversityData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("alpha_two_code")]
        public string CountryCode { get; set; }
    }
}
