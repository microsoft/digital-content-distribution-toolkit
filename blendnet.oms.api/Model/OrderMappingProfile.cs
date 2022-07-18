using AutoMapper;
using blendnet.common.dto;
using blendnet.common.dto.Oms;

namespace blendnet.oms.api.Model
{
    /// <summary>
    /// OrderMappingProfile
    /// </summary>
    public class OrderMappingProfile: Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderResponse>();
            CreateMap<blendnet.common.dto.Oms.OrderItem, OrderItemResponse>();
            CreateMap<ContentProviderSubscriptionDto, ContentProviderSubscriptionResponseDto>();
        }
    }
}
