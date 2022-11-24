// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Device;
using blendnet.cosmos.utility.BroadCastMigration;
using blendnet.cosmos.utility.DeviceFilterMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cosmos.utility.MappingProfiles
{
    public class GenericMappingProfile:Profile
    {
        public GenericMappingProfile()
        {
            CreateMap<OldContent, Content>().ForMember(c => c.ContentBroadcastedBy, option => option.Ignore());

            CreateMap<OldDevice, Device>().ForMember(c => c.FilterUpdatedBy, option => option.Ignore());
        }
    }
}
