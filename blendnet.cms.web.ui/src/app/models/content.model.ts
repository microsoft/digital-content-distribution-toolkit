import { Attachment } from "./attachment.model";
import { ContentStatus } from "./content-status.enum";

export class Content {
    constructor(
        public id: string,
        public cpId: string,
        public cpContentId: string,
        public title: string,
        public status: ContentStatus,
        public shortDes: string,
        public longDes: string,
        public addDes1: string,
        public addDes2: string,
        public genre: string,
        public yearOfRelease: string,
        public language: string,
        public durationInMin: number,
        public rating: string,
        public mediaFileName: string,
        public hierarchy: string,
        public isHeaderContent: boolean,
        public isFreeContent: boolean,
        public artists: string[],
        public attachments: Attachment[]

    ) {}
}
