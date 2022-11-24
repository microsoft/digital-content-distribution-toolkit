// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { PlanDetails } from "./incentive.model";

export class Device {
    constructor(
        public createdByUserId: string,
        public createdDate: string,
        public deviceContainerType: string,
        public deviceId: string,
        public deviceStatus: string,
        public deviceStatusUpdatedOn: string,
        public filterUpdateStatus: string,
        public id: string
          
    ){}
}
