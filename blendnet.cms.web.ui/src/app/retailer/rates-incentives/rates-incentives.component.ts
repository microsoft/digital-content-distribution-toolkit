import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service';
import { EventType } from 'src/app/models/incentive.model';
import { Contentproviders } from '../../models/incentive.model';
import { ContentProviderService } from 'src/app/services/content-provider.service';

@Component({
  selector: 'app-rates-incentives',
  templateUrl: './rates-incentives.component.html',
  styleUrls: ['./rates-incentives.component.css']
})
export class RatesIncentivesComponent implements OnInit, AfterViewInit, OnDestroy {
  ratesIncentivesReferrals = [];
  ratesIncentivesMilestones = [];
  ratesIncentivesCommissions = [];
  partnerCode;
  retailerPartnerProvidedId;
  eventType = EventType;
  contentProviders;
  referralsValid;
  milestonesValid;
  incentivesValid;
  baseHref = this.retailerDashboardService.getBaseHref();
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
    this.retailerDashboardService.getMilestoneRatesIncentives(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
      res => {
        this.milestonesValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
        if(res["planDetails"] && res["planDetails"].length) {
          this.ratesIncentivesMilestones = res["planDetails"];
        }
      },
      err => {
      });
    this.getRatesAndIncentives();

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

  getRatesAndIncentives() {

    // this.contentProviderService.browseContentProviders().subscribe(
    //   res => {
        this.retailerDashboardService.getRegularRatesIncentives(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
          res => {
            if(res["planDetails"] && res["planDetails"].length) {
              this.referralsValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
              this.incentivesValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
              res["planDetails"].forEach(eachEvent => {
                if(eachEvent.eventType ===  EventType["Retailer Referral"]) {
                  this.ratesIncentivesReferrals.push(eachEvent);
                } else if(eachEvent.eventType ===  EventType["Retailer Order Complete"]) {
                  this.ratesIncentivesCommissions.push(eachEvent);
                }
              });
            }
          },
          err => {
    
          }
        )
        // this.contentProviders = res;
        // this.contentProviders.forEach(contentProvider => {
        //   this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
        // }); 
    //   },
    //   err => console.log(err)
    // )
  
  }


}
