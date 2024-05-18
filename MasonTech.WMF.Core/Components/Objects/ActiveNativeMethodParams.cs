using System.Text.Json.Serialization;

namespace MasonTech.WMF.Core.Components.Objects
{
    public class ActiveNativeMethodParams
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;
    }
}
