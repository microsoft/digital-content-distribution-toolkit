using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blendnet.common.dto.Common
{
    /// <summary>
    /// Generic class which represents content result from any query and corresponding continuation token to query further 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultData<T>
    {
        /// <summary>
        /// Data Count Property
        /// </summary>
        public int Count 
        {
            get
            {
                if (Data != null)
                {
                    return Data.Count;

                } else
                {
                    return 0;
                }
            }
        }
            
        public List<T> Data { get; set; }

        public string ContinuationToken { get; set; }

        public ResultData(List<T> data, string continuationToken)
        {
            Data = data;

            ContinuationToken = continuationToken;
        }
    }
}
