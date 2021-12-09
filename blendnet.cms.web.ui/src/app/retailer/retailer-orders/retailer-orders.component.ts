import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RetailerRequestService } from 'src/app/services/retailer/retailer-request-service.service';
import { OrderStatus } from 'src/app/models/order-status.enum';
import { CreatedOrder } from 'src/app/models/created-order.model';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-retailer-orders',
  templateUrl: './retailer-orders.component.html',
  styleUrls: ['./retailer-orders.component.css']
})
export class RetailerOrdersComponent implements OnInit {

  orderAmount;
  userContact: string = "";
  requestPage: boolean = true;
  emptyRequest:boolean = false;
  orderCompleteRequestSuccess: boolean = false;
  createdOrders: CreatedOrder[] = [];
  partner: any = {};
  baseHref = environment.baseHref;

  partnerCode = sessionStorage.getItem('partnerCode');
  retailerPartnerProvidedId = sessionStorage.getItem('partnerProvidedId');

  constructor( 
    private location: Location,
    private retailerDashboardService: RetailerDashboardService,
    private retailerRequestService: RetailerRequestService
  ) { }

  ngOnInit(): void {
    this.getProfile();
  }

  getProfile() {
    this.retailerDashboardService.getProfile(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
      res => {
        this.partner = res;
        this.retailerDashboardService.changeRetailerReferral(this.partner.referralCode);
      },
      err => {
        console.log(err);
      }
    )
  } 
  

  goToPreviousPage() {
    this.requestPage = true;
  }

  getOrders(): void {
    const orderStatus = [OrderStatus.CREATED];
    this.createdOrders = [];
    this.requestPage = false;
    this.retailerRequestService.getOrders(this.userContact, orderStatus).subscribe(
      data => {
        this.createdOrders = data;
        console.log(this.createdOrders);
        this.emptyRequest = false;
      },
      err => {
        this.emptyRequest = true;
      }
    );
  }

  completeOrder(order): void {
    console.log(order);
    const index = this.createdOrders.findIndex(x => x.id == order.id);
    this.orderAmount = order.price;
    const payload = {
      orderId: order.id,
      userPhoneNumber: this.userContact,
      retailerPartnerProvidedId: this.retailerPartnerProvidedId,
      retailerPartnerCode: this.partnerCode,
      amountCollected: order.price,
      partnerReferenceNumber: this.retailerPartnerProvidedId,
      depositDate: new Date()
    }
    
    this.retailerRequestService.completeOrder(payload).subscribe(
      response => {
        console.log('order complete successful');
        this.createdOrders[index].processed = true;
        this.createdOrders[index].processedStatus = OrderStatus.SUCCESS;
        this.getOrders();
        this.showOrderCompleteSuccess();
      },
      err => {
        console.log('order complete failed: ', err);
        this.createdOrders[index].processed = true;
        this.orderCompleteRequestSuccess = false;
        this.createdOrders[index].processedStatus = OrderStatus.FAILED;
      }
    );
  }

  orderCompleteFailed(order) {
    return order.processedStatus === OrderStatus.FAILED;
  }

  showOrderCompleteSuccess() {
    this.orderCompleteRequestSuccess = true;
    setTimeout(() => {
      this.orderCompleteRequestSuccess = false;
    }, 5000);
  }
}
