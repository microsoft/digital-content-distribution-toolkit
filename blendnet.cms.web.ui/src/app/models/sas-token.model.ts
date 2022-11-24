// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

export class SasToken {

    constructor(
        public storageAccount: string,
        public containerName: string,
        public policyName: string,
        public sasUri: string,
        public expiryInMinutes: number
        
    ) {}
}
