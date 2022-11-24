// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Storage.Blobs;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IIS.Media.DASH.MPDParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener
{
    /// <summary>
    /// Responsible for Downloading Segments from MPD Url
    /// Todo : check if adding the parallel dowload further decreases the download time
    /// </summary>
    public class SegmentDowloader
    {
        private readonly ILogger _logger;

        private readonly AppSettings _appSettings;

        private HttpClient _httpClient;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="clientFactory"></param>
        public SegmentDowloader(ILogger<SegmentDowloader> logger,
                                IOptionsMonitor<AppSettings> optionsMonitor,
                                IHttpClientFactory clientFactory)
        {
            _logger = logger;

            _appSettings = optionsMonitor.CurrentValue;

            _httpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientNames.AMS);
        }


        /// <summary>
        /// Responsible for downloading segments
        /// </summary>
        /// <param name="dashUrl"></param>
        /// <param name="downloadDirectory"></param>
        /// <param name="uniqueId"></param>
        /// <param name="blobContainerClient"></param>
        /// <returns></returns>
        public async Task<MpdInfo> DownloadSegments(string dashUrl,
                                                    string downloadDirectory, 
                                                    string uniqueId,
                                                    BlobContainerClient blobContainerClient)
        {
            MpdInfo mpdInfo = new MpdInfo() { AdaptiveSets = new List<AdaptiveSetInfo>() };

            MPDParser parser = new MPDParser();

            MPD manifest = parser.parse(dashUrl);

            Uri manifestUrl = new Uri(dashUrl);

            AdaptiveSetInfo adaptiveSetInfo;

            //Download audio and video segments
            foreach (var adaptationSet in manifest.Periods[0].AdaptationSets)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

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

                    await DownloadToFile(initSegmentUrl, pathToDownload);
                }
                else
                {
                    pathToDownload = $"{downloadDirectory}/{directoryName}/{filename}";

                    await DownloadToBlob(blobContainerClient, initSegmentUrl, pathToDownload);
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

                            await DownloadToFile(currentUri, pathToDownload);
                        }else
                        {
                            pathToDownload = ($"{downloadDirectory}/{directoryName}/{filename}");

                            await DownloadToBlob(blobContainerClient, currentUri, pathToDownload);
                        }

                        currentTimeStamp += timelineEntry.D;

                        downloadCount++;

                    } while (downloadCount <= timelineEntry.R);
                }

                stopwatch.Stop();

                _logger.LogInformation($"Dowloaded Segments for {adaptiveSetInfo.DirectoryName} Download Directory {downloadDirectory}. Duration Milisecond : {stopwatch.ElapsedMilliseconds}");

            }

            mpdInfo.MpdName = $"{uniqueId}.mpd";

            if (blobContainerClient == null)
            {
                await DownloadToFile(manifestUrl, Path.Combine(downloadDirectory, mpdInfo.MpdName));
            }
            else
            {
                await DownloadToBlob(blobContainerClient, manifestUrl, $"{downloadDirectory}/{mpdInfo.MpdName}");
            }

            return mpdInfo;

        }

       
        /// <summary>
        /// Download the segments to the given directory
        /// </summary>
        /// <param name="dashUrl"></param>
        /// <param name="downloadDirectory"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public async Task<MpdInfo> DownloadSegments(string dashUrl, 
                                                    string downloadDirectory, 
                                                    string uniqueId)
        {
            return await DownloadSegments(dashUrl, downloadDirectory, uniqueId,null);
        }


        /// <summary>
        /// Returns only the segment metadata
        /// </summary>
        /// <param name="dashUrl"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public MpdInfo GetSegmentsMetadata(string dashUrl,string uniqueId)
        {
            MpdInfo mpdInfo = new MpdInfo() { AdaptiveSets = new List<AdaptiveSetInfo>() };

            MPDParser parser = new MPDParser();

            MPD manifest = parser.parse(dashUrl);

            Uri manifestUrl = new Uri(dashUrl);

            AdaptiveSetInfo adaptiveSetInfo;

            //Download audio and video segments
            foreach (var adaptationSet in manifest.Periods[0].AdaptationSets)
            {
                uint bandwidth = adaptationSet.Representations[0].Bandwidth;

                string fullName = adaptationSet.SegmentTemplate.InitializationAttribute.Replace(ApplicationConstants.MPDTokens.Bandwidth,
                                                                                                bandwidth.ToString());
                string directoryName = fullName.Split('/')[0];

                adaptiveSetInfo = new AdaptiveSetInfo() { DirectoryName = directoryName, Type = adaptationSet.ContentType };

                mpdInfo.AdaptiveSets.Add(adaptiveSetInfo);
            }

            mpdInfo.MpdName = $"{uniqueId}.mpd";

            return mpdInfo;
        }


        /// <summary>
        /// Returns the Init Url
        /// </summary>
        /// <param name="manifestUrl"></param>
        /// <param name="initUrlSegment"></param>
        /// <param name="bandwidth"></param>
        /// <returns></returns>
        Uri FormatInitUrl(Uri manifestUrl, string initUrlSegment, uint bandwidth)
        {
            string relativeSegment = initUrlSegment.Replace(ApplicationConstants.MPDTokens.Bandwidth, Convert.ToString(bandwidth));

            return new Uri(manifestUrl, relativeSegment);
        }

        /// <summary>
        /// Get the Fragment URL
        /// </summary>
        /// <param name="manifestUrl"></param>
        /// <param name="initUrlSegment"></param>
        /// <param name="bandwidth"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        Uri FormatFragmentUrl(Uri manifestUrl, string initUrlSegment, uint bandwidth, ulong timestamp)
        {
            string relativeSegment = initUrlSegment.Replace(ApplicationConstants.MPDTokens.Bandwidth, Convert.ToString(bandwidth));

            relativeSegment = relativeSegment.Replace(ApplicationConstants.MPDTokens.Timeline, Convert.ToString(timestamp));
            
            return new Uri(manifestUrl, relativeSegment);
        }

        /// <summary>
        /// Download Segment
        /// </summary>
        /// <param name="downloadUri"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        async Task DownloadToFile(Uri downloadUri, string filename)
        {
            using (var fileStream = File.Create(filename))
            {
                using (Stream data = await _httpClient.GetStreamAsync(downloadUri))
                {
                    data.CopyTo(fileStream);

                    fileStream.Flush();
                }
            }
        }


        /// <summary>
        /// Download the segment directory to blob
        /// </summary>
        /// <param name="blobContainerClient"></param>
        /// <param name="downloadUri"></param>
        /// <param name="blobUploadPath"></param>
        /// <returns></returns>
        async Task DownloadToBlob(BlobContainerClient blobContainerClient,Uri downloadUri, string blobUploadPath)
        {
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobUploadPath);
           
            using (Stream data = await _httpClient.GetStreamAsync(downloadUri))
            {
                await blobClient.UploadAsync(data, true);

                data.Close();
            }
        }
    }



    /// <summary>
    /// Adaptive Set Info
    /// </summary>
    public class AdaptiveSetInfo
    {
        public string Type { get; set; }

        public string DirectoryName { get; set; }

        public string FinalPath { get; set; }

        public long Length { get; set; }

        public string Checksum { get; set; }

    }

    /// <summary>
    /// MpdInfo
    /// </summary>
    public class MpdInfo
    {
        public string FinalMpdPath { get; set; }

        public string MpdName { get; set; }

        public long Length { get; set; }

        public string Checksum { get; set; }

        public List<AdaptiveSetInfo> AdaptiveSets { get; set; }
    }
}
