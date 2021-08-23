using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Exceptions
{
    /// <summary>
    /// Custom Exception for Transaction Batch Failure from Cosmos
    ///https://github.com/MicrosoftDocs/azure-docs/issues/19782
    ///https://stackoverflow.com/questions/43371936/how-to-log-exception-data-key-values-with-serilog-2
    /// </summary>
    [Serializable]
    public class BlendNetCosmosTransactionalBatchException:Exception
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public BlendNetCosmosTransactionalBatchException()
        {
        }

        public BlendNetCosmosTransactionalBatchException(string message)
            : base(message)
        {
        }

        public BlendNetCosmosTransactionalBatchException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Appends the additional information to Data collection, in case someone need to access programatically
        /// </summary>
        /// <param name="message"></param>
        /// <param name="batchResponseStatusCode"></param>
        /// <param name="batchResponseErrorMessage"></param>
        /// <param name="operationStatus"></param>
        public BlendNetCosmosTransactionalBatchException(   string message,
                                                            HttpStatusCode? batchResponseStatusCode,
                                                            string batchResponseErrorMessage,
                                                            List<string> operationStatus)
            : base(GetAdditionalInformation(message,batchResponseStatusCode,batchResponseErrorMessage,operationStatus))
        {

            if (batchResponseStatusCode.HasValue)
            {
                Data.Add("BatchResponseStatusCode", batchResponseStatusCode.Value);
            }

            if (!string.IsNullOrEmpty(batchResponseErrorMessage))
            {
                Data.Add("BatchResponseErrorMessage", batchResponseErrorMessage);
            }

            if (operationStatus != null && operationStatus.Count > 0)
            {
                Data.Add("OperationStatus", operationStatus);
            }
        }

        /// <summary>
        /// Appends the Additional Informatio to Message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="batchResponseStatusCode"></param>
        /// <param name="batchResponseErrorMessage"></param>
        /// <param name="operationStatus"></param>
        /// <returns></returns>
        private static string GetAdditionalInformation(string message,
                                                HttpStatusCode? batchResponseStatusCode,
                                                string batchResponseErrorMessage,
                                                List<string> operationStatus)
        {
            StringBuilder additionalInfo = new StringBuilder();

            if (batchResponseStatusCode.HasValue)
            {
                additionalInfo.Append($" BatchResponseStatusCode : {batchResponseStatusCode.Value}");
            }

            if (!string.IsNullOrEmpty(batchResponseErrorMessage))
            {
                additionalInfo.Append($" BatchResponseErrorMessage : {batchResponseErrorMessage}");
            }

            if (operationStatus != null && operationStatus.Count > 0)
            {
                additionalInfo.Append($" OperationStatus : {string.Join(Environment.NewLine, operationStatus)}");
            }

            message = $"{message} Additional Info : {additionalInfo.ToString()}";

            return message;
        }
    }
}
