using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace blendnet.cms.listener.Common
{
    /// <summary>
    ///  https://github.com/icsharpcode/SharpZipLib/wiki/GZip-and-Tar-Samples#createFull
    /// </summary>
    public class TarGenerator
    {
        public void TarCreateFromStream(string tarFilePath, string tarSourceDirectory)
        {
            // Create an output stream. Does not have to be disk, could be MemoryStream etc.

            using (Stream outStream = File.Create(tarFilePath))
            {
                using (TarOutputStream tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8))
                {
                    CreateTarManually(tarOutputStream, tarSourceDirectory);

                    // Closing the archive also closes the underlying stream.
                    // If you don't want this (e.g. writing to memorystream), set tarOutputStream.IsStreamOwner = false
                    tarOutputStream.Close();
                }
            }
        }


        private void CreateTarManually(TarOutputStream tarOutputStream, string sourceDirectory)
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
                CreateTarManually(tarOutputStream, directory);
        }
    }
}
