// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ContentStatus } from "../models/content-status.enum";

export let unprocessedContentFilters = {
    "contentUploadStatuses": [
       ContentStatus.UPLOAD_SUBMITTED, 
       ContentStatus.UPLOAD_INPROGRESS, 
       ContentStatus.UPLOAD_FAILED,
       ContentStatus.UPLOAD_COMPLETE
    ],
    "contentTransformStatuses": [
      ContentStatus.TRANSFORM_NOT_INITIALIZED,
      ContentStatus.TRANSFORM_SUBMITTED,
      ContentStatus.TRANSFORM_INPROGRESS,
      ContentStatus.TRANSFORM_AMS_JOB_INPROGRESS,
      ContentStatus.TRANSFORM_DOWNLOAD_INPROGRESS,
      ContentStatus.TRANSFORM_FAILED
    ],
    "contentBroadcastStatuses": [
       ContentStatus.BROADCAST_NOT_INITIALIZED
    ]
  };


export let processedContentFilters = {
    "contentUploadStatuses": [
      ContentStatus.UPLOAD_COMPLETE
    ],
    "contentTransformStatuses": [
      ContentStatus.TRANSFORM_COMPLETE
    ],
    "contentBroadcastStatuses": [
      ContentStatus.BROADCAST_NOT_INITIALIZED, 
      ContentStatus.BROADCAST_SUBMITTED,
      ContentStatus.BROADCAST_INPROGRESS,
      ContentStatus.BROADCAST_FAILED,
      ContentStatus.BROADCAST_CANCEL_COMPLETE,
      ContentStatus.BROADCAST_ORDER_CANCELLED,
      ContentStatus.BROADCAST_ORDER_REJECTED,
      ContentStatus.BROADCAST_ORDER_FAILED,
      ContentStatus.BROADCAST_ORDER_COMPLETE
    ]
  };

export let  broadcastContentFilters = {
    "contentUploadStatuses": [
      ContentStatus.UPLOAD_COMPLETE
    ],
    "contentTransformStatuses": [
      ContentStatus.TRANSFORM_COMPLETE
    ],
    "contentBroadcastStatuses": [
      ContentStatus.BROADCAST_COMPLETE,
      ContentStatus.BROADCAST_TAR_PUSHED,
      ContentStatus.BROADCAST_CANCEL_INPROGRESS,
      ContentStatus.BROADCAST_ORDER_ACTIVE,
      ContentStatus.BROADCAST_ORDER_CANCELLED,
      ContentStatus.BROADCAST_ORDER_CREATED,
      ContentStatus.BROADCAST_ORDER_COMPLETE,
      ContentStatus.BROADCAST_ORDER_REJECTED,
      ContentStatus.BROADCAST_ORDER_FAILED,
      ContentStatus.BROADCAST_COMPLETE,
      ContentStatus.BROADCAST_CANCEL_SUBMITTED,
      ContentStatus.BROADCAST_CANCEL_INPROGRESS,
      ContentStatus.BROADCAST_CANCEL_FAILED,
      ContentStatus.BROADCAST_CANCEL_COMPLETE
    ]
  };

export let  broadcastCompleteContentFilters = {
  "contentUploadStatuses": [
    ContentStatus.UPLOAD_COMPLETE
  ],
  "contentTransformStatuses": [
    ContentStatus.TRANSFORM_COMPLETE
  ],
  "contentBroadcastStatuses": [
    ContentStatus.BROADCAST_ORDER_COMPLETE,
  ]
};