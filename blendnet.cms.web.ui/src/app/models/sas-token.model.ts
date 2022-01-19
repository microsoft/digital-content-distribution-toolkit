export class SasToken {

    constructor(
        public storageAccount: string,
        public containerName: string,
        public policyName: string,
        public sasUri: string,
        public expiryInMinutes: number
        
    ) {}
}
