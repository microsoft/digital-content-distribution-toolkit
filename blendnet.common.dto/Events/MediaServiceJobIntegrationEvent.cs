// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Integration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Events
{
    public class MediaServiceJobIntegrationEvent: IntegrationEvent
    {
        public string topic { get; set; }
        public string subject { get; set; }
        public string eventType { get; set; }
        public string id { get; set; }
        public JobData data { get; set; }
        public string dataVersion { get; set; }
        public string metadataVersion { get; set; }
        public DateTime eventTime { get; set; }
    }

    public class JobData
    {
        public List<Output> outputs { get; set; }
        public string previousState { get; set; }
        public string state { get; set; }
        public CorrelationData correlationData { get; set; }
    }

    public class Output
    {
        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }
        public string assetName { get; set; }
        public object error { get; set; }
        public string label { get; set; }
        public int progress { get; set; }
        public string state { get; set; }
    }

    public class CorrelationData
    {
    }
}
