import { Component, OnInit } from '@angular/core';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service';

@Component({
  selector: 'app-no-transactions',
  templateUrl: './no-transactions.component.html',
  styleUrls: ['./no-transactions.component.css']
})
export class NoTransactionsComponent implements OnInit {

  constructor(
    private retailerDashboardService: RetailerDashboardService
  ) { }
  baseHref = this.retailerDashboardService.getBaseHref();
  ngOnInit(): void {
  }

}
