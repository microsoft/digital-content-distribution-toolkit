// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Exceptions;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;

namespace blendnet.common.infrastructure.Extensions
{
    public static class TransactionalBatchResponseExtensions
    {
        /// <summary>
        /// Get Exception from Cosmos Transactional Batch Response
        /// </summary>
        /// <param name="batchResponse">response</param>
        /// <param name="errorMessage">error message to use in case of errors</param>
        /// <returns></returns>
        public static BlendNetCosmosTransactionalBatchException GetTransactionalBatchException(this TransactionalBatchResponse batchResponse, string errorMessage)
        {
            List<string> operationStatus = batchResponse.Select((result, index) => $"Status code for index {index} is {result.StatusCode}").ToList();

            BlendNetCosmosTransactionalBatchException batchException =
                            new BlendNetCosmosTransactionalBatchException(errorMessage,
                                                                          batchResponse.StatusCode,
                                                                          batchResponse.ErrorMessage,
                                                                          operationStatus);
            return batchException;
        }
    }
}
