using Newtonsoft.Json;

namespace Nop.Plugin.Misc.GittiGidiyor.Domain
{
    public class ProductServiceResponse : BaseResponse
    {
        [JsonProperty("productid")]
        public int ProductId;

        [JsonProperty("result")]
        public string Result;
    }
}
