// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace blendnet.common.dto.Retailer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ServiceType
    {
        Media,
    }

    /// <summary>
    /// Represents the service type along with associated metadata
    /// e.g. Media, ...
    /// </summary>
    public class ServiceTypeDto
    {
        /// <summary>
        /// Name of the service type
        /// </summary>
        public ServiceType Id { get; set; }

        // TODO: Add more metadata
    }
}
