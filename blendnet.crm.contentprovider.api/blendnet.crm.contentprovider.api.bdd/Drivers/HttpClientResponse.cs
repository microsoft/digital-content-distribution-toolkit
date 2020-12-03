using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace blendnet.crm.contentprovider.api.bdd.Drivers
{
    public class HttpClientResponse<T>
    {
        public T Data { get; set; }

        public HttpResponseMessage RawMessage { get; set; }
    }
}
