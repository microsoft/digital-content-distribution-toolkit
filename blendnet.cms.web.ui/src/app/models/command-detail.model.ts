import { BroadcastRequest, Content } from "./content.model"

export class CommandDetail {
    constructor(
        public id: string,
        public contentId: string,
        public content: Content,
        public type: string,
        public commandType: string,
        public createdByUserId: string,
        public createdDate: Date,
        public modifiedDate: Date,
        public failureDetails: string[],
        public commandStatus: string,
        public broadcastRequest: BroadcastRequest,
        public executionDetails: ExecutionDetail

    ){}
}

class ExecutionDetail{
    constructor(
        public eventName: string,
        public contentBroadcastStatus: string,
        public eventDateTime: Date
    ){}
}