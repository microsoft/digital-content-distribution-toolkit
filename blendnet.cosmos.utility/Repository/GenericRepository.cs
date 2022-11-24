// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.infrastructure.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cosmos.utility.Repository
{
    public class GenericRepository
    {
        private CosmosClient _dbClient;

        public GenericRepository(CosmosClient dbClient)
        {
            _dbClient = dbClient;
        }

        /// <summary>
        /// Returns the list
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<O>> GetList<O>( string databaseName, 
                                string collectionName, 
                                string query)
        {
            Container container = _dbClient.GetContainer(databaseName, collectionName);

            var queryDef = new QueryDefinition(query);

            return await container.ExtractDataFromQueryIterator<O>(queryDef);
        }

        /// <summary>
        /// Update a record. Throws exception in case no record is found
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <param name="partionKey"></param>
        /// <param name="updatedObject"></param>
        /// <returns></returns>
        public async Task<int> Update<I>(   string databaseName,
                                            string collectionName, 
                                            string id,
                                            string partionKey,
                                            I updatedObject)
        {
            Container container = _dbClient.GetContainer(databaseName, collectionName);

            var response = await container.ReplaceItemAsync<I>(updatedObject,
                                                                id,
                                                                new PartitionKey(partionKey));

            return (int)response.StatusCode;

        }
    }
}
