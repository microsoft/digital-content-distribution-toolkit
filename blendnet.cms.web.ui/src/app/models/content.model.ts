import { Attachment } from "./attachment.model";
import { ContentStatus } from "./content-status.enum";

export class Content {
    constructor(
        public id: string,
        public contentProviderContentId: string,
        public title: string,
        public status: ContentStatus,
        public shortDescription: string,
        public longDescription: string,
        public addionalDescription1: string,
        public addionalDescription2: string,
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
        public dashUrl: string
    ) {}
}
