
export class Incentive {

    constructor(
        public id: string,
        public planName: string,
        public planType: PlanType,
        public startDate: Date,
        public endDate: Date,
        public audience: Audience,
        public planDetails: PlanDetails[],
        public publishMode: PublishMode

        
    ) {}
}

export enum PlanType {
        REGULAR = "REGULAR",
        MILESTONE = "MILESTONE"
}

export class PlanDetails {
    constructor(
        public detailId: string,
        public eventType: EventType,
        public eventSubType: string,
        public eventTitle: string,
        public ruleType: RuleType,
        public formula: Formula,
        public result: Result

    ) {}
}
export enum RuleType {
    SUM = "SUM",
    COUNT = "COUNT"
}
export class Formula {
    constructor(
        public formulaType: FormulaType,
        public firstOperand: number,
        public secondOperand: number,
        public rangeOperand: RangeValue[]
    ){}
}
export enum FormulaType{
    PLUS = "PLUS",
    MINUS = "MINUS",
    MULTIPLY = "MULTIPLY",
    PERCENTAGE = "PERCENTAGE",
    DIVIDE_AND_MULTIPLY = "DIVIDE_AND_MULTIPLY",
    RANGE = "RANGE"
}
export class RangeValue {
    constructor(
        public startRange: number,
        public endRange: number,
        public output: number
    ){}
}
export class Result {
    constructor(
        public value: number,
        public value1: number
    ) {}
}
export enum EventType {
    "Consumer First Sign-In" ="CONSUMER_INCOME_FIRST_SIGNIN",
    "Consumer App Open Once a Day"= "CONSUMER_INCOME_APP_ONCE_OPEN",
    "Consumer Order Complete" = "CONSUMER_INCOME_ORDER_COMPLETED",
    "Consumer Redeem Subscription" = "CONSUMER_EXPENSE_SUBSCRIPTION_REDEEM",
    "Retailer Order Complete"= "RETAILER_INCOME_ORDER_COMPLETED",
    "Retailer Referral"= "RETAILER_INCOME_REFERRAL_COMPLETED",
    "Retailer Download Complete" = "RETAILER_INCOME_DOWNLOAD_MEDIA_COMPLETED"
}



export enum PublishMode {
    DRAFT = "DRAFT",
    PUBLISHED = "PUBLISHED"
}

class Audience {
    constructor(
        public audienceType: AudienceType,
        public subTypeName: string
    ) {}
}

enum AudienceType {
    CONSUMER = "CONSUMER",
    RETAILER = "RETAILER"
}

export enum RetailerPartner {
    NOVO = "NOVO",
    TSTP = "TSTP"
}

export interface Contentproviders {
    [key: string]: String;
}