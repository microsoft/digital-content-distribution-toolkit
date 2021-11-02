import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { OwlOptions } from 'ngx-owl-carousel-o';
import { RetailerCommissionDialogComponent } from '../retailer-commission-dialog/retailer-commission-dialog.component';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service'
import { EventType } from '@azure/msal-browser';
import { ContentProviderService } from 'src/app/services/content-provider.service';


@Component({
  selector: 'app-retailer-commissions',
  templateUrl: './retailer-commissions.component.html',
  styleUrls: ['./retailer-commissions.component.css']
})
export class RetailerCommissionsComponent implements OnInit, AfterViewInit, OnDestroy {
  carouselInit =false;
  customOptions: OwlOptions = {
    loop: true,
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
        items: 2
      }
    },
    nav: true
  }
  monthsDropDown: Array<any> = [];
  commissionsCarouselArr: Array<any> = [];
  monthSelect: any;
  partnerCode;
  retailerPartnerProvidedId;
  nestedCommissions = [];
  totalEarnings = 0;
  baseHref = this.retailerDashboardService.getBaseHref();
  contentProviders: any;
  constructor(
    public userService: UserService,
    public router: Router,
    public dialog: MatDialog,
    private retailerDashboardService: RetailerDashboardService,
    private contentProviderService: ContentProviderService
  ) { }

  ngOnInit(): void {
    this.getContentProviders();
    this.partnerCode = this.retailerDashboardService.getpartnerCode();
    this.retailerPartnerProvidedId = this.retailerDashboardService.getRetailerPartnerProvidedId();
    this.getDates();
    this.getCommissionsSliderInfo();
  }

  ngAfterViewInit() {
    console.log('setting routed to' + true);
    //this.userService.setRetailerRouted(true);
  }

  ngOnDestroy() {
    //this.userService.setRetailerRouted(false);
  }

  navigateToDashboard() {
    this.router.navigate(['/retailer/dashboard']);
  }

  handleCarouselEvents($event) {
    
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
      this.getCommissionsMonthly(this.partnerCode, this.retailerPartnerProvidedId, event.value)
    }
  }

  getCommissionsMonthly(partnerCode, retailerPartnerProvidedId, dateObj) {
    let occuredDates = [];
    let nestedCommissions = [];
    let totalEarnings = 0;
    var startDate = new Date(dateObj.firstDateString);
    var endDate = new Date(dateObj.lastDateString);
    endDate.setHours(23);
    endDate.setMinutes(59);
    endDate.setSeconds(59);
    var endDateUTCString  = endDate.toISOString();
    var startDateUTCString  = startDate.toISOString();
    this.retailerDashboardService.getReferralsCommissionsInDetail(partnerCode, retailerPartnerProvidedId, startDateUTCString, endDateUTCString, 'RETAILER_INCOME_ORDER_COMPLETED').subscribe(
      res => {
         res.forEach(transaction => {
           if(transaction.calculatedValue) {
            const occuredDate = transaction.eventOccuranceTime.split("T")[0];
            transaction.occuredDate = occuredDate;
            transaction.occuredTime = transaction.eventOccuranceTime.split("T")[1].split(".")[0];
            transaction.occuredTimeReadable = this.retailerDashboardService.generateReadableTime(transaction.eventOccuranceTime);
            transaction.properties.forEach(property => {
              if(property.name === 'OrderItem') {
                const orderItem = JSON.parse(property.value);
                transaction.name = orderItem.subscriptionTitle;
                transaction.transactionId = orderItem.transactionId;
                return;
              }
            });
            if(occuredDates.includes(occuredDate)) {
              const index = occuredDates.indexOf(occuredDate); 
              nestedCommissions[index].transactions.push(transaction); 
              nestedCommissions[index].amount+=transaction.calculatedValue;
              totalEarnings+=transaction.calculatedValue;
            } else {
              occuredDates.push(occuredDate);
              const index = occuredDates.indexOf(occuredDate); 
              nestedCommissions[index] = {};
              nestedCommissions[index].occuredDate = occuredDate;
              nestedCommissions[index].transactions = [];
              nestedCommissions[index].transactions.push(transaction);
              nestedCommissions[index].amount =  transaction.calculatedValue;
              totalEarnings+=transaction.calculatedValue;
              nestedCommissions[index].occuredDateReadable = this.retailerDashboardService.generateReadableDate(occuredDate);
            }
           }
        });
        this.nestedCommissions = nestedCommissions;
        this.totalEarnings = totalEarnings;
      },
      err => {
        this.nestedCommissions = [];
        this.totalEarnings = 0;
      }
    )
  }

  getContentProviders() {
    if(sessionStorage.getItem("CONTENT_PROVIDERS")) {
      this.contentProviders =  JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
      this.contentProviders.forEach(contentProvider => {
        this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
      }); 
    } else {
      this.contentProviderService.browseContentProviders().subscribe(res => {
        sessionStorage.setItem("CONTENT_PROVIDERS",  JSON.stringify(res));
        this.contentProviders = res;
        this.contentProviders.forEach(contentProvider => {
          this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
        }); 
      });
    }
  }

  getCommissionsSliderInfo() {
    let commissionsCarouselArr = [];
    this.retailerDashboardService.getReferralsCommissionsCarouselInfo(
      this.partnerCode, this.retailerPartnerProvidedId
      ).subscribe(
      res => {
        res.planDetails.forEach(planDetail => {
          if(planDetail.eventType === 'RETAILER_INCOME_ORDER_COMPLETED') {
            console.log(planDetail.formula.formulaType);
            if(planDetail.formula.formulaType === 'PERCENTAGE') {
              planDetail.earnText = 'Earn ' + planDetail.formula.firstOperand + '% for each ' + this.contentProviders[planDetail.eventSubType] + ' order';
            } else if(planDetail.formula.formulaType === 'PLUS') {
              planDetail.earnText = 'Earn Rs.' + planDetail.formula.firstOperand + ' for each ' + this.contentProviders[planDetail.eventSubType] + ' order';

            }
            planDetail.validTillText = res.endDate;
            commissionsCarouselArr.push(planDetail);
          }
        });
        this.commissionsCarouselArr = commissionsCarouselArr;
        this.carouselInit = true;
      },
      err => {

      }
    )
  }

  commissionDialog(commission, date) {
    commission.occuredDateReadable = date;
    const dialogRef = this.dialog.open(RetailerCommissionDialogComponent, {
      disableClose: false,
      data: commission,
      position: {
        bottom: '0'
      },
      // width: '100vw',
      panelClass: 'full-screen-modal'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'proceed') {

      }
    });
  }

}
