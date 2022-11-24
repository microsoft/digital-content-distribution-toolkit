// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Telemetery Command
    /// </summary>
    public class IOTTelemetryCommand
    {
        [JsonPropertyName("commandName")]
        public IOTTelemetryCommandName? CommandName { get; set; }

        [JsonPropertyName("commandData")]
        public string CommandData { get; set; }

    }
    /// <summary>
    /// Supported Telemetery Commands
    /// </summary>
    public enum IOTTelemetryCommandName
    {
        ContentDownloaded = 0,
        ContentDeleted = 1,
        CompleteCommand = 2,
        ProvisionDevice = 3
    }
}
