using Newtonsoft.Json;

namespace WebUI.Models
{
    public class MenuModel
    {
        [JsonProperty("Action")]
        public string? Action { get; set; }

        [JsonProperty("Name")]
        public string? Name { get; set; }
        [JsonProperty("Icon")]
        public string? Icon { get; set; }

        [JsonProperty("Children")]
        public List<MenuModel>? Children { get; set; }

        [JsonProperty("Order")]
        public int? Order { get; set; }
        [JsonProperty("IsHiddenMenu")]
        public bool? IsHiddenMenu { get; set; }

    }
}
