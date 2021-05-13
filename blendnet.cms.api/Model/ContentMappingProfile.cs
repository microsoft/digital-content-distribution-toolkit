using AutoMapper;

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

        }
    }
}
