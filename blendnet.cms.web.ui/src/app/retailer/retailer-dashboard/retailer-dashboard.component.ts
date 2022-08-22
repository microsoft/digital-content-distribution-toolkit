import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service'
import { OwlOptions } from 'ngx-owl-carousel-o';
import { ContentProviderService } from 'src/app/services/content-provider.service';


@Component({
  selector: 'app-retailer-dashboard',
  templateUrl: './retailer-dashboard.component.html',
  styleUrls: ['./retailer-dashboard.component.css']
})
export class RetailerDashboardComponent implements OnInit {
  constructor(
    private toastr: ToastrService,
    private contentProviderService: ContentProviderService,
    private retailerDashboardService: RetailerDashboardService
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
  milestonesDuration: Array<any>=[];
  monthSelect: any;
  milestoneSelect: any;
  partnerCode = sessionStorage.getItem('partnerCode');;
  retailerPartnerProvidedId = sessionStorage.getItem('partnerProvidedId');
  carouselInit = false;
  contentProviders: any;
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
        items: 3
      }
    },
    nav: true
  }

  ngOnInit(): void {
    this.getContentProviders();
    this.getProfile();
    this.getDates();
    this.getMilestoneDetails();
    //hardcoded remove once flow form novo sends us above data
    // this.getRetailerTotals();
  }

  
  getContentProviders() {
    if(sessionStorage.getItem("CONTENT_PROVIDERS_PUBLISHED")) {
      this.contentProviders =  JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS_PUBLISHED"));
      this.contentProviders.forEach(contentProvider => {
        this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
      }); 
    } else {
      this.contentProviderService.browseContentProviders().subscribe(
        res => {
        this.contentProviders = res;
        sessionStorage.setItem("CONTENT_PROVIDERS_PUBLISHED",  JSON.stringify(this.contentProviders));
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
    let referralTotal = 0;
    let commissionTotal = 0;
    let startDate = new Date(dateObj.firstDateString);
    let endDate = new Date(dateObj.lastDateString);
    endDate.setHours(23);
    endDate.setMinutes(59);
    endDate.setSeconds(59);
    let endDateUTCString  = endDate.toISOString();
    let startDateUTCString  = startDate.toISOString()
    this.retailerDashboardService.getAggregatedRetailerDetails(partnerCode, retailerPartnerProvidedId, startDateUTCString, endDateUTCString).subscribe(res => {
      if(res.eventAggregateResponses) {
        res.eventAggregateResponses.forEach(planDetail => {
          if(planDetail.eventType === 'RETAILER_INCOME_ORDER_COMPLETED') {
            if(planDetail.aggregratedCalculatedValue) {
              commissionTotal += planDetail.aggregratedCalculatedValue;
            }
          } else if(planDetail.eventType === 'RETAILER_INCOME_REFERRAL_COMPLETED') {
            if(planDetail.aggregratedCalculatedValue) {
              referralTotal += planDetail.aggregratedCalculatedValue;
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

  getMilestoneDetails(){
    let allMilestones: Array<any>=[];
    this.retailerDashboardService.getAllPublishedIncentivePlansByPlanType(this.retailerPartnerProvidedId, "milestone", this.partnerCode).subscribe( res => {
      res.forEach(milestone => {
        if(milestone.audience.audienceType=="RETAILER") {
            const firstDateString = new Date(milestone.startDate);
            const lastDateString = new Date(milestone.endDate); 
            allMilestones.push({
              id: milestone.id,
              name: milestone.planName,
              startDate: firstDateString,
              endDate: lastDateString,
              dateString: firstDateString.toDateString() + " - " + lastDateString.toDateString()});
        };
      });
      allMilestones.sort(function(a,b){return a.endDate - b.endDate});
      allMilestones.reverse();
      const currentDate = new Date();
      var currentActivePlan = false;
      //to check if there are any current active plans
      for(var i=0;i<allMilestones.length;i++){
        if((allMilestones[i].startDate.toDateString()<currentDate && allMilestones[i].endDate.toDateString()>currentDate) || allMilestones[i].startDate==currentDate || allMilestones[i].endDate==currentDate){
          currentActivePlan=true;
          for(var c=i;c<i+5 && c<allMilestones.length;c++){
            this.milestonesDuration.push({
              id: allMilestones[c].id,
              name: allMilestones[c].name,
              dateString: allMilestones[c].dateString});
            }
            break;
        }
      }
      if(!currentActivePlan){
        for(var c=0;c<5 && c<allMilestones.length;c++){
          this.milestonesDuration.push({
            id: allMilestones[c].id,
            name: allMilestones[c].name,
            dateString: allMilestones[c].dateString});
          }
      }
      let milestoneSelected = this.milestonesDuration[0];
        if(milestoneSelected!=null) {
          this.getMilestoneTotal(milestoneSelected);
          this.milestoneSelect = this.milestonesDuration[0];
        }
      this.carouselInit =true;
    } ,err => {
      console.log('error in milestone duration fetch');
      this.totalMilestoneEarnings = 0;
      this.carouselInit =true;
    });
  }
  
  milestoneSelected(event) {
    this.getMilestoneTotal(event.value);
  }

  getMilestoneTotal(milestone) {
    let totalMilestoneEarnings = 0;
    let milestonesCarouselArr = [];
    this.retailerDashboardService.getMileStonesPlanDetails(this.partnerCode,this.retailerPartnerProvidedId, milestone.id).subscribe( res => {  
      if(res.planDetails) {
        res.planDetails.forEach(planDetail => {
          if(planDetail.formula && planDetail.formula.formulaType === 'DIVIDE_AND_MULTIPLY') {
            planDetail.formulaName = planDetail.formula.formulaType;
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
                formulaType: planDetail.formulaName,
                ruleType: planDetail.ruleType,
                firstOperand: planDetail.formula.firstOperand,
                secondOperand: planDetail.formula.secondOperand,
                value : planDetail.result.value ? planDetail.result.value : 0,
                residualValue : planDetail.result.residualValue ? planDetail.result.residualValue : 0,
                progress: ((planDetail.result.residualValue ? planDetail.result.residualValue : 0))*100/planDetail.formula.firstOperand,
                eventType: planDetail.eventType,
                contentProviderId: planDetail.eventSubType
              });
            } 
          } else if(planDetail.formula && planDetail.formula.formulaType === 'RANGE') {
            planDetail.formulaName = planDetail.formula.formulaType;
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
            if(planDetail.result) {
              milestonesCarouselArr.push({
                formulaType: planDetail.formulaName,
                ruleType: planDetail.ruleType,
                value : planDetail.result.value ? planDetail.result.value : 0,
                eventType: planDetail.eventType,
                contentProviderId: planDetail.eventSubType
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
        sessionStorage.setItem('partnerCode', this.partner.partnerCode);
        sessionStorage.setItem('partnerProvidedId', this.partner.partnerProvidedId);
        this.retailerDashboardService.changeRetailerReferral(this.partner.referralCode) 
      },
      err => {

      }
    )
  } 
}
