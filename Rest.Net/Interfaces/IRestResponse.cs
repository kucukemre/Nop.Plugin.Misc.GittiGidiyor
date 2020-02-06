using System;
using System.Net;
using System.Net.Http;

namespace Nop.Plugin.Misc.GittiGidiyor.Rest.Net.Interfaces
{
    public interface IRestResponse<T>
    {
        int Code { get; }
        HttpStatusCode StatusCode { get; }
        T Data { get; }
        Exception Exception { get; }
        HttpResponseMessage OriginalHttpResponseMessage { get; }
        object RawData { get; }
        bool IsError { get; }
        TimeSpan RequestTime { get; }
        long RequestTimeMs { get; }
    }
}
