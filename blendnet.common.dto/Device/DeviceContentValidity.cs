// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class DeviceContentValidity
    {
        /// <summary>
        /// Total Broadcasted Content
        /// </summary>
        public int TotalActiveBroacastedContent { get; set; }

        /// <summary>
        /// Device ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Device filter used to determining the validity
        /// </summary>
        public List<string> DeviceFiltersUsed { get; set; }

        /// <summary>
        /// Total content valid  broadcasted for this device
        /// </summary>
        public int TotalValidActiveBroacastedContent
        {
            get
            {
                if (ValidActiveBroadcastedContentList != null)
                {
                    return ValidActiveBroadcastedContentList.Count();
                }

                return 0;
            }
        }

        /// <summary>
        /// Total valid Content available on device
        /// </summary>
        public int TotalValidActiveAvailableContent
        {
            get
            {
                if (ValidActiveBroadcastedContentList != null)
                {
                    return ValidActiveBroadcastedContentList.Where(ba=>ba.IsAvailableOnDevice).Count();
                }

                return 0;
            }
        }


        public List<ContentValidity> ValidActiveBroadcastedContentList { get; set; }
    }

    /// <summary>
    /// Content Validity
    /// </summary>
    public class ContentValidity
    {
        /// <summary>
        /// Valid Broadcast Content
        /// </summary>
        public Content ValidActiveBroadcastedContent { get; set; }

        public DeviceContent DeviceContent { get; set; }

        /// <summary>
        /// If the content is available on device
        /// </summary>
        public bool IsAvailableOnDevice 
        {
            get
            {
                if (DeviceContent == null)
                {
                    return false;
                }

                return true;
            } 
        }
    }
}
