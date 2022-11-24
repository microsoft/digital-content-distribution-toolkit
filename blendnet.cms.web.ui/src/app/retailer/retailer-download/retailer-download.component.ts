// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { OwlOptions } from 'ngx-owl-carousel-o';
import { MatDialog } from '@angular/material/dialog';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service'
import { ContentProviderService } from 'src/app/services/content-provider.service';
import { Location } from '@angular/common';
import { ContentProviderLtdInfo } from 'src/app/models/contentprovider.model';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-retailer-download',
  templateUrl: './retailer-download.component.html',
  styleUrls: ['./retailer-download.component.css']
})
export class RetailerDownloadComponent implements OnInit {

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
  downloadsCarouselArr: Array<any> = [];
  monthSelect: any;
  partnerCode;
  retailerPartnerProvidedId;
  nestedDownloads = [];
  totalEarnings = 0;
  baseHref = this.retailerDashboardService.getBaseHref();
  contentProviders: ContentProviderLtdInfo[];
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
    this.getDates();
    this.getDownloadsSliderInfo();
  }

  navigateToDashboard() {
    this.location.back();
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
      this.getDownloadsMonthly(this.partnerCode, this.retailerPartnerProvidedId, event.value)
    }
  }

  getDownloadsMonthly(partnerCode, retailerPartnerProvidedId, dateObj) {
    let occuredDates = [];
    let nestedDownloads = [];
    let totalEarnings = 0;
    var startDate = new Date(dateObj.firstDateString);
    var endDate = new Date(dateObj.lastDateString);
    endDate.setHours(23);
    endDate.setMinutes(59);
    endDate.setSeconds(59);
    var endDateUTCString  = endDate.toISOString();
    var startDateUTCString  = startDate.toISOString();
    this.retailerDashboardService.getReferralsCommissionsInDetail(partnerCode, retailerPartnerProvidedId, startDateUTCString, endDateUTCString, 'RETAILER_INCOME_DOWNLOAD_MEDIA_COMPLETED').subscribe(
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
              nestedDownloads[index].transactions.push(transaction); 
              nestedDownloads[index].amount+=transaction.calculatedValue;
              totalEarnings+=transaction.calculatedValue;
            } else {
              occuredDates.push(occuredDate);
              const index = occuredDates.indexOf(occuredDate); 
              nestedDownloads[index] = {};
              nestedDownloads[index].occuredDate = occuredDate;
              nestedDownloads[index].transactions = [];
              nestedDownloads[index].transactions.push(transaction);
              nestedDownloads[index].amount =  transaction.calculatedValue;
              totalEarnings+=transaction.calculatedValue;
              nestedDownloads[index].occuredDateReadable = this.retailerDashboardService.generateReadableDate(occuredDate);
            }
           }
        });
        this.nestedDownloads = nestedDownloads;
        this.totalEarnings = totalEarnings;
      },
      err => {
        this.nestedDownloads = [];
        this.totalEarnings = 0;
        console.error(err);
      }
    )
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

  getDownloadsSliderInfo() {
    let downloadsCarouselArr = [];
    this.retailerDashboardService.getReferralsCommissionsCarouselInfo(
      this.partnerCode, this.retailerPartnerProvidedId
      ).subscribe(
      res => {
        res.planDetails.forEach(planDetail => {
          if(planDetail.eventType === 'RETAILER_INCOME_DOWNLOAD_MEDIA_COMPLETED') {
            if(planDetail.formula.formulaType === 'PERCENTAGE') {
              planDetail.earnText = 'Earn ' + planDetail.formula.firstOperand + '% for each' + ' download';
            } else if(planDetail.formula.formulaType === 'PLUS') {
              planDetail.earnText = 'Earn Rs.' + planDetail.formula.firstOperand + ' for each' + ' download';
            }
            
            planDetail.validTillText = this.retailerDashboardService.formatDateOnlyDay(res.endDate);
            planDetail.logoUrl = environment.cdnBaseUrl + planDetail.eventSubType + environment.cpLogoWaterMarkImg;

            downloadsCarouselArr.push(planDetail);
          }
        });
        this.downloadsCarouselArr = downloadsCarouselArr;
        this.carouselInit = true;
      },
      err => {

      }
    )
  }
}


