// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.cms.api.Model;

namespace blendnet.common.dto.Cms
{
    /// <summary>
    /// This creates a auto mapper for Content and ContentDto
    /// </summary>
    public class ContentMappingProfile : Profile
    {
        public ContentMappingProfile()
        {
            CreateMap<Content, ContentDto>();
            CreateMap<ContentDto, Content>();
            CreateMap<ContentProviderDto, ContentProviderItem>();
            CreateMap<ContentProviderItem, ContentProviderDto>();
            CreateMap<ContentInfo, Content>();
            CreateMap<Content, ContentInfo>();
            CreateMap<ContentBroadcastedBy, ContentBroadcastedByDto>();
            CreateMap<BroadcastRequest, BroadcastRequestDto>();
        }
    }
}
