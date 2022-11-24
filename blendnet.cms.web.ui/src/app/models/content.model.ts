// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ContentStatus } from "./content-status.enum";

export class Content {
    constructor(
        public id: string,
        public contentId: string,
        public contentProviderContentId: string,
        public title: string,
        public type: string,
        public contentProviderId: string,
        public status: ContentStatus,
        public shortDescription: string,
        public longDescription: string,
        public additionalDescription1: string,
        public additionalDescription2: string,
        public genre: string,
        public yearOfRelease: string,
        public language: string,
        public durationInMts: number,
        public rating: string,
        public mediaFileName: string,
        public hierarchy: string,
        public isHeaderContent: boolean,
        public isFreeContent: boolean,
        public artists: string[],
        public attachments: Attachment[],
        public isSelected: boolean,
        public dashUrl: string,
        public contentBroadcastStatus: string,
        public contentBroadcastStatusUpdatedBy: string,
        public contentTransformStatus: string,
        public contentTransformStatusUpdatedBy: string,
        public contentUploadStatus: string,
        public contentUploadStatusUpdatedBy: string,
        public ageAppropriateness: string,
        public contentAdvisory: string,
        public people: People[],
        public createdByUserId: string,
        public modifiedByByUserId: string,
        public createdDate: Date,
        public modifiedDate: Date,
        public audioTarFileSize: number,
        public videoTarFileSize: number,
        public isExclusiveContent: boolean,
        public isActive: boolean,
        public isBroadCastActive: boolean,
        public contentBroadcastedBy: ContentBroadcastDetails

    ) {} 
}

export class ContentBroadcastDetails{
    constructor(
        public commandId: string,
        public broadcastRequest: BroadcastRequest
       
    ){}
}

export class BroadcastRequest{
    constructor(
        public filters: string[],
        public startDate: Date,
        public endDate: Date
    ){}
}
class People {
    constructor(
        public name: string,
        public role: string
    ){}
}


class Attachment {
    constructor(
        public name: string,
        public type: AttachmentType
    ){}
}

export enum AttachmentType {
    Thumbnail = 0,
    Teaser = 1
}