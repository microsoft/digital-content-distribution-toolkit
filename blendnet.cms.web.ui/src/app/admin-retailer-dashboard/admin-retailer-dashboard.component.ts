import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { RetailerService } from '../services/retailer.service';
import { RetailerDashboardService } from '../services/retailer/retailer-dashboard.service';

@Component({
  selector: 'app-admin-retailer-dashboard',
  templateUrl: './admin-retailer-dashboard.component.html',
  styleUrls: ['./admin-retailer-dashboard.component.css']
})
export class AdminRetailerDashboardComponent implements OnInit {

  partnerId;
  selectedRetailerPartner;
  error;
  partners = [];
  showDashboard: Boolean = false;

  constructor(
    private retailerService: RetailerService,
    private toastr: ToastrService,
    private retailerDashboardService: RetailerDashboardService) { }

  ngOnInit(): void {
    this.partnerId = new FormControl('', [Validators.required]);
    this.getRetailerPartners();
    if(sessionStorage.getItem('partnerCode') && sessionStorage.getItem('partnerProvidedId')) {
      this.partnerId.value = sessionStorage.getItem('partnerProvidedId');
      this.selectedRetailerPartner = sessionStorage.getItem('partnerCode');
      this.getRetailerDashboard();
    }
  }

  getRetailerDashboard() {
    sessionStorage.setItem('partnerCode', this.selectedRetailerPartner);
    sessionStorage.setItem('partnerProvidedId', this.partnerId.value);
    this.showDashboard = false;
    this.error = false;
    this.getProfile();
  }

  getProfile() {
    this.retailerDashboardService.getProfile(this.selectedRetailerPartner, this.partnerId.value).subscribe(
      res => {
        this.showDashboard = true;
      },
      err => {
        console.error(err);
        this.showDashboard = false;
        this.error = true;
      }
    )
  }

  getRetailerPartners() {
    this.retailerService.getRetailerPartners().subscribe(
      res => {
        this.partners = this.getPartnerCodes(res);
        sessionStorage.setItem("RETAILER_PARTNERS", JSON.stringify(this.partners));
        this.selectedRetailerPartner = this.partners[0];
      },
      err => this.toastr.error("Unable to load retailer Partner.")
    );
  }

  getPartnerCodes(partners) {
    var partnerCodes = [];
    if(partners && Array.isArray(partners)){
      partners.forEach(partner => {
        partnerCodes.push(partner.partnerCode);
      })
    }
    return partnerCodes;
  }

}
