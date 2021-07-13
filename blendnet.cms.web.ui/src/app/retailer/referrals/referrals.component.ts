import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service'


@Component({
  selector: 'app-referrals',
  templateUrl: './referrals.component.html',
  styleUrls: ['./referrals.component.css']
})
export class ReferralsComponent implements OnInit, AfterViewInit, OnDestroy {
  selectedButton = 'today';
  today = new Date();
  sixMonthsAgo = new Date();
  monthsDropDown: Array<any> = [];
  milestonesCarouselArr: Array<any> = [];
  monthSelect: any;
  partnerCode = 'NOVO';
  retailerPartnerProvidedId = 'NVP';
  nestedReferrals = [];
  totalEarnings = 0;

  constructor(
    public userService: UserService,
    public router: Router,
    private retailerDashboardService: RetailerDashboardService
  ) { }

  ngOnInit(): void {
    this.getDates();
    this.sixMonthsAgo.setMonth(this.today.getMonth() - 6);
  }

  ngAfterViewInit() {
    console.log('setting routed to' + true);
    this.userService.setRetailerRouted(true);
  }

  ngOnDestroy() {
    this.userService.setRetailerRouted(false);
  }

  navigateToHome() {
    this.router.navigate(['retailer/home']);
  }

  selected(period: string) {
    console.log(period);
  }

  getReferralsMonthly(partnerCode, retailerPartnerProvidedId, dateObj) {
    let occuredDates = [];
    let nestedReferrals = [];
    let totalEarnings = 0;
    this.retailerDashboardService.getReferralsCommissionsInDetail(partnerCode, retailerPartnerProvidedId, dateObj.firstDateString, dateObj.lastDateString, 'RETAILER_INCOME_REFFRAL_COMPLETED').subscribe(
      res => {
         res.forEach(transaction => {
          console.log(transaction.eventOccuranceTime);
          const occuredDate = transaction.eventOccuranceTime.split("T")[0];
          transaction.occuredTime = transaction.eventOccuranceTime.split("T")[1].split(".")[0];
          transaction.properties.forEach(property => {
            if(property.name === 'UserPhone') {
              transaction.mobile = property.value;
              return;
            }
          });
          if(occuredDates.includes(occuredDate)) {
            const index = occuredDates.indexOf(occuredDate); 
            nestedReferrals[index].transactions.push(transaction); 
            nestedReferrals[index].amount+=transaction.calculatedValue;
            totalEarnings+=transaction.calculatedValue;
          } else {
            occuredDates.push(occuredDate);
            const index = occuredDates.indexOf(occuredDate); 
            nestedReferrals[index] = {};
            nestedReferrals[index].occuredDate = occuredDate;
            nestedReferrals[index].transactions = [];
            nestedReferrals[index].transactions.push(transaction);
            nestedReferrals[index].amount =  transaction.calculatedValue;
            totalEarnings+=transaction.calculatedValue;
          }
          console.log(occuredDate);
        });
        console.log(nestedReferrals);
        this.nestedReferrals = nestedReferrals;
        this.totalEarnings = totalEarnings;
      },
      err => {
        this.nestedReferrals = [];
        this.totalEarnings = 0;
      }
    )
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
      this.getReferralsMonthly(this.partnerCode, this.retailerPartnerProvidedId, event.value)
    }
  }
}
