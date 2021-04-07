export enum ContentStatus {
    UPLOAD_SUBMITTED ="UploadSubmitted",
    UPLOAD_INPROGRESS ="UploadInProgress", 
    UPLOAD_FAILED ="UploadFailed",
    UPLOAD_COMPLETE ="UploadComplete",

    TRANSFORM_NOT_INITIALIZED = 'TransformNotInitialized',
    TRANSFORM_AMS_JOB_INPROGRESS = 'TransformAMSJobInProgress',
    TRANSFORM_DOWNLOAD_INPROGRESS = "TransformDownloadInProgress",
    TRANSFORM_DOWNLOAD_COMPLETE = "TransformDownloadComplete",
    TRANSFORM_COMPLETE = "TransformComplete",
    TRANSFORM_FAILED = "TransformFailed",

    BROADCAST_NOT_INITIALIZED = "BroadcastNotInitialized",
    BROADCAST_INPROGRESS = "BroadcastInProgress",
    BROADCAST_FAILED = "BroadcastFailed",
    BROADCAST_COMPLETE = "BroadcastComplete",

    ARCHIVED = 'ARCHIVED',
    ARCHIVING = 'ARCHIVING',
    DELETING = 'DELETING',
    BROADCASTED = 'BROADCASTED',
    REVOKE = 'REVOKE'
}
