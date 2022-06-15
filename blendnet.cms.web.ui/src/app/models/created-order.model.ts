import { OrderStatus } from "./order-status.enum";

export class CreatedOrder {
    id: string;
    title: string;
    price: number;
    days: number;
    processed: boolean;
    processedStatus: OrderStatus;
    cpId: string;

    constructor(id, title, price, days, cpId){
        this.id = id;
        this.title = title,
        this.price = price,
        this.days = days;
        this.processed = false;
        this.processedStatus = OrderStatus.CREATED,
        this.cpId = cpId
    }
}