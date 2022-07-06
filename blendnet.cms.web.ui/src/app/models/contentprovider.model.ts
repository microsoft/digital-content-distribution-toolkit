import { ContentproviderAdmin } from "./contentprovider-admin";

export class Contentprovider {

    constructor(
        public id: string,
        public name: string,
        public logoUrl: string,
        public contentAdministrators: ContentproviderAdmin[],
        public isPublished: boolean
        
    ) {}
}

export class ContentProviderLtdInfo {
    constructor(
        public contentProviderId: string,
        public name: string
    ){}
}

