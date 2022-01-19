
export class CPSubscription {

    constructor(
        public id: string,
        public type: string,
        public durationDays: number,
        public startDate: Date,
        public endDate: Date,
        public displayStartDate: Date,
        public displayEndDate: Date,
        public isReedemable: Boolean,
        public redemptionValue: number
    ){}

}
