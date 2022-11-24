// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.common.dto.Retailer;

namespace blendnet.retailer.api.Models
{
    /// <summary>
    /// This creates a auto mapper for RMS classes
    /// </summary>
    public class RetailerMappingProfile : Profile
    {
        public RetailerMappingProfile()
        {
            CreateMap<RetailerWithDistanceDto, RetailerWithDistanceResponse>();
            CreateMap<RetailerDto, RetailerResponse>();
        }
    }
}
