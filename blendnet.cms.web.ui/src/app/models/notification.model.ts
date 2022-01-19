export class Notification {

    constructor(
        public title: string,
        public description: string,
        public type: string,
        public attachment: string,
        public tags: string,
        public createdDate: Date,
        public displayCreatedDate: string
        
    ) {}
}
