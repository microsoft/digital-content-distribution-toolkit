import { ContentStatus } from "./content-status.enum";

export class Content {
    constructor(
        public id: string,
        public name: string,
        public status: ContentStatus
    
    ) {}
}
