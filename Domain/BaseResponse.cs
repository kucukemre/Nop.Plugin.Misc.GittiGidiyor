using Newtonsoft.Json;

namespace Nop.Plugin.Misc.GittiGidiyor.Domain
{
    public class BaseResponse
    {
        [JsonProperty("ackCode")]
        public string AckCode;

        [JsonProperty("responseTime")]
        public string ResponseTime;

        [JsonProperty("error")]
        public ErrorType Error;

        [JsonProperty("timeElapsed")]
        public string TimeElapsed;
    }

    public partial class ErrorType
    {
        [JsonProperty("errorId")]
        public string ErrorId;

        [JsonProperty("errorCode")]
        public string ErrorCode;

        [JsonProperty("message")]
        public string Message;

        [JsonProperty("viewMessage")]
        public string ViewMessage;
    }
}
