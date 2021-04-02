using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.Common
{
    /// <summary>
    ///  https://github.com/icsharpcode/SharpZipLib/wiki/GZip-and-Tar-Samples#createFull
    /// </summary>
    public class TarGenerator
    {
        public void TarCreateFromFileStream(string tarFilePath, string tarSourceDirectory)
        {
            // Create an output stream. Does not have to be disk, could be MemoryStream etc.

            using (Stream outStream = File.Create(tarFilePath))
            {
                using (TarOutputStream tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8))
                {
                    CreateTarManuallyFromFile(tarOutputStream, tarSourceDirectory);

                    // Closing the archive also closes the underlying stream.
                    // If you don't want this (e.g. writing to memorystream), set tarOutputStream.IsStreamOwner = false
                    tarOutputStream.Close();
                }
            }
        }

        private void CreateTarManuallyFromFile(TarOutputStream tarOutputStream, string sourceDirectory)
        {
            // Optionally, write an entry for the directory itself.
            TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory);

            tarEntry.Name = "";

            tarOutputStream.PutNextEntry(tarEntry);

            // Write each file to the tar.
            string[] filenames = Directory.GetFiles(sourceDirectory);

            foreach (string filename in filenames)
            {
                using (Stream inputStream = File.OpenRead(filename))
                {
                    string onlyFileName = Path.GetFileName(filename);

                    string tarName;

                    tarName = onlyFileName;

                    long fileSize = inputStream.Length;

                    // Create a tar entry named as appropriate. You can set the name to anything,
                    // but avoid names starting with drive or UNC.
                    TarEntry entry = TarEntry.CreateTarEntry(tarName);

                    // Must set size, otherwise TarOutputStream will fail when output exceeds.
                    entry.Size = fileSize;

                    // Add the entry to the tar stream, before writing the data.
                    tarOutputStream.PutNextEntry(entry);

                    // this is copied from TarArchive.WriteEntryCore
                    byte[] localBuffer = new byte[32 * 1024];
                    while (true)
                    {
                        int numRead = inputStream.Read(localBuffer, 0, localBuffer.Length);
                        if (numRead <= 0)
                            break;

                        tarOutputStream.Write(localBuffer, 0, numRead);
                    }
                }
                tarOutputStream.CloseEntry();
            }

            // Recurse. Delete this if unwanted.
            string[] directories = Directory.GetDirectories(sourceDirectory);
            foreach (string directory in directories)
                CreateTarManuallyFromFile(tarOutputStream, directory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobContainerClient"></param>
        /// <param name="tarFilePath"></param>
        /// <param name="tarFileName"></param>
        /// <param name="tarSourceDirectory"></param>
        /// <returns></returns>
        public async Task<long> TarCreateFromBlobStream(BlobContainerClient blobContainerClient,
                                        string tarFilePath,
                                        string tarFileName,
                                        string tarSourceDirectory)
        {
            // Create an output stream. Does not have to be disk, could be MemoryStream etc.
            BlockBlobClient blockBlobClient = blobContainerClient.GetBlockBlobClient(tarFilePath);

            BlockBlobOpenWriteOptions options = new BlockBlobOpenWriteOptions();

            using (Stream outStream = await blockBlobClient.OpenWriteAsync(true, options))
            {
                using (TarOutputStream tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8))
                {
                    await CreateTarManuallyFromBlob(blobContainerClient, tarOutputStream, tarFileName, tarSourceDirectory);

                    // Closing the archive also closes the underlying stream.
                    // If you don't want this (e.g. writing to memorystream), set tarOutputStream.IsStreamOwner = false
                    tarOutputStream.Close();

                    return tarOutputStream.Position;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobContainerClient"></param>
        /// <param name="tarOutputStream"></param>
        /// <param name="tarfileName"></param>
        /// <param name="sourceDirectoryPath"></param>
        /// <returns></returns>
        private async Task CreateTarManuallyFromBlob(   BlobContainerClient blobContainerClient,
                                                        TarOutputStream tarOutputStream, 
                                                        string tarfileName, 
                                                        string sourceDirectoryPath)
        {
            // Write each file to the tar.
            List<BlobItem> blobItems = await ListBlobsForFolder(blobContainerClient, sourceDirectoryPath);

            foreach (BlobItem blobItem in blobItems)
            {
                string filename = blobItem.Name.Split('/').Last();

                TarEntry entry = TarEntry.CreateTarEntry(filename);

                entry.Size = blobItem.Properties.ContentLength.Value;

                tarOutputStream.PutNextEntry(entry);

                BlockBlobClient blockBlobClient = blobContainerClient.GetBlockBlobClient(blobItem.Name);

                await blockBlobClient.DownloadToAsync(tarOutputStream);

                tarOutputStream.CloseEntry();
            }
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
