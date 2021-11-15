using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace blendnet.cms.testutility
{
    class Program
    {
        private const string C_PREFIX = "amstest";

        private static byte[] TokenSigningKey = new byte[40];

        private static string _assestIngestUrl = string.Empty;

        private static readonly string DefaultStreamingEndpointName = "default";

        private static Guid _uniqueness_raw = Guid.NewGuid();

        private static string _uniqueness = _uniqueness_raw.ToString("N");

        private static string _workingDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"{_uniqueness_raw.ToString()}","wrking");

        private static string _finalDirectory = Path.Combine(Directory.GetCurrentDirectory(), _uniqueness_raw.ToString(),"fnl");

        private static string _rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), _uniqueness_raw.ToString());

        private static string _blobrootDirectory = _uniqueness_raw.ToString();

        private static string _blobFinalDirectory = $"{_uniqueness_raw.ToString()}/fnl";

        private static string _blobworkingDirectory = $"{_uniqueness_raw.ToString()}/wrking";

        private static string _outputLogPath = Path.Combine(Directory.GetCurrentDirectory(), _uniqueness_raw.ToString(),$"{_uniqueness_raw.ToString()}.output.txt");

        private static string _xmlTemplateFileName = "sample_ingest_content_rvwd.xml";

        static async Task Main(string[] args)
        {
            AppSettings config = new AppSettings(new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build());

            await PerformChecksumTest();

            return;

            
            if (args == null || args.Length <=0)
            {
                Console.WriteLine("Please provide media item url");

            }else
            {
                Directory.CreateDirectory(_rootDirectory);

                Directory.CreateDirectory(_workingDirectory);

                Directory.CreateDirectory(_finalDirectory);

                _assestIngestUrl = args[0];

                Console.WriteLine($"Starting transcoding process for {_assestIngestUrl}! - {_uniqueness_raw.ToString()}");

                Console.WriteLine($"Dowloading to Blob - {config.DownloadToBlob}");

                #region Direct Test
                if (args.Length > 1)
                {
                    Console.WriteLine("Inside Download");

                    await DownloadAsync(config);
                }
                else
                {
                    Console.WriteLine("Inside Run Async");

                    await RunAsync(config);
                }
                #endregion
            }

            Console.WriteLine("Process Complete!");
        }


        private static async Task PerformChecksumTest()
        {
            string storageConnection = "";

            //string containerName = "0f1c4593-c132-4037-9f5d-cee8552346b2-processed";
            //string filePathToTest = "95481721-87e9-42e5-9c18-3612a15fe747/c6b0432d-8149-479c-bef8-489c735f0e67/fnl/c6b0432d-8149-479c-bef8-489c735f0e67.mpd";

            string containerName = "0f1c4593-c132-4037-9f5d-cee8552346b2-mezzanine";
            string filePathToTest = "61b31451-5d29-4e28-8ed0-13d26decca95/d541e59d-9bb6-4262-a3b3-bfe3bee64086/wrking/d541e59d-9bb6-4262-a3b3-bfe3bee64086.mpd";

            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnection);

            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlockBlobClient sourceBlob = blobContainerClient.GetBlockBlobClient(filePathToTest);

            Tuple<long, string> checksumInfo = await GetBlobChecksumAndLength(sourceBlob);
        }

        private static async Task<Tuple<long, string>> GetBlobChecksumAndLength(BlockBlobClient blockBlobClient)
        {
            string checksum = string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                await blockBlobClient.DownloadToAsync(stream);

                stream.Position = 0;

                checksum = EventHandlingUtilities.GetChecksum(stream);
            }

            BlobProperties blobProperties = await blockBlobClient.GetPropertiesAsync();

            return new Tuple<long, string>(blobProperties.ContentLength, checksum);

        }

        private static async Task DownloadAsync(AppSettings config)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(config.StorageConnection);

                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("sourcecontainer");

                Console.WriteLine($"****Starting downloading Segments to blob****** {DateTime.Now.ToString()}");

                MpdInfo segmentInfo = await SegmentDownloader.DownloadSegments(_assestIngestUrl,
                                                                       _blobworkingDirectory,
                                                                       _uniqueness_raw.ToString(),
                                                                       blobContainerClient);

                Console.WriteLine($"****Completed downloading Segments to blob****** {DateTime.Now.ToString()}");

                Console.WriteLine($"****Moving content to Tar Files to blob****** {DateTime.Now.ToString()}");

                await MoveContentToFinalBlob(config, blobContainerClient, segmentInfo);

                Console.WriteLine($"****Moving content ended to Tar Files to blob****** {DateTime.Now.ToString()}");

            }
            catch (Exception ex)
            {
                string exs = ex.ToString();

                Console.WriteLine(exs);
            }
        }


        private static async Task RunAsync(AppSettings config)
        {
            IAzureMediaServicesClient client;
            try
            {
                client = await CreateMediaServicesClientAsync(config);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Make sure that you have filled out the appsettings.json file.");
                Console.Error.WriteLine($"{e.Message}");
                return;
            }

            // Set the polling interval for long running operations to 2 seconds.
            // The default value is 30 seconds for the .NET client SDK
            client.LongRunningOperationRetryTimeout = 2;
            
            string jobName = $"{C_PREFIX}-{_uniqueness}";
            
            string locatorName = $"{C_PREFIX}-locator-{_uniqueness}";
            
            string outputAssetName = $"{C_PREFIX}-output-{_uniqueness}";

            Console.WriteLine($"Creating Job with Name : {jobName}");

            Console.WriteLine($"Creating Locator with Name : {locatorName}");

            Console.WriteLine($"Creating Output Asset with Name : {outputAssetName}");

            try
            {
                // Ensure that you have the desired encoding Transform. This is really a one time setup operation.
                //Transform transform = await GetOrCreateTransformAsync(client, config.ResourceGroup, config.AccountName, AdaptiveStreamingTransformName);

                Transform transform = EnsureTransformExists(config, client, config.ResourceGroup, config.AccountName, config.BineTransformName);

                // Output from the encoding Job must be written to an Asset, so let's create one
                Asset outputAsset = await CreateOutputAssetAsync(client, config.ResourceGroup, config.AccountName, outputAssetName);

                Job job = await SubmitJobAsync(client, config.ResourceGroup, config.AccountName, config.BineTransformName, outputAsset.Name, jobName);

                Console.WriteLine("Polling job status. It may take hours depending on the file size.");

                job = await WaitForJobToFinishAsync(client, config.ResourceGroup, config.AccountName, config.BineTransformName, jobName);

                if (job.State == JobState.Finished)
                {
                    // Set a token signing key that you want to use
                    TokenSigningKey = Convert.FromBase64String(config.SymmetricKey);

                    // Create the content key policy that configures how the content key is delivered to end clients
                    // via the Key Delivery component of Azure Media Services.
                    // We are using the ContentKeyIdentifierClaim in the ContentKeyPolicy which means that the token presented
                    // to the Key Delivery Component must have the identifier of the content key in it. 
                    ContentKeyPolicy policy = await GetOrCreateContentKeyPolicyAsync(config, client, config.ResourceGroup, config.AccountName, config.BineContentKeyPolicyName, TokenSigningKey);

                    // Sets StreamingLocator.StreamingPolicyName to "Predefined_MultiDrmCencStreaming" policy.
                    StreamingLocator locator = await CreateStreamingLocatorAsync(client, config.ResourceGroup, config.AccountName, outputAsset.Name, locatorName, config.BineContentKeyPolicyName);

                    // In this example, we want to play the Widevine (CENC) encrypted stream. 
                    // We need to get the key identifier of the content key where its type is CommonEncryptionCenc.
                    string keyIdentifier = locator.ContentKeys.Where(k => k.Type == StreamingLocatorContentKeyType.CommonEncryptionCenc).First().Id.ToString();

                    Console.WriteLine($"KeyIdentifier = {keyIdentifier}");

                    // In order to generate our test token we must get the ContentKeyId to put in the ContentKeyIdentifierClaim claim.
                    string token = GetTokenAsync(config.TokenIssuer, config.TokenAudience, keyIdentifier, TokenSigningKey);

                    StreamingEndpoint streamingEndpoint = await client.StreamingEndpoints.GetAsync(config.ResourceGroup, config.AccountName,
                        DefaultStreamingEndpointName);


                    if (streamingEndpoint != null)
                    {
                        if (streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running)
                        {
                            Console.WriteLine("Streaming Endpoint was Stopped, restarting now..");

                            await client.StreamingEndpoints.StartAsync(config.ResourceGroup, config.AccountName, DefaultStreamingEndpointName);
                        }
                    }
                    string dashPath = await GetDASHStreamingUrlAsync(client, config.ResourceGroup, config.AccountName, locator.Name, streamingEndpoint);

                    Console.WriteLine($"Dash URL : {dashPath}");

                    File.AppendAllLines(_outputLogPath, new string[] { $"Dash URL : {dashPath}", System.Environment.NewLine });

                    Console.WriteLine("Copy and paste the following URL in your browser to play back the file in the Azure Media Player.");

                    Console.WriteLine("You can use Chrome for Widevine.");

                    Console.WriteLine();

                    Console.WriteLine($"https://ampdemo.azureedge.net/?url={dashPath}&widevine=true&token=Bearer%3D{token}");

                    File.AppendAllLines(_outputLogPath, new string[] { $"https://ampdemo.azureedge.net/?url={dashPath}&widevine=true&token=Bearer%3D{token}", System.Environment.NewLine });

                    Console.WriteLine();

                    Console.WriteLine($"****Starting downloading Segments - {DateTime.Now.ToString()}******");

                    Console.WriteLine($"****Please monitor progress at {_rootDirectory} ******");

                    MpdInfo segmentInfo;

                    if (config.DownloadToBlob)
                    {
                        BlobServiceClient blobServiceClient = new BlobServiceClient(config.StorageConnection);

                        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("sourcecontainer");

                        segmentInfo = await SegmentDownloader.DownloadSegments(dashPath,
                                                                               _blobworkingDirectory, 
                                                                               _uniqueness_raw.ToString(), 
                                                                               blobContainerClient);

                        Console.WriteLine($"****Completed downloading Segments to blob - {DateTime.Now.ToString()}******");

                        Console.WriteLine($"****Moving content to Tar Files to blob****** - {DateTime.Now.ToString()}***");

                        await MoveContentToFinalBlob(config, blobContainerClient, segmentInfo);

                       Console.WriteLine("****Tar File Generated at blob******");
                    }
                    else
                    {
                        segmentInfo = await SegmentDownloader.DownloadSegments(dashPath, _workingDirectory, _uniqueness_raw.ToString());

                        Console.WriteLine("****Completed downloading Segments******");

                        Console.WriteLine("****Moving content to Tar Files******");

                        MoveContentToFinal(config, segmentInfo);

                        Console.WriteLine("****Tar File Generated******");

                    }
                }

                Console.Out.Flush();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hit GeneralErrorException");

                File.AppendAllLines(_outputLogPath, new string[] { $"{ex.ToString()}", System.Environment.NewLine });
            }
        }

        /// <summary>
        /// Moves the segments, xml, dummy file to final and creates tar
        /// </summary>
        /// <param name="segmentInfo"></param>
        public static void MoveContentToFinal(AppSettings config, MpdInfo segmentInfo)
        {
            string tarPath;

            if (!System.IO.Directory.Exists(_finalDirectory))
            {
                System.IO.Directory.CreateDirectory(_finalDirectory);
            }

            foreach (AdaptiveSetInfo adaptiveSet in segmentInfo.AdaptiveSets)
            {
                tarPath = Path.Combine(_workingDirectory,$"{adaptiveSet.DirectoryName}.tar");

                adaptiveSet.FinalPath = Path.Combine(_finalDirectory, $"{_uniqueness_raw.ToString()}_{adaptiveSet.DirectoryName}.tar");

                TarHelper.TarCreateFromStream(tarPath, Path.Combine(_workingDirectory, adaptiveSet.DirectoryName));

                File.Move(tarPath, adaptiveSet.FinalPath);
            }

            string mpdPath = Path.Combine(_workingDirectory, $"{segmentInfo.MpdName}");

            segmentInfo.FinalMpdPath = Path.Combine(_finalDirectory, $"{segmentInfo.MpdName}");

            File.Move(mpdPath, segmentInfo.FinalMpdPath);

            string xmlFilePath = Path.Combine(_finalDirectory, $"{_uniqueness_raw}{config.XmlFileName}");

            //copy the template to final directory
            File.Copy(_xmlTemplateFileName, xmlFilePath);

            //replace tags in xml template
            ReplaceTokenInXml(xmlFilePath, segmentInfo);

            string finalTarPath = Path.Combine(_rootDirectory, $"{_uniqueness_raw.ToString()}.tar");

            TarHelper.TarCreateFromStream(finalTarPath, _finalDirectory);
        }

        private static async Task MoveContentToFinalBlob(AppSettings config,
                                                    BlobContainerClient mezzContainer,
                                                    MpdInfo mpdInfo)
        {
            string tarFileName;

            string tarPath;

            string tarSourceDirectory;

            BlockBlobClient sourceBlob;

            BlockBlobClient targetBlob;

            foreach (AdaptiveSetInfo adaptiveSet in mpdInfo.AdaptiveSets)
            {
                tarFileName = $"{adaptiveSet.DirectoryName}.tar";

                tarPath = $"{_blobworkingDirectory}/{tarFileName}";

                //appending slash at the end so that list blobs returns all the child values only
                tarSourceDirectory = $"{_blobworkingDirectory}/{adaptiveSet.DirectoryName}/";

                adaptiveSet.Length = await TarHelper.TarCreateFromMemoryStream(mezzContainer, tarPath, tarSourceDirectory);

                adaptiveSet.FinalPath = $"{_blobFinalDirectory}/{_uniqueness_raw}_{adaptiveSet.DirectoryName}.tar";

                sourceBlob = mezzContainer.GetBlockBlobClient(tarPath);

                targetBlob = mezzContainer.GetBlockBlobClient(adaptiveSet.FinalPath);

                await EventHandlingUtilities.CopyBlob(sourceBlob, targetBlob);

            }

            string mpdPath = $"{_blobworkingDirectory}/{mpdInfo.MpdName}";

            mpdInfo.FinalMpdPath = $"{_blobFinalDirectory}/{mpdInfo.MpdName}";

            sourceBlob = mezzContainer.GetBlockBlobClient(mpdPath);

            targetBlob = mezzContainer.GetBlockBlobClient(mpdInfo.FinalMpdPath);

            await EventHandlingUtilities.CopyBlob(sourceBlob, targetBlob);

            string xmlFilePath = $"{_blobFinalDirectory}/{_uniqueness_raw}{config.XmlFileName}";

            string xmlFileContent = File.ReadAllText(_xmlTemplateFileName);

            xmlFileContent = await ReplaceTokenInXmlString(mezzContainer, xmlFileContent, mpdInfo);

            await EventHandlingUtilities.UploadBlob(mezzContainer, xmlFilePath, xmlFileContent);

            string finalTarPath = $"{_blobrootDirectory}/{_uniqueness_raw}.tar";

            string finalTarFileName = $"{_uniqueness_raw}.tar";

            await TarHelper.TarCreateFromMemoryStream(mezzContainer, finalTarPath,_blobFinalDirectory);

        }

        private static  async Task<string> ReplaceTokenInXmlString(BlobContainerClient mezzContainer,
                                       string xmlContent,
                                       MpdInfo segmentInfo)
        {
            AdaptiveSetInfo audioSet = segmentInfo.AdaptiveSets.Where(audio => (audio.Type == ApplicationConstants.AdaptiveSetTypes.Audio)).FirstOrDefault();

            BlobProperties blobProperties = await mezzContainer.GetBlockBlobClient(audioSet.FinalPath).GetPropertiesAsync();

            xmlContent = xmlContent.Replace(XMLConstants.AUDIO_TAR, audioSet.FinalPath.Split('/').Last());

            //xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.AUDIO_FILE_CHECKSUM, GetChecksum(audioSet.FinalPath));

            xmlContent = xmlContent.Replace(XMLConstants.AUDIO_FILE_SIZE, blobProperties.ContentLength.ToString());

            xmlContent = xmlContent.Replace(XMLConstants.AUDIO_TAR_FOLDER_NAME, audioSet.DirectoryName);

            AdaptiveSetInfo videoSet = segmentInfo.AdaptiveSets.Where(audio => (audio.Type == ApplicationConstants.AdaptiveSetTypes.Video)).FirstOrDefault();

            blobProperties = await mezzContainer.GetBlockBlobClient(videoSet.FinalPath).GetPropertiesAsync();

            xmlContent = xmlContent.Replace(XMLConstants.VIDEO_TAR, videoSet.FinalPath.Split('/').Last());

            //xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.VIDEO_FILE_CHECKSUM, GetChecksum(videoSet.FinalPath));

            xmlContent = xmlContent.Replace(XMLConstants.VIDEO_FILE_SIZE, blobProperties.ContentLength.ToString());

            xmlContent = xmlContent.Replace(XMLConstants.VIDEO_TAR_FOLDER_NAME, videoSet.DirectoryName);

            blobProperties = await mezzContainer.GetBlockBlobClient(segmentInfo.FinalMpdPath).GetPropertiesAsync();

            xmlContent = xmlContent.Replace(XMLConstants.MPD_FILE, segmentInfo.FinalMpdPath.Split('/').Last());

            //xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.MPD_FILE_CHECKSUM, GetChecksum(segmentInfo.FinalMpdPath));

            xmlContent = xmlContent.Replace(XMLConstants.MPD_FILE_SIZE, blobProperties.ContentLength.ToString());

            xmlContent = xmlContent.Replace(XMLConstants.UNIQUE_ID,_uniqueness_raw.ToString());

            return xmlContent;
        }


        private static void ReplaceTokenInXml(string xmlFilePath, MpdInfo segmentInfo)
        {
            string fileContent = File.ReadAllText(xmlFilePath);

            fileContent = fileContent.Replace(XMLConstants.START_DATE, DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss"));
             
            fileContent = fileContent.Replace(XMLConstants.END_DATE, DateTime.UtcNow.AddMonths(6).ToString("yyyy-MM-ddTHH:mm:ss"));

            AdaptiveSetInfo audioSet = segmentInfo.AdaptiveSets.Where(audio => (audio.Type == "audio")).FirstOrDefault();

            FileInfo fileInfo = new FileInfo(audioSet.FinalPath);

            fileContent = fileContent.Replace(XMLConstants.AUDIO_TAR, Path.GetFileName(audioSet.FinalPath));

            fileContent = fileContent.Replace(XMLConstants.AUDIO_FILE_CHECKSUM, GetChecksum(audioSet.FinalPath));

            fileContent = fileContent.Replace(XMLConstants.AUDIO_FILE_SIZE, fileInfo.Length.ToString());

            fileContent = fileContent.Replace(XMLConstants.AUDIO_TAR_FOLDER_NAME, audioSet.DirectoryName);


            AdaptiveSetInfo videoSet = segmentInfo.AdaptiveSets.Where(audio => (audio.Type == "video")).FirstOrDefault();

            fileInfo = new FileInfo(videoSet.FinalPath);
            
            fileContent = fileContent.Replace(XMLConstants.VIDEO_TAR, Path.GetFileName(videoSet.FinalPath));
            
            fileContent = fileContent.Replace(XMLConstants.VIDEO_FILE_CHECKSUM, GetChecksum(videoSet.FinalPath));
            
            fileContent = fileContent.Replace(XMLConstants.VIDEO_FILE_SIZE, fileInfo.Length.ToString());

            fileContent = fileContent.Replace(XMLConstants.VIDEO_TAR_FOLDER_NAME, videoSet.DirectoryName);

            fileInfo = new FileInfo(segmentInfo.FinalMpdPath);
            
            fileContent = fileContent.Replace(XMLConstants.MPD_FILE, Path.GetFileName(segmentInfo.FinalMpdPath));
            
            fileContent = fileContent.Replace(XMLConstants.MPD_FILE_CHECKSUM, GetChecksum(segmentInfo.FinalMpdPath));
            
            fileContent = fileContent.Replace(XMLConstants.MPD_FILE_SIZE, fileInfo.Length.ToString());

            fileContent = fileContent.Replace(XMLConstants.UNIQUE_ID, _uniqueness_raw.ToString());

            File.WriteAllText(xmlFilePath, fileContent);
        }


        private static string GetChecksum(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty).ToLowerInvariant();
            }
        }

        /// <summary>
        /// Creates a StreamingLocator for the specified asset and with the specified streaming policy name.
        /// Once the StreamingLocator is created the output asset is available to clients for playback.
        /// This StreamingLocator uses "Predefined_MultiDrmCencStreaming" 
        /// because this sample encrypts with Widevine (CENC encryption).  
        /// "Predefined_MultiDrmCencStreaming" policy also adds AES encryption.
        /// As a result, two content keys are added to the StreamingLocator.
        /// 
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The name of the output asset.</param>
        /// <param name="locatorName">The StreamingLocator name (unique in this case).</param>
        /// <returns></returns>
        private static async Task<StreamingLocator> CreateStreamingLocatorAsync(
            IAzureMediaServicesClient client,
            string resourceGroup,
            string accountName,
            string assetName,
            string locatorName,
            string contentPolicyName)
        {
            StreamingLocator locator = await client.StreamingLocators.GetAsync(resourceGroup, accountName, locatorName);

            if (locator != null)
            {
                // Name collision! This should not happen in this sample. If it does happen, in order to get the sample to work,
                // let's just go ahead and create a unique name.
                // Note that the returned locatorName can have a different name than the one specified as an input parameter.
                // You may want to update this part to throw an Exception instead, and handle name collisions differently.
                Console.WriteLine("Warning – found an existing Streaming Locator with name = " + locatorName);

                string uniqueness = $"-{Guid.NewGuid().ToString("N")}";
                locatorName += uniqueness;

                Console.WriteLine("Creating a Streaming Locator with this name instead: " + locatorName);
            }

            locator = await client.StreamingLocators.CreateAsync(
                resourceGroup,
                accountName,
                locatorName,
                new StreamingLocator
                {
                    AssetName = assetName,
                    // "Predefined_MultiDrmCencStreaming" policy supports envelope and cenc encryption
                    StreamingPolicyName = "Predefined_MultiDrmCencStreaming",
                    DefaultContentKeyPolicyName = contentPolicyName

                });

            return locator;
        }

        /// <summary>
        /// Creates the AzureMediaServicesClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="config">The param is of type ConfigWrapper. This class reads values from local configuration file.</param>
        /// <returns></returns>
        private static async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync(AppSettings config)
        {
            var credentials = await GetCredentialsAsync(config);

            return new AzureMediaServicesClient(config.ArmEndpoint, credentials)
            {
                SubscriptionId = config.SubscriptionId,
            };
        }


        /// <summary>
        /// Create the ServiceClientCredentials object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="config">The param is of type ConfigWrapper. This class reads values from local configuration file.</param>
        /// <returns></returns>
        private static async Task<ServiceClientCredentials> GetCredentialsAsync(AppSettings config)
        {
            // Use ApplicationTokenProvider.LoginSilentAsync to get a token using a service principal with symmetric key
            ClientCredential clientCredential = new ClientCredential(config.AadClientId, config.AadSecret);

            return await ApplicationTokenProvider.LoginSilentAsync(config.AadTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);
        }

        private static Transform EnsureTransformExists(AppSettings config, IAzureMediaServicesClient client,
                                                        string resourceGroupName,
                                                        string accountName,
                                                        string transformName)
        {
            // Does a Transform already exist with the desired name? Assume that an existing Transform with the desired name
            // also uses the same recipe or Preset for processing content.
            Transform transform = client.Transforms.Get(resourceGroupName, accountName, transformName);

            if (transform == null)
            {
                // Create a new Transform Outputs array - this defines the set of outputs for the Transform
                TransformOutput[] outputs = new TransformOutput[]
                {
                    // Create a new TransformOutput with a custom Standard Encoder Preset
                    // This demonstrates how to create custom codec and layer output settings

                 new TransformOutput(
                    new StandardEncoderPreset(
                    codecs: new Codec[]
                    {
                        // Add an AAC Audio layer for the audio encoding
                        new AacAudio(
                            channels: 2,
                            samplingRate: 48000,
                            bitrate: 128000,
                            profile: AacAudioProfile.AacLc
                        ),
                        // Next, add a H264Video for the video encoding
                       new H264Video (
                            // Set the GOP interval to 2 seconds for both H264Layers
                            keyFrameInterval:TimeSpan.FromSeconds(config.KeyFrameInterval),
                            //  Add H264Layers, one at HD and the other at SD. Assign a label that you can use for the output filename
                            layers:  new H264Layer[]
                            {
                                new H264Layer (
                                    bitrate: config.Bitrate,
                                    width: $"{config.Width.ToString()}",
                                    height: $"{config.Height.ToString()}",
                                    label: $"{config.Label}"
                                )
                            }
                        ),
                        //// Also generate a set of PNG thumbnails
                        //new PngImage(
                        //    start: "25%",
                        //    step: "25%",
                        //    range: "80%",
                        //    layers: new PngLayer[]{
                        //        new PngLayer(
                        //            width: "50%",
                        //            height: "50%"
                        //        )
                        //    }
                        //)
                    },
                    // Specify the format for the output files - one for video+audio, and another for the thumbnails
                    formats: new Format[]
                    {
                        // Mux the H.264 video and AAC audio into MP4 files, using basename, label, bitrate and extension macros
                        // Note that since you have multiple H264Layers defined above, you have to use a macro that produces unique names per H264Layer
                        // Either {Label} or {Bitrate} should suffice
                         
                        new Mp4Format(
                            filenamePattern:"Video-{Basename}-{Label}-{Bitrate}{Extension}"
                        ),
                        //new PngFormat(
                        //    filenamePattern:"Thumbnail-{Basename}-{Index}{Extension}"
                        //)
                    }
                ),
                onError: OnErrorType.StopProcessingJob,
                relativePriority: Priority.Normal
            )
                };

                string description = "A custom encoding transform with 1 bitrate for bine";
                // Create the custom Transform with the outputs defined above
                transform = client.Transforms.CreateOrUpdate(resourceGroupName, accountName, transformName, outputs, description);
            }

            return transform;
        }


        /// <summary>
        /// Creates an output asset. The output from the encoding Job must be written to an Asset.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The output asset name.</param>
        /// <returns></returns>
        private static async Task<Asset> CreateOutputAssetAsync(IAzureMediaServicesClient client, string resourceGroupName, string accountName, string assetName)
        {
            // Check if an Asset already exists
            Asset outputAsset = await client.Assets.GetAsync(resourceGroupName, accountName, assetName);
            Asset asset = new Asset();
            string outputAssetName = assetName;

            if (outputAsset != null)
            {
                // Name collision! In order to get the sample to work, let's just go ahead and create a unique asset name
                // Note that the returned Asset can have a different name than the one specified as an input parameter.
                // You may want to update this part to throw an Exception instead, and handle name collisions differently.
                string uniqueness = $"-{Guid.NewGuid().ToString("N")}";
                outputAssetName += uniqueness;

                Console.WriteLine("Warning – found an existing Asset with name = " + assetName);
                Console.WriteLine("Creating an Asset with this name instead: " + outputAssetName);
            }

            Console.WriteLine("Creating an output asset...");
            return await client.Assets.CreateOrUpdateAsync(resourceGroupName, accountName, outputAssetName, asset);
        }



        /// <summary>
        /// Submits a request to Media Services to apply the specified Transform to a given input video.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The name of the transform.</param>
        /// <param name="outputAssetName">The (unique) name of the  output asset that will store the result of the encoding job. </param>
        /// <param name="jobName">The (unique) name of the job.</param>
        /// <returns></returns>
        private static async Task<Job> SubmitJobAsync(IAzureMediaServicesClient client,
            string resourceGroup,
            string accountName,
            string transformName,
            string outputAssetName,
            string jobName)
        {

            // This example shows how to encode from any HTTPs source URL - a new feature of the v3 API.  
            // Change the URL to any accessible HTTPs URL or SAS URL from Azure.
            JobInputHttp jobInput = 
                new JobInputHttp(files: new[] { _assestIngestUrl });

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(outputAssetName),
            };

            // In this example, we are assuming that the job name is unique.
            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, the Get method on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            Console.WriteLine("Creating a job...");
            Job job = await client.Jobs.CreateAsync(
                resourceGroup,
                accountName,
                transformName,
                jobName,
                new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs,
                });

            return job;
        }


        /// <summary>
        /// Polls Media Services for the status of the Job.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The name of the transform.</param>
        /// <param name="jobName">The name of the job you submitted.</param>
        /// <returns></returns>
        private static async Task<Job> WaitForJobToFinishAsync(IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string transformName,
            string jobName)
        {
            const int SleepIntervalMs = 30 * 1000;

            Job job;

            do
            {
                job = await client.Jobs.GetAsync(resourceGroupName, accountName, transformName, jobName);

                Console.WriteLine($"Job is '{job.State}'.");

                for (int i = 0; i < job.Outputs.Count; i++)
                {
                    JobOutput output = job.Outputs[i];

                    Console.Write($"\tJobOutput[{i}] is '{output.State}'.");
                    
                    if (output.State == JobState.Processing)
                    {
                        Console.Write($"Progress (%): '{output.Progress}'.");
                    }

                    Console.WriteLine();
                }

                if (job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled)
                {
                    await Task.Delay(SleepIntervalMs);
                }
            }
            while (job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled);

            return job;
        }


        /// <summary>
        /// Create the content key policy that configures how the content key is delivered to end clients 
        /// via the Key Delivery component of Azure Media Services.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="contentKeyPolicyName">The name of the content key policy resource.</param>
        /// <returns></returns>
        private static async Task<ContentKeyPolicy> GetOrCreateContentKeyPolicyAsync(
            AppSettings appSettings,
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string contentKeyPolicyName,
            byte[] tokenSigningKey)
        {
            ContentKeyPolicy policy = await client.ContentKeyPolicies.GetAsync(resourceGroupName, accountName, contentKeyPolicyName);

            if (policy == null)
            {
                ContentKeyPolicySymmetricTokenKey primaryKey = new ContentKeyPolicySymmetricTokenKey(tokenSigningKey);

                List<ContentKeyPolicyTokenClaim> requiredClaims = new List<ContentKeyPolicyTokenClaim>()
                {
                    ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim
                };

                List<ContentKeyPolicyRestrictionTokenKey> alternateKeys = null;

                ContentKeyPolicyTokenRestriction restriction
                    = new ContentKeyPolicyTokenRestriction(appSettings.TokenIssuer, appSettings.TokenAudience, primaryKey, ContentKeyPolicyRestrictionTokenType.Jwt, alternateKeys, requiredClaims);

                ContentKeyPolicyWidevineConfiguration widevineConfig = ConfigureWidevineLicenseTempate();

                List<ContentKeyPolicyOption> options = new List<ContentKeyPolicyOption>
                {
                    new ContentKeyPolicyOption()
                    {
                        Configuration = widevineConfig,
                        Restriction = restriction
                    }
                };

                policy = await client.ContentKeyPolicies.CreateOrUpdateAsync(resourceGroupName, accountName, contentKeyPolicyName, options);
            }
            else
            {
                // Get the signing key from the existing policy.
                var policyProperties = await client.ContentKeyPolicies.GetPolicyPropertiesWithSecretsAsync(resourceGroupName, accountName, contentKeyPolicyName);

                if (policyProperties.Options[0].Restriction is ContentKeyPolicyTokenRestriction restriction)
                {
                    if (restriction.PrimaryVerificationKey is ContentKeyPolicySymmetricTokenKey signingKey)
                    {
                        TokenSigningKey = signingKey.KeyValue;
                    }
                }
            }
            return policy;
        }


        /// <summary>
        /// Configures Widevine license template.
        /// </summary>
        /// <returns></returns>
        private static ContentKeyPolicyWidevineConfiguration ConfigureWidevineLicenseTempate()
        {
            WidevineTemplate template = new WidevineTemplate()
            {
                AllowedTrackTypes = "SD_HD",
                ContentKeySpecs = new ContentKeySpec[]
                {
                    new ContentKeySpec()
                    {
                        TrackType = "SD",
                        SecurityLevel = 1,
                        RequiredOutputProtection = new OutputProtection()
                        {
                            HDCP = "HDCP_NONE"
                        }
                    }
                },
                PolicyOverrides = new PolicyOverrides()
                {
                    CanPlay = true,
                    CanPersist = true,
                    CanRenew = false,
                    RentalDurationSeconds = 2592000,
                    PlaybackDurationSeconds = 10800,
                    LicenseDurationSeconds = 604800,
                }
            };

            ContentKeyPolicyWidevineConfiguration objContentKeyPolicyWidevineConfiguration = new ContentKeyPolicyWidevineConfiguration
            {
                WidevineTemplate = Newtonsoft.Json.JsonConvert.SerializeObject(template)
            };

            return objContentKeyPolicyWidevineConfiguration;
        }

        /// <summary>
        /// Create a token that will be used to protect your stream.
        /// Only authorized clients would be able to play the video.  
        /// </summary>
        /// <param name="issuer">The issuer is the secure token service that issues the token. </param>
        /// <param name="audience">The audience, sometimes called scope, describes the intent of the token or the resource the token authorizes access to. </param>
        /// <param name="keyIdentifier">The content key ID.</param>
        /// <param name="tokenVerificationKey">Contains the key that the token was signed with. </param>
        /// <returns></returns>
        private static string GetTokenAsync(string issuer, string audience, string keyIdentifier, byte[] tokenVerificationKey)
        {
            var tokenSigningKey = new SymmetricSecurityKey(tokenVerificationKey);

            SigningCredentials cred = new SigningCredentials(
                tokenSigningKey,
                // Use the  HmacSha256 and not the HmacSha256Signature option, or the token will not work!
                SecurityAlgorithms.HmacSha256,
                SecurityAlgorithms.Sha256Digest);

            Claim[] claims = new Claim[]
            {
                new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now.AddMinutes(-5),
                expires: DateTime.Now.AddMinutes(262800),
                signingCredentials: cred);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }


        /// <summary>
        /// Checks if the "default" streaming endpoint is in the running state,
        /// if not, starts it.
        /// Then, builds the streaming URLs.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="locatorName">The name of the StreamingLocator that was created.</param>
        /// <param name="streamingEndpoint">The streaming endpoint.</param>
        /// <returns></returns>
        private static async Task<string> GetDASHStreamingUrlAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string locatorName,
            StreamingEndpoint streamingEndpoint)
        {
            string dashPath = "";

            ListPathsResponse paths = await client.StreamingLocators.ListPathsAsync(resourceGroupName, accountName, locatorName);

            foreach (StreamingPath path in paths.StreamingPaths)
            {
                UriBuilder uriBuilder = new UriBuilder
                {
                    Scheme = "https",
                    Host = streamingEndpoint.HostName
                };

                // Look for just the DASH path and generate a URL for the Azure Media Player to playback the encrypted DASH content. 
                // Note that the JWT token is set to expire in 1 hour. 
                if (path.StreamingProtocol == StreamingPolicyStreamingProtocol.Dash)
                {
                    uriBuilder.Path = path.Paths[0];
                    dashPath = uriBuilder.ToString();
                }
            }

            return dashPath;
        }
    }
}