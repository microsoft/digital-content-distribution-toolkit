import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Incentive } from '../models/incentive.model';
import { IncentiveService } from '../services/incentive.service';

@Component({
  selector: 'app-incentive-management',
  templateUrl: './incentive-management.component.html',
  styleUrls: ['./incentive-management.component.css']
})
export class IncentiveManagementComponent implements OnInit {
  displayedColumnsRetailers: string[] = ['name', 'type', 'partner', 'startDate', 'endDate', 'status' , 'edit'];
  dataSourceRetailers: MatTableDataSource<Incentive>;

  dataSourceConsumers: MatTableDataSource<Incentive>;
  displayedColumnsConsumers: string[] = ['name', 'type', 'startDate', 'endDate', 'status' , 'edit'];


  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();

  showRetailerIncentive = true;
  createRetailerIncentive = false;
  showConsumerIncentive = true;
  createConsumerIncentive = false;

  constructor( private incentiveService: IncentiveService,
    public dialog: MatDialog
    ) { }

  ngOnInit(): void {
    this.getRetailerIncentivePlans();

  }
  tabClick(event) {
    if(event.tab.textLabel === "Retailer") {
      this.getRetailerIncentivePlans();
    } else if(event.tab.textLabel === "Consumer"){
      this.getConsumerIncentivePlans();
    }
  }
  getRetailerIncentivePlans() {
    this.incentiveService.getRetailerIncentives().subscribe(
      res => {
          const validResult = res.filter((result) => Array.isArray(result));
          var mergedResult = [].concat.apply([], validResult);
          this.dataSourceRetailers = this.createDataSource(mergedResult);
          this.dataSourceRetailers.paginator = this.paginator.toArray()[0];;
          this.dataSourceRetailers.sort = this.sort.toArray()[0];          
        },
      err => {
        this.dataSourceRetailers = this.createDataSource([]);
        // this.toastr.error(err);
        console.log('HTTP Error', err);
      }
    );
  }

  createDataSource(response) {
    var dataSource: Incentive[] =[];
    if(response) {
      response.forEach( data => {
        var row: any = {};
        row.name = data.planName;
        row.status = data.publishMode;
        row.type = data.planType;
        row.partner = data.audience.subTypeName;
        row.startDate = data.startDate;
        row.endDate =  data.endDate;
        dataSource.push(row);
      });
    }
    return new MatTableDataSource(dataSource);
  }


  getConsumerIncentivePlans() {
    this.incentiveService.getConsumerIncentives().subscribe(
      res => { 
          const validResult = res.filter((result) => Array.isArray(result));
          var mergedResult = [].concat.apply([], validResult);
          this.dataSourceConsumers = this.createDataSource(mergedResult);
          this.dataSourceConsumers.paginator = this.paginator.toArray()[1];;
          this.dataSourceConsumers.sort = this.sort.toArray()[1];         
        },
      err => {
        this.dataSourceConsumers = this.createDataSource([]);
        // this.toastr.error(err);
        console.log('HTTP Error', err);
      }
    );
  }

  

  openAddIncentivePage(audience): void {
    if(audience === "Retailer") {
      this.showRetailerIncentive = false;
      this.createRetailerIncentive = true;
    } else {
      this.showConsumerIncentive = false;
      this.createConsumerIncentive = true;
    }

  }

  showIncentivePage(audience): void {
    if(audience === "Retailer") {
      this.showRetailerIncentive = true;
      this.createRetailerIncentive = false;
    } else {
      this.showConsumerIncentive = true;
      this.createConsumerIncentive = false;
    }
  }

  applyFilterRetailer(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSourceRetailers.filter = filterValue.trim().toLowerCase();

    if (this.dataSourceRetailers.paginator) {
      this.dataSourceRetailers.paginator.firstPage();
    }
  }

  applyFilterConsumer(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSourceConsumers.filter = filterValue.trim().toLowerCase();

    if (this.dataSourceConsumers.paginator) {
      this.dataSourceConsumers.paginator.firstPage();
    }
  }
  
}
