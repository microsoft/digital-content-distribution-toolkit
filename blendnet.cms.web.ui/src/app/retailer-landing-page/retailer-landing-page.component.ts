import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service'
import { OwlOptions } from 'ngx-owl-carousel-o';
import { EventType, Contentproviders } from '../models/incentive.model';
import { ContentProviderService } from 'src/app/services/content-provider.service';
import { ContentProviderLtdInfo } from '../models/contentprovider.model';


@Component({
  selector: 'app-retailer-landing-page',
  templateUrl: './retailer-landing-page.component.html',
  styleUrls: ['./retailer-landing-page.component.css']
})
export class RetailerLandingPageComponent implements OnInit {
  constructor(
    private toastr: ToastrService,
    private retailerDashboardService: RetailerDashboardService,
    private contentProviderService: ContentProviderService
  ) { }
  baseHref = this.retailerDashboardService.getBaseHref();
  partner: any = {
    userName: ''
  };
  referralTotal = 0;
  commissionsTotal = 0;
  totalMilestoneEarnings = 0
  monthsDropDown: Array<any> = [];
  milestonesCarouselArr: Array<any> = [];
  monthSelect: any;
  partnerCode: string;
  retailerPartnerProvidedId: string;
  carouselInit = false;
  contentProviders: ContentProviderLtdInfo[];
  customOptions: OwlOptions = {
    loop: false,
    mouseDrag: true,
    touchDrag: true,
    pullDrag: true,
    dots: false,
    navSpeed: 700,
    navText: ['', ''],
    responsive: {
      0: {
        items: 1
      },
      400: {
        items: 1
      },
      740: {
        items: 2
      },
      940: {
        items: 4
      }
    },
    nav: true
  }

  ngOnInit(): void {
    this.getContentProviders();
    this.partnerCode = this.retailerDashboardService.getpartnerCode();
    this.retailerPartnerProvidedId = this.retailerDashboardService.getRetailerPartnerProvidedId();;
    this.getProfile();
    this.getDates();
    this.getMilestoneTotal();
   
    //hardcoded remove once flow form novo sends us above data
    // this.getRetailerTotals();
  }

 

  getContentProviders() {
    if(sessionStorage.getItem("CONTENT_PROVIDERS")) {
      this.contentProviders =  JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
      this.contentProviders.forEach(contentProvider => {
        this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
      }); 
    } else {
      this.contentProviderService.browseContentProviders().subscribe(res => {
        this.contentProviders = res;
        sessionStorage.setItem("CONTENT_PROVIDERS",  JSON.stringify(res));
        this.contentProviders.forEach(contentProvider => {
          this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
        }); 
      });
    }
  }

  copiedConfirm() {
    this.toastr.success("Referral code is copied!");
  }

  getRetailerTotals(partnerCode, retailerPartnerProvidedId, dateObj) {
    var startDate = new Date(dateObj.firstDateString);
    var endDate = new Date(dateObj.lastDateString);
    endDate.setHours(endDate.getHours() + 23);
    endDate.setMinutes(endDate.getMinutes() + 59);
    endDate.setSeconds(endDate.getSeconds() + 59);
    var endDateUTCString  = endDate.toISOString();
    var startDateUTCString  = startDate.toISOString()
    let referralTotal = 0;
    let commissionTotal = 0;
    this.retailerDashboardService.getAggregatedRetailerDetails(partnerCode, retailerPartnerProvidedId, startDateUTCString ,endDateUTCString).subscribe(res => {
      if(res.eventAggregateResponses) {
        res.eventAggregateResponses.forEach(planDetail => {
          if(planDetail.eventType === 'RETAILER_INCOME_ORDER_COMPLETED') {
            if(planDetail.aggregratedValue) {
              commissionTotal += planDetail.aggregratedValue;
            }
          } else if(planDetail.eventType === 'RETAILER_INCOME_REFERRAL_COMPLETED') {
            if(planDetail.aggregratedValue) {
              referralTotal += planDetail.aggregratedValue;
            }
          }
        });
      }
      this.referralTotal = referralTotal;
      this.commissionsTotal = commissionTotal;
    },
  err => {
    //  this.toastr.error(err);
    this.referralTotal = referralTotal;
    this.commissionsTotal = commissionTotal;
  })
  }

  getMilestoneTotal() {
    let totalMilestoneEarnings = 0;
    let milestonesCarouselArr = []
    this.retailerDashboardService.getMileStonesHome(this.partnerCode, this.retailerPartnerProvidedId).subscribe( res => {
      
      if(res.planDetails) {
        res.planDetails.forEach(planDetail => {
          if(planDetail.formula && planDetail.formula.formulaType === 'DIVIDE_AND_MULTIPLY') {
            if(planDetail.result && planDetail.result.value) {
              const value = planDetail.result.value;
              totalMilestoneEarnings+=value;
            }
            if(!planDetail.result) {
              planDetail.result = {
                residualValue: 0,
                value: 0
              }
            }
            if(planDetail.formula.firstOperand && planDetail.formula.secondOperand && planDetail.result) {
              milestonesCarouselArr.push({
                ruleType: planDetail.ruleType,
                firstOperand: planDetail.formula.firstOperand,
                secondOperand: planDetail.formula.secondOperand,
                value : planDetail.result.value ? planDetail.result.value : 0,
                residualValue : planDetail.result.residualValue ? planDetail.result.residualValue : 0,
                progress: ((planDetail.result.residualValue ? planDetail.result.residualValue : 0))*100/planDetail.formula.firstOperand,
                referral: planDetail.eventType === EventType['Retailer Referral'],
                contentProviderId: planDetail.eventSubType
                // eventType: planDetail.eventType
              });
            } 
          }
        });
        this.totalMilestoneEarnings = totalMilestoneEarnings;
        this.milestonesCarouselArr = milestonesCarouselArr;
        this.carouselInit =true;
      }
    } ,err => {
      console.log('error in milestone fetch');
      this.totalMilestoneEarnings = totalMilestoneEarnings;
      this.carouselInit =true;
    });
  }

  getDates = () => {
    for(let i=0; i<6;i++) {
      const date = new Date();
      // con
      const firstDay = new Date(date.getFullYear(), date.getMonth()-i, 1);
      const lastDay = new Date(date.getFullYear(), date.getMonth()-i+1, 0);
      const monthName = firstDay.toLocaleString('default', { month: 'long' });
      const firstDateString = firstDay.getFullYear() + '-' + (Number(firstDay.getMonth())+1) + '-' + firstDay.getDate();
      const lastDateString = lastDay.getFullYear() + '-' + (Number(lastDay.getMonth())+1)  + '-' + lastDay.getDate(); 
      this.monthsDropDown.push({
        firstDateString: firstDateString,
        lastDateString: lastDateString,
        monthName: monthName
      })
    }
    let monthSelected =  this.retailerDashboardService.getMonthSelected() ? this.retailerDashboardService.getMonthSelected():  this.monthsDropDown[0];
    this.monthsDropDown.forEach( (month, index) => {
      if(month.monthName === monthSelected.monthName) {
        this.monthSelected({value: month});
        this.monthSelect = this.monthsDropDown[index];
      }
    })
  }

  monthSelected(event) {
    this.retailerDashboardService.setMonthSelected(event.value);
    if(event.value && event.value.firstDateString && event.value.lastDateString) {
      this.getRetailerTotals(this.partnerCode, this.retailerPartnerProvidedId, event.value);
    }
  }

  getProfile() {
    this.retailerDashboardService.getProfile(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
      res => {
        this.partner = res; 
      },
      err => {

      }
    )
  } 
}
