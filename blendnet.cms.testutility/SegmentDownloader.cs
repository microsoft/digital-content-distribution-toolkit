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
        public static SegmentInfo DownloadSegments(string manifestPath, string downloadDirectory, string uniqueName)
        {
            SegmentInfo segmentInfo = new SegmentInfo() {AdaptiveSets = new List<AdaptiveSetInfo>() };

            HttpClient client = new HttpClient();

            MPDParser parser = new MPDParser();

            MPD manifest = parser.parse(manifestPath);

            string bandwidthToken = "$Bandwidth$";

            string timelineToken = "$Time$";

            Uri manifestUrl = new Uri(manifestPath);

            List<string> directoryNames = new List<string>();

            AdaptiveSetInfo adaptiveSetInfo;

            foreach (var adaptationSet in manifest.Periods[0].AdaptationSets)
            {
                uint bandwidth = adaptationSet.Representations[0].Bandwidth;

                //string filename = Path.Combine(downloadDirectory, bandwidth.ToString() + "_init");

                string fullName = adaptationSet.SegmentTemplate.InitializationAttribute.Replace(bandwidthToken, bandwidth.ToString());

                string directoryName = fullName.Split('/')[0];

                adaptiveSetInfo = new AdaptiveSetInfo() { DirectoryName = directoryName, Type = adaptationSet.ContentType };

                segmentInfo.AdaptiveSets.Add(adaptiveSetInfo);
                
                directoryNames.Add(directoryName);

                string filename = fullName.Split('/')[1];

                string pathToDownload;

                if (!System.IO.Directory.Exists(Path.Combine(downloadDirectory, directoryName)))
                {
                    System.IO.Directory.CreateDirectory(Path.Combine(downloadDirectory, directoryName));
                }

                pathToDownload = Path.Combine(downloadDirectory, directoryName, filename);

                Uri initSegmentUrl = FormatInitUrl(manifestUrl, adaptationSet.SegmentTemplate.InitializationAttribute, bandwidth);

                DownloadToFile(client, initSegmentUrl, pathToDownload);

                uint timescale = adaptationSet.SegmentTemplate.Timescale;

                ulong currentTimeStamp = adaptationSet.SegmentTemplate.StartNumber;

                foreach (var timelineEntry in adaptationSet.SegmentTemplate.SegmentTimeline.Ss)
                {
                    ulong downloadCount = 0;
                    do
                    {
                        //filename = Path.Combine(downloadDirectory, bandwidth.ToString()+ "_" + currentTimeStamp.ToString());

                        fullName = adaptationSet.SegmentTemplate.Media.Replace(bandwidthToken, bandwidth.ToString());

                        fullName = fullName.Replace(timelineToken, currentTimeStamp.ToString());

                        directoryName = fullName.Split('/')[0];

                        filename = fullName.Split('/')[1];

                        if (!System.IO.Directory.Exists(Path.Combine(downloadDirectory, directoryName)))
                        {
                            System.IO.Directory.CreateDirectory(Path.Combine(downloadDirectory, directoryName));
                        }

                        pathToDownload = Path.Combine(downloadDirectory, directoryName, filename);

                        Uri currentUri = FormatFragmentUrl(manifestUrl, adaptationSet.SegmentTemplate.Media, bandwidth, currentTimeStamp);

                        DownloadToFile(client, currentUri, pathToDownload);

                        currentTimeStamp += timelineEntry.D;

                        downloadCount++;

                    } while (downloadCount <= timelineEntry.R);
                }
            }

            segmentInfo.MpdName = $"{uniqueName}.mpd";
            //download maifest

            DownloadToFile(client, manifestUrl, Path.Combine(downloadDirectory, segmentInfo.MpdName));

            return segmentInfo;
        }



        static Uri FormatInitUrl(Uri manifestUrl, string initUrlSegment, uint bandwidth)
        {
            string relativeSegment = initUrlSegment.Replace("$Bandwidth$", Convert.ToString(bandwidth));
            return new Uri(manifestUrl, relativeSegment);
        }

        static Uri FormatFragmentUrl(Uri manifestUrl, string initUrlSegment, uint bandwidth, ulong timestamp)
        {
            string relativeSegment = initUrlSegment.Replace("$Bandwidth$", Convert.ToString(bandwidth));
            relativeSegment = relativeSegment.Replace("$Time$", Convert.ToString(timestamp));
            return new Uri(manifestUrl, relativeSegment);
        }

        static void DownloadToFile(HttpClient client, Uri downloadUri, string filename)
        {
            using (var fileStream = File.Create(filename))
            using (Stream data = client.GetStreamAsync(downloadUri).Result)
            {
                data.CopyTo(fileStream);
                fileStream.Flush();
            }
        }
    }


    public class AdaptiveSetInfo
    {
        public string Type { get; set; }

        public string DirectoryName { get; set; }

        public string FinalPath { get; set; }
       
    }

    public class SegmentInfo
    {
        public string FinalMpdPath { get; set; }

        public string MpdName { get; set; }
        
        public List<AdaptiveSetInfo> AdaptiveSets { get; set; }
    }

}
