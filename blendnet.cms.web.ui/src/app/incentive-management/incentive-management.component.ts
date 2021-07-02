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

  statuses= [];
  selectedStatusRetailer;
  selectedStatusConsumer;
  selectedPlan = null;

  constructor( private incentiveService: IncentiveService,
    public dialog: MatDialog
    ) { }

  ngOnInit(): void {
    this.statuses = ['REGULAR', 'MILESTONE'];
    this.getRetailerIncentivePlans();

  }

  tabClick(event) {
    if(event.tab.textLabel === "RETAILER") {
      this.getRetailerIncentivePlans();
    } else if(event.tab.textLabel === "CONSUMER"){
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
          this.selectedStatusRetailer= this.statuses[0]; 
          this.applyFilterRetailer(null);           
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
        row.id = data.id;
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


  addIncentive(audience) {
    this.selectedPlan = null;
    if(audience === "RETAILER") {
      this.createRetailerIncentive = false;
      this.getRetailerIncentivePlans();
      this.showRetailerIncentive = true;
    } else {
      this.createConsumerIncentive = false;
      this.getConsumerIncentivePlans();
      this.showConsumerIncentive = true;
    }
  }

  getConsumerIncentivePlans() {
    this.incentiveService.getConsumerIncentives().subscribe(
      res => { 
          const validResult = res.filter((result) => Array.isArray(result));
          var mergedResult = [].concat.apply([], validResult);
          this.dataSourceConsumers = this.createDataSource(mergedResult);
          this.dataSourceConsumers.paginator = this.paginator.toArray()[1];;
          this.dataSourceConsumers.sort = this.sort.toArray()[1];   
          this.selectedStatusConsumer= this.statuses[0];     
          this.applyFilterConsumer(null);    
        },
      err => {
        this.dataSourceConsumers = this.createDataSource([]);
        // this.toastr.error(err);
        console.log('HTTP Error', err);
      }
    );
  }

  editIncentivePlan(row, audience) {
    this.selectedPlan = row;
    this.openAddIncentivePage(audience);
  }

  

  openAddIncentivePage(audience): void {
    if(audience === "RETAILER") {
      this.showRetailerIncentive = false;
      this.createRetailerIncentive = true;
    } else {
      this.showConsumerIncentive = false;
      this.createConsumerIncentive = true;
    }

  }

  showIncentivePage(audience): void {
    if(audience === "RETAILER") {
      this.showRetailerIncentive = true;
      this.createRetailerIncentive = false;
    } else {
      this.showConsumerIncentive = true;
      this.createConsumerIncentive = false;
    }
  }

  applyFilterRetailer(event) {
    var filterValue = "";
    if(event && event.target) {
      filterValue = event.target ? (event.target as HTMLInputElement).value : this.selectedStatusRetailer;
    } else {
      filterValue = event.value;
    }
    this.dataSourceRetailers.filter = filterValue.trim().toLowerCase();

    if (this.dataSourceRetailers.paginator) {
      this.dataSourceRetailers.paginator.firstPage();
    }
  }


  applyFilterConsumer(event) {
    var filterValue = "";
    if(event && event.target) {
       filterValue = event.target ? (event.target as HTMLInputElement).value : this.selectedStatusConsumer;
    } else {
      filterValue = event.value;
    }
    this.dataSourceConsumers.filter = filterValue.trim().toLowerCase();

    if (this.dataSourceConsumers.paginator) {
      this.dataSourceConsumers.paginator.firstPage();
    }
  }
  
}
