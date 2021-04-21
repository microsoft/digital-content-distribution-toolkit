using blendnet.common.dto.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.repository.CosmosRepository
{
/// <summary>
/// Generic class which represents content result from any query and corresponding continuation token to query further 
/// </summary>
    public class ContentApiResult<T>
    {
        public List<T> _data { get; set; }

        public string _continuationToken { get; set; }

        public ContentApiResult(List<T> data, string continuationToken)
        {
            _data = data;

            _continuationToken = continuationToken;
        }
    }
}
