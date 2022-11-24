// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.AIEvents;
using Microsoft.ApplicationInsights;

namespace blendnet.common.infrastructure.Extensions
{
    public static class TelemetryClientExtensions
    {
        /// <summary>
        /// Wrapper extension to log a BaseAI event
        /// </summary>
        /// <param name="telemetryClient">telemetry client</param>
        /// <param name="aiEvent">event to be logged</param>
        public static void TrackEvent(this TelemetryClient telemetryClient, BaseAIEvent aiEvent)
        {
            telemetryClient.TrackEvent(aiEvent.GetAIEventName(), aiEvent.ToDictionary());
        }
    }
}