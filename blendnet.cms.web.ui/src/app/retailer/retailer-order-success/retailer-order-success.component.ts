import { Component, OnInit, Input } from '@angular/core';
import { RetailerRequestService } from 'src/app/services/retailer/retailer-request-service.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-retailer-order-success',
  templateUrl: './retailer-order-success.component.html',
  styleUrls: ['./retailer-order-success.component.css']
})
export class RetailerOrderSuccessComponent implements OnInit {

  @Input() price;
  incentiveVal;
  getIncentiveError: Boolean
  baseHref = environment.baseHref;
  partnerCode = sessionStorage.getItem('partnerCode');
  retailerPartnerProvidedId = sessionStorage.getItem('partnerProvidedId');

  constructor(
    private retailerRequestService: RetailerRequestService
  ) { }

  ngOnInit(): void {
    this.getIncentiveError = false;
    this.getIncentiveAmount();
  }

  getIncentiveAmount() {
    // console.log('start showing page, ', this.price);

    this.retailerRequestService.getIncentivePlan(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
      res => {
        this.getIncentiveError = false;
        let planDetail = res.planDetails.find(obj => {
          return obj.eventType === "RETAILER_INCOME_ORDER_COMPLETED";
        });

        if(planDetail && planDetail.formula){
          // Will add more of formula type here
          if(planDetail.formula.formulaType === "PERCENTAGE") {
            let percentVal = planDetail.formula.firstOperand;
            this.incentiveVal = (this.price)*(percentVal/100);
          }
          else if(planDetail.formula.formulaType === 'MULTIPLY') {
            let multipleVal = planDetail.formula.firstOperand;
            this.incentiveVal = (this.price)*(multipleVal);
          }
        }
        
      },
      err => {
        console.log(err);
        this.getIncentiveError = true;
      }
    );
  }
}
