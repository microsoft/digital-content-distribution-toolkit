import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { RetailerService } from '../services/retailer.service';

@Component({
  selector: 'app-admin-retailer-dashboard',
  templateUrl: './admin-retailer-dashboard.component.html',
  styleUrls: ['./admin-retailer-dashboard.component.css']
})
export class AdminRetailerDashboardComponent implements OnInit {

  partnerCode;
  partnerId;
  showDashboard: Boolean = false;

  constructor() { }

  ngOnInit(): void {
    this.partnerCode = new FormControl('', [Validators.required]);
    this.partnerId = new FormControl('', [Validators.required]);
  }

  getRetailerDashboard() {
    sessionStorage.setItem('partnerCode', this.partnerCode.value);
    sessionStorage.setItem('partnerProvidedId', this.partnerId.value);
    this.showDashboard = true;
  }


}
