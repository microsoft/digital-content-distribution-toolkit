using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using blendnet.cms.listener.IntegrationEventHandling;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.Common
{
    /// <summary>
    ///  https://github.com/icsharpcode/SharpZipLib/wiki/GZip-and-Tar-Samples#createFull
    /// </summary>
    public class TarGenerator
    {
        private readonly ILogger _logger;

        public TarGenerator(ILogger<TarGenerator> logger)
        {
            _logger = logger;

        }


        /// <summary>
        /// Generates and uploads the TAR to blob
        /// </summary>
        /// <param name="blobContainerClient"></param>
        /// <param name="tarFilePath"></param>
        /// <param name="tarFileName"></param>
        /// <param name="tarSourceDirectory"></param>
        /// <returns></returns>
        public async Task<Tuple<long,string>> CreateTar(BlobContainerClient blobContainerClient,
                                        string tarFilePath,
                                        string tarFileName,
                                        string tarSourceDirectory,
                                        bool generateChecksum = false)
        {

            long contentLength;

            string checksum = string.Empty ;

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Create an output stream. Does not have to be disk, could be MemoryStream etc.
            using (var ms = new MemoryStream())
            {
                using (TarOutputStream tarOutputStream = new TarOutputStream(ms, Encoding.UTF8))
                {
                    tarOutputStream.IsStreamOwner = false;

                    List<BlobItem> blobItems = await ListBlobsForFolder(blobContainerClient, tarSourceDirectory);

                    foreach (BlobItem blobItem in blobItems)
                    {
                        string filename = blobItem.Name.Split('/').Last();

                        TarEntry entry = TarEntry.CreateTarEntry(filename);

                        entry.Size = blobItem.Properties.ContentLength.Value;

                        tarOutputStream.PutNextEntry(entry);

                        BlockBlobClient blockBlobClient = blobContainerClient.GetBlockBlobClient(blobItem.Name);

                        await blockBlobClient.DownloadToAsync(tarOutputStream);

                        await tarOutputStream.FlushAsync();

                        await ms.FlushAsync();

                        tarOutputStream.CloseEntry();

                    }

                    tarOutputStream.Close();
                }

                contentLength = ms.Position;

                ms.Position = 0;

                if (generateChecksum)
                {
                    checksum = EventHandlingUtilities.GetChecksum(_logger,tarFilePath, ms);

                    ms.Position = 0;
                }

                await EventHandlingUtilities.UploadBlob(blobContainerClient, tarFilePath, ms);
            }

            stopwatch.Stop();

            _logger.LogInformation($"Generated TAR File {tarFileName}. Duration Milisecond : {stopwatch.ElapsedMilliseconds}");

            return new Tuple<long, string>(contentLength, checksum);
        }

        /// <summary>
        /// Get the List of Blolbs
        /// </summary>
        /// <param name="container"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private async Task<List<BlobItem>> ListBlobsForFolder(BlobContainerClient container, string prefix)
        {
            List<BlobItem> blobItems = new List<BlobItem>();

            // Call the listing operation and return pages of the specified size.
            var resultSegment = container.GetBlobsAsync(prefix: prefix)
                .AsPages();

            // Enumerate the blobs returned for each page.
            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    blobItems.Add(blobItem);
                }
            }

            return blobItems;
        }

    }
}
