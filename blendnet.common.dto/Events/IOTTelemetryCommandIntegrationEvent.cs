// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Device;
using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Structure of data recieved in the telemetry recieved from IOT Central
    /// </summary>
    public class IOTTelemetryCommandIntegrationEvent : IntegrationEvent
    {
        [JsonPropertyName("applicationId")]
        public string ApplicationId { get; set; }

        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }

        [JsonPropertyName("messageSource")]
        public string MessageSource { get; set; }

        [JsonPropertyName("templateId")]
        public string TemplateId { get; set; }

        [JsonPropertyName("enqueuedTime")]
        public DateTime EnqueuedTime { get; set; }

        [JsonPropertyName("module")]
        public string Module { get; set; }

        [JsonPropertyName("telemetry")]
        public IOTTelemetry Telemetry { get; set; }

    }

    /// <summary>
    /// Represents Telemetry Property of the main object
    /// </summary>
    public class IOTTelemetry
    {
        [JsonPropertyName("deviceIdInData")]
        public string DeviceIdInData { get; set; }

        [JsonPropertyName("applicationName")]
        public string ApplicationName { get; set; }

        [JsonPropertyName("applicationVersion")]
        public string ApplicationVersion { get; set; }

        [JsonPropertyName("telemetryCommandData")]
        public IOTTelemetryCommand TelemetryCommandData { get; set; }

    }
}
