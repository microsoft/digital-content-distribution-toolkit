import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-rates-incentives',
  templateUrl: './rates-incentives.component.html',
  styleUrls: ['./rates-incentives.component.css']
})
export class RatesIncentivesComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    public userService: UserService,
    public router: Router,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
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

}
