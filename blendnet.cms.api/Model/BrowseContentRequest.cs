﻿using System.ComponentModel.DataAnnotations;

namespace blendnet.cms.api.Model
{
    public class BrowseContentRequest
    {
        /// <summary>
        /// Continuation Token
        /// </summary>
        public string ContinuationToken { get; set; }

        /// <summary>
        /// page size
        /// </summary>
        [Range(1, 200)]
        public int PageSize { get; set; } = 100;
    }
}
