using AutoMapper;
using blendnet.cms.repository.CosmosRepository;

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
        }
    }
}
