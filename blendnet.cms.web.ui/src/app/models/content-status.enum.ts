export enum ContentStatus {
    UPLOAD_SUBMITTED ="UploadSubmitted",
    UPLOAD_INPROGRESS ="UploadInProgress", 
    UPLOAD_FAILED ="UploadFailed",
    UPLOAD_COMPLETE ="UploadComplete",

    TRANSFORM_NOT_INITIALIZED = "TransformNotInitialized",
    TRANSFORM_SUBMITTED = "TransformSubmitted",
    TRANSFORM_INPROGRESS = "TransformInProgress",
    TRANSFORM_AMS_JOB_INPROGRESS = "TransformAMSJobInProgress",
    TRANSFORM_DOWNLOAD_INPROGRESS = "TransformDownloadInProgress",
    // TRANSFORM_DOWNLOAD_COMPLETE = "TransformDownloadComplete",
    TRANSFORM_COMPLETE = "TransformComplete",
    TRANSFORM_FAILED = "TransformFailed",

    BROADCAST_NOT_INITIALIZED = "BroadcastNotInitialized",
    BROADCAST_SUBMITTED = "BroadcastSubmitted",
    BROADCAST_INPROGRESS = "BroadcastInProgress",
    BROADCAST_TAR_PUSHED = "BroadcastTarPushed",
    BROADCAST_ORDER_CREATED = "BroadcastOrderCreated",
    BROADCAST_ORDER_ACTIVE = "BroadcastOrderActive",
    BROADCAST_ORDER_COMPLETE = "BroadcastOrderComplete",
    BROADCAST_ORDER_REJECTED = "BroadcastOrderRejected",
    BROADCAST_ORDER_FAILED = "BroadcastOrderFailed",
    BROADCAST_ORDER_CANCELLED = "BroadcastOrderCancelled",
    BROADCAST_FAILED = "BroadcastFailed",
    BROADCAST_COMPLETE = "BroadcastComplete",
    BROADCAST_CANCEL_SUBMITTED = "BroadcastCancelSubmitted",
    BROADCAST_CANCEL_INPROGRESS = "BroadcastCancelInProgress",
    BROADCAST_CANCEL_COMPLETE = "BroadcastCancelComplete",
    BROADCAST_CANCEL_FAILED = "BroadcastCancelFailed",


    ARCHIVED = 'ARCHIVED',
    ARCHIVING = 'ARCHIVING',
    DELETING = 'DELETING',
    BROADCAST = 'BROADCAST',
    REVOKE = 'REVOKE'
}
