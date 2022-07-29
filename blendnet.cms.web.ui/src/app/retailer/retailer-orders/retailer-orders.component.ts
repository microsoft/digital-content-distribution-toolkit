import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RetailerRequestService } from 'src/app/services/retailer/retailer-request-service.service';
import { OrderStatus } from 'src/app/models/order-status.enum';
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
  createdOrders: any[] = [];
  partner: any = {};
  baseHref = environment.baseHref;
  orderImgUrl: string;
  noOrderImgUrl: string;

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
    const body = {
      "retailerPartnerProvidedId": this.retailerPartnerProvidedId,
      "retailerPartnerCode": this.partnerCode
    }
    this.createdOrders = [];
    this.requestPage = false;
    this.retailerRequestService.getOrders(this.userContact, body).subscribe(
      data => {
        this.createdOrders = data;
        this.emptyRequest = false;
        this.createdOrders?.forEach(o => {
          // o.orderImgUrl =  this.cdnBaseUrl + o.cpId + "-cdn/logos/pictorialmark_square.png";
          const img = new Image();
          var imgUrl = environment.cdnBaseUrl + o.cpId + environment.cpLogoPictorialImg;
      
          if (img.complete) {
           o.orderImgUrl = imgUrl;
          } else {
           img.onload = () => {
            o.orderImgUrl = imgUrl;
          };        
          img.onerror = () => {
             o.orderImgUrl  = "../../" + this.baseHref + environment.defaultCplogoImg;
           };
         }
        });
      },
      err => {
        this.emptyRequest = true;
        this.noOrderImgUrl = "../../"+ this.baseHref + environment.noOrdersImg;
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
