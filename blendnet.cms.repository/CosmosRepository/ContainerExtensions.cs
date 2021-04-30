using blendnet.cms.repository.CosmosRepository;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.repository
{
    static class ContainerExtensions
    {
        /// <summary>
        /// Helper method to run a SELECT query and return all results as a list
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="queryDef">the SELECT query</param>
        /// <returns>List of items that match the query</returns>
        public static async Task<List<T>> ExtractDataFromQueryIterator<T>(this Container container, QueryDefinition queryDef)
        {
            var returnList = new List<T>();
            var query = container.GetItemQueryIterator<T>(queryDef);

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                returnList.AddRange(response.ToList());
            }

            return returnList;
        }

        /// <summary>
        /// Helper method to run a SELECT query and return results as a list with a continuation token
        /// </summary>
        /// <typeparam name="T">Result Type</typeparam>
        /// <param name="queryDef">the SELECT query</param>
        /// <param name="continuationToken">continuation token</param>
        /// <returns></returns>
        public static async Task<ContentApiResult<T>> ExtractDataFromQueryIteratorWithToken<T>(this Container container, QueryDefinition queryDef, string continuationToken)
        {
            ContentApiResult<T> contentResult = null;
            List<T> returnList = new List<T>();

            QueryRequestOptions options = new QueryRequestOptions { MaxItemCount = 1000 }; // should make it configurable in future

            var query = container.GetItemQueryIterator<T>(queryDef, continuationToken: continuationToken, requestOptions: options);

            if (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                returnList.AddRange(response.ToList());

                contentResult = new ContentApiResult<T>(returnList, response.ContinuationToken);
            }
            else
            {
                contentResult = new ContentApiResult<T>(returnList, null);
            }

            return contentResult;
        }
    }
}
