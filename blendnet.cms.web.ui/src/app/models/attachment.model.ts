import { AttachmentType } from "./attachmentType.enum";

export class Attachment {
    constructor(
        
        public name: string,
        public attachmentType: AttachmentType

    ) {}
}

