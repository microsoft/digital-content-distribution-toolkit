// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Storage.Blobs;
using Microsoft.IIS.Media.DASH.MPDParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.testutility
{
    public static class SegmentDownloader
    {
        public static async Task<MpdInfo> DownloadSegments(string dashUrl,
                                                    string downloadDirectory,
                                                    string uniqueId,
                                                    BlobContainerClient blobContainerClient)
        {
            MpdInfo mpdInfo = new MpdInfo() { AdaptiveSets = new List<AdaptiveSetInfo>() };

            MPDParser parser = new MPDParser();

            MPD manifest = parser.parse(dashUrl);

            Uri manifestUrl = new Uri(dashUrl);

            AdaptiveSetInfo adaptiveSetInfo;

            HttpClient httpClient = new HttpClient();


            //Download audio and video segments
            foreach (var adaptationSet in manifest.Periods[0].AdaptationSets)
            {
                uint bandwidth = adaptationSet.Representations[0].Bandwidth;

                string fullName = adaptationSet.SegmentTemplate.InitializationAttribute.Replace(ApplicationConstants.MPDTokens.Bandwidth,
                                                                                                bandwidth.ToString());
                string directoryName = fullName.Split('/')[0];

                string filename = fullName.Split('/')[1];

                adaptiveSetInfo = new AdaptiveSetInfo() { DirectoryName = directoryName, Type = adaptationSet.ContentType };

                mpdInfo.AdaptiveSets.Add(adaptiveSetInfo);

                //download init file
                Uri initSegmentUrl = FormatInitUrl(manifestUrl, adaptationSet.SegmentTemplate.InitializationAttribute, bandwidth);

                string pathToDownload;


                if (blobContainerClient == null)
                {
                    if (!Directory.Exists(Path.Combine(downloadDirectory, directoryName)))
                    {
                        Directory.CreateDirectory(Path.Combine(downloadDirectory, directoryName));
                    }

                    pathToDownload = Path.Combine(downloadDirectory, directoryName, filename);

                    await DownloadToFile(httpClient ,initSegmentUrl, pathToDownload);
                }
                else
                {
                    pathToDownload = $"{downloadDirectory}/{directoryName}/{filename}";

                    await DownloadToBlob(httpClient ,blobContainerClient, initSegmentUrl, pathToDownload);
                }

                uint timescale = adaptationSet.SegmentTemplate.Timescale;

                ulong currentTimeStamp = adaptationSet.SegmentTemplate.StartNumber;

                //dowload Segments
                foreach (var timelineEntry in adaptationSet.SegmentTemplate.SegmentTimeline.Ss)
                {
                    ulong downloadCount = 0;

                    do
                    {
                        fullName = adaptationSet.SegmentTemplate.Media.Replace(ApplicationConstants.MPDTokens.Bandwidth,
                                                                               bandwidth.ToString());

                        fullName = fullName.Replace(ApplicationConstants.MPDTokens.Timeline, currentTimeStamp.ToString());

                        directoryName = fullName.Split('/')[0];

                        filename = fullName.Split('/')[1];

                        Uri currentUri = FormatFragmentUrl(manifestUrl, adaptationSet.SegmentTemplate.Media, bandwidth, currentTimeStamp);

                        if (blobContainerClient == null)
                        {
                            pathToDownload = Path.Combine(downloadDirectory, directoryName, filename);

                            await DownloadToFile(httpClient,currentUri, pathToDownload);
                        }
                        else
                        {
                            pathToDownload = ($"{downloadDirectory}/{directoryName}/{filename}");

                            await DownloadToBlob(httpClient,blobContainerClient, currentUri, pathToDownload);
                        }

                        currentTimeStamp += timelineEntry.D;

                        downloadCount++;

                    } while (downloadCount <= timelineEntry.R);
                }
            }

            mpdInfo.MpdName = $"{uniqueId}.mpd";

            if (blobContainerClient == null)
            {
                await DownloadToFile(httpClient, manifestUrl, Path.Combine(downloadDirectory, mpdInfo.MpdName));
            }
            else
            {
                await DownloadToBlob(httpClient, blobContainerClient, manifestUrl, $"{downloadDirectory}/{mpdInfo.MpdName}");
            }

            return mpdInfo;


        }


        public static async Task<MpdInfo> DownloadSegments(string dashUrl,
                                            string downloadDirectory,
                                            string uniqueId)
        {
            return await DownloadSegments(dashUrl, downloadDirectory, uniqueId, null);
        }

        static Uri FormatInitUrl(Uri manifestUrl, string initUrlSegment, uint bandwidth)
        {
            string relativeSegment = initUrlSegment.Replace(ApplicationConstants.MPDTokens.Bandwidth, Convert.ToString(bandwidth));

            return new Uri(manifestUrl, relativeSegment);
        }

        static Uri FormatFragmentUrl(Uri manifestUrl, string initUrlSegment, uint bandwidth, ulong timestamp)
        {
            string relativeSegment = initUrlSegment.Replace(ApplicationConstants.MPDTokens.Bandwidth, Convert.ToString(bandwidth));

            relativeSegment = relativeSegment.Replace(ApplicationConstants.MPDTokens.Timeline, Convert.ToString(timestamp));

            return new Uri(manifestUrl, relativeSegment);
        }

        static async Task DownloadToFile(HttpClient client, Uri downloadUri, string filename)
        {
            using (var fileStream = File.Create(filename))
            using ( Stream data = await client.GetStreamAsync(downloadUri))
            {
                data.CopyTo(fileStream);
                fileStream.Flush();
            }
        }

        static async Task DownloadToBlob(HttpClient client, BlobContainerClient blobContainerClient, Uri downloadUri, string blobUploadPath)
        {
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobUploadPath);

            using (Stream data = await client.GetStreamAsync(downloadUri))
            {
                await blobClient.UploadAsync(data, true);

                data.Close();
            }
        }
    }


    public class AdaptiveSetInfo
    {
        public string Type { get; set; }

        public string DirectoryName { get; set; }

        public string FinalPath { get; set; }

        public long Length { get; set; }


    }

    public class MpdInfo
    {
        public string FinalMpdPath { get; set; }

        public string MpdName { get; set; }
        
        public List<AdaptiveSetInfo> AdaptiveSets { get; set; }
    }

    public class ApplicationConstants
    {
        public struct AdaptiveSetTypes
        {
            public const string Audio = "audio";

            public const string Video = "video";
        }

        public struct MPDTokens
        {
            public const string Bandwidth = "$Bandwidth$";

            public const string Timeline = "$Time$";
        }

    }

}
