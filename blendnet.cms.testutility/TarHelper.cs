// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.testutility
{
    /// <summary>
    /// https://github.com/icsharpcode/SharpZipLib/wiki/GZip-and-Tar-Samples#createFull
    /// </summary>
    public static class TarHelper
    {
        public static void TarCreateFromStream(string tarFilePath, string tarSourceDirectory)
        {
            // Create an output stream. Does not have to be disk, could be MemoryStream etc.
            string tarOutFn = tarFilePath;

            Stream outStream = File.Create(tarOutFn);

            //TarOutputStream tarOutputStream = new TarOutputStream(outStream);

            using (TarOutputStream tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8))
            {
                CreateTarManually(tarOutputStream, tarSourceDirectory);

                // Closing the archive also closes the underlying stream.
                // If you don't want this (e.g. writing to memorystream), set tarOutputStream.IsStreamOwner = false
                tarOutputStream.Close();

            }
        }

        private static void CreateTarManually(TarOutputStream tarOutputStream, string sourceDirectory)
        {
            // Optionally, write an entry for the directory itself.
            TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory);

            tarEntry.Name = "";

            //if (string.IsNullOrEmpty(parentTarEntryName))
            //{
            //    tarEntry.Name = Path.GetFileName(sourceDirectory);
            //}else
            //{
            //    tarEntry.Name = "";
            //}

            tarOutputStream.PutNextEntry(tarEntry);

            // Write each file to the tar.
            string[] filenames = Directory.GetFiles(sourceDirectory);

            foreach (string filename in filenames)
            {
                // You might replace these 3 lines with your own stream code

                using (Stream inputStream = File.OpenRead(filename))
                {
                    string onlyFileName = Path.GetFileName(filename);

                    string tarName;

                    tarName = onlyFileName;

                    //if (useOnlyFileNameForChildTar)
                    //{
                    //    tarName = onlyFileName;
                    //}
                    //else
                    //{
                    //    tarName = Path.Combine(tarEntry.Name, onlyFileName);
                    //}

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
                CreateTarManually(tarOutputStream, directory);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobContainerClient"></param>
        /// <param name="tarFilePath"></param>
        /// <param name="tarFileName"></param>
        /// <param name="tarSourceDirectory"></param>
        /// <returns></returns>
        //public async static Task<long> TarCreateFromBlobStream(BlobContainerClient blobContainerClient,
        //                                string tarFilePath,
        //                                string tarFileName,
        //                                string tarSourceDirectory)
        //{
        //    // Create an output stream. Does not have to be disk, could be MemoryStream etc.
        //    BlockBlobClient blockBlobClient = blobContainerClient.GetBlockBlobClient(tarFilePath);

        //    BlockBlobOpenWriteOptions options = new BlockBlobOpenWriteOptions();

        //    options.BufferSize = 4194304000;

        //    Console.WriteLine($"{tarFileName} - Start - {DateTime.Now.ToString()}");

        //    using (Stream outStream = await blockBlobClient.OpenWriteAsync(true, options))
        //    {
        //        using (TarOutputStream tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8))
        //        {
        //            await CreateTarManuallyFromBlob(blobContainerClient, tarOutputStream, tarFileName, tarSourceDirectory);

        //            // Closing the archive also closes the underlying stream.
        //            // If you don't want this (e.g. writing to memorystream), set tarOutputStream.IsStreamOwner = false
        //            tarOutputStream.Close();

        //            Console.WriteLine($"{tarFileName} - End - {DateTime.Now.ToString()}");

        //            return tarOutputStream.Position;
        //        }
        //    }


        //}

        public async static Task<long> TarCreateFromMemoryStream(BlobContainerClient blobContainerClient,
                                      string tarFilePath,
                                      string tarSourceDirectory)
        {
            long contentLength;

            Console.WriteLine($"Generating TAR for {tarFilePath} at {DateTime.Now.ToString()}");

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

                string checksum = GetChecksum(tarFilePath, ms);
                
                Console.WriteLine($"Generated Checksum value {checksum} at {DateTime.Now.ToString()}");

                ms.Position = 0;
                
                await EventHandlingUtilities.UploadBlob(blobContainerClient, tarFilePath, ms);

            }

            Console.WriteLine($"Generated TAR for {tarFilePath} at {DateTime.Now.ToString()}");

            return contentLength;

        }

        private static string GetChecksum(string fileName , MemoryStream stream)
        {
            Console.WriteLine($"Generating checksum  for {fileName} at {DateTime.Now.ToString()}");

            string checksum = string.Empty;

            using (var sha = SHA256.Create())
            {
                byte[] checksumBytes = sha.ComputeHash(stream);

                checksum = BitConverter.ToString(checksumBytes).Replace("-", String.Empty).ToLowerInvariant();
            }

            Console.WriteLine($"Generated checksum for {fileName} at {DateTime.Now.ToString()}");

            return checksum;
        }


        /// <summary>
        /// Get the List of Blolbs
        /// </summary>
        /// <param name="container"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private async static Task<List<BlobItem>> ListBlobsForFolder(BlobContainerClient container, string prefix)
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
