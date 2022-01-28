export class ExportDataReq {

    constructor(
        public name: string,
        public phonenumber: string,
        public createdDate: string,
        public modifiedDate: string,
        public displayCreatedDate: Date,
        public displayModifiedDate: string
        
    ) {}
}
