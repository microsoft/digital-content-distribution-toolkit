using blendnet.common.dto;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure.ApplicationInsights
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/azure-monitor/app/sampling#frequently-asked-questions
    /// https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-filtering-sampling#addmodify-properties-itelemetryinitializer
    /// In case of BlendNet custom event set the Sampling rate to 100 so that none of the events are missed.
    /// By default it will send 5 events per seconds and rest will be skipped.
    /// Initialize is fired for each request, trace etc.
    /// </summary>
    public class BlendNetTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is EventTelemetry)
            {
                EventTelemetry eventTelemetry = telemetry as EventTelemetry;

                if (eventTelemetry.Properties != null && 
                    eventTelemetry.Properties.Count > 0 &&
                    eventTelemetry.Properties.ContainsKey(ApplicationConstants.ApplicationInsightsDefaultEventProperty.BlendNetCustomEvent))
                {
                    ((ISupportSampling)telemetry).SamplingPercentage = 100;
                }
            }
        }
    }
}
