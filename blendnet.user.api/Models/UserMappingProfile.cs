// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.common.dto.User;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// OrderMappingProfile
    /// </summary>
    public class UserMappingProfile: Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserResponse>();
        }
    }
}
