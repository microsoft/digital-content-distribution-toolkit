// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ContentBroadcastDetails } from "./content.model";

export class ContentView {
    constructor(
        public id: string,
        public contentId: string,
        public title: string,
        public createdDate: Date,
        public modifiedDate: Date,
        public isActive: boolean,
        public contentBroadcastStatus: string,
        public contentBroadcastStatusUpdatedBy: string,
        public contentTransformStatus: string,
        public contentTransformStatusUpdatedBy: string,
        public contentUploadStatus: string,
        public contentUploadStatusUpdatedBy: string,
        public displayStatus: string,
        public displayCreatedDate: string,
        public displayModifiedDate: string,
        public isSelected: boolean,
        public contentBroadcastedBy : ContentBroadcastDetails,
        public dashUrl: string,
        public isBroadCastActive: boolean
    ) {} 
}
