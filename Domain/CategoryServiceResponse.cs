using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.GittiGidiyor.Domain
{
    public class CategoryServiceResponse : BaseResponse
    {
        [JsonProperty("categoryCount")]
        public string CategoryCount { get; set; }
        [JsonProperty("categories")]
        public Categories Categories { get; set; }
    }

    public class Categories
    {
        [JsonProperty("category")]
        public List<Category> Category { get; set; }
    }

    public class Category
    {
        [JsonProperty("@hasCatalog")]
        public string HasCatalog { get; set; }
        [JsonProperty("@deepest")]
        public string Deepest { get; set; }
        [JsonProperty("categoryCode")]
        public string CategoryCode { get; set; }
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }
        [JsonProperty("specs")]
        public Specs Specs { get; set; }
    }

    public class Specs
    {
        [JsonProperty("spec")]
        public List<Spec> Spec { get; set; }
    }

    public class Spec
    {
        [JsonProperty("@type")]
        public string Type { get; set; }
        [JsonProperty("@required")]
        public string Required { get; set; }
        [JsonProperty("@name")]
        public string Name { get; set; }
        [JsonProperty("values")]
        public Values Values { get; set; }
    }

    public class Values
    {
        [JsonProperty("value")]
        public List<string> Value { get; set; }
    }
}
