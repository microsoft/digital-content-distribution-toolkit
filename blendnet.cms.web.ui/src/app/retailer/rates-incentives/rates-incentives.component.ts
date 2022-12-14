// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service';
import { EventType } from 'src/app/models/incentive.model';
import { ContentProviderService } from 'src/app/services/content-provider.service';
import { Location } from '@angular/common';
import { ContentProviderLtdInfo } from 'src/app/models/contentprovider.model';

@Component({
  selector: 'app-rates-incentives',
  templateUrl: './rates-incentives.component.html',
  styleUrls: ['./rates-incentives.component.css']
})
export class RatesIncentivesComponent implements OnInit, AfterViewInit, OnDestroy {
  ratesIncentivesReferrals = [];
  ratesIncentivesMilestones = [];
  ratesIncentivesCommissions = [];
  ratesIncentivesDownloads = [];
  partnerCode;
  retailerPartnerProvidedId;
  eventType = EventType;
  contentProviders: ContentProviderLtdInfo[];
  referralsValid;
  milestonesValid;
  incentivesValid;
  downloadsValid;
  baseHref = this.retailerDashboardService.getBaseHref();
  constructor(
    public userService: UserService,
    public router: Router,
    public location: Location,
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
    // console.log('setting routed to' + true);
    //this.userService.setRetailerRouted(true);
  }

  ngOnDestroy() {
    //this.userService.setRetailerRouted(false);
  }

  navigateToDashboard() {
    this.location.back();
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

  getRatesAndIncentives() {
        this.retailerDashboardService.getRegularRatesIncentives(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
          res => {
            if(res["planDetails"] && res["planDetails"].length) {
              this.referralsValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
              this.incentivesValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
              this.downloadsValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
              res["planDetails"].forEach(eachEvent => {
                if(eachEvent.eventType ===  EventType["Retailer Referral"]) {
                  this.ratesIncentivesReferrals.push(eachEvent);
                } else if(eachEvent.eventType ===  EventType["Retailer Order Complete"]) {
                  this.ratesIncentivesCommissions.push(eachEvent);
                } else if(eachEvent.eventType ===  EventType["Retailer Download Complete"]) {
                  this.ratesIncentivesDownloads.push(eachEvent);}
              });
            }
          },
          err => {
            console.log(err);
          }
        )
  
  }


}
