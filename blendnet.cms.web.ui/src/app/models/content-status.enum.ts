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
    BROADCAST_INPROGRESS = "BroadcastInProgress",
    BROADCAST_SUBMITTED = "BroadcastSubmitted",
    BROADCAST_TAR_PUSHED = "BroadcastTarPushed",
    BROADCAST_FAILED = "BroadcastFailed",
    BROADCAST_COMPLETE = "BroadcastComplete",
    BROADCAST_CANCEL_INPROGRESS = "BroadcastCancelInProgress",


    ARCHIVED = 'ARCHIVED',
    ARCHIVING = 'ARCHIVING',
    DELETING = 'DELETING',
    BROADCAST = 'BROADCAST',
    REVOKE = 'REVOKE'
}
