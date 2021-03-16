import { ContentproviderAdmin } from "./contentprovider-admin";

export class Contentprovider {

    constructor(
        public id: string,
        public name: string,
        public logoUrl: string,
        public status:boolean,
        public activationDate: Date,
        public deactivationDate: Date,
        public admins: ContentproviderAdmin[]
        
    ) {}
}

