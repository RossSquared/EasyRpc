﻿using Newtonsoft.Json;

namespace EasyRpc.DynamicClient.Messages
{
    public class ErrorClass
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
    
    public class RpcResponseMessage<T>
    {
        [JsonProperty("jsonrpc")]
        public string Version { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("error")]
        public ErrorClass Error { get; set; }
    }
}
