import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-retailer-home',
  templateUrl: './retailer-home.component.html',
  styleUrls: ['./retailer-home.component.css']
})
export class RetailerHomeComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    public userService: UserService
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit() {
    console.log('setting routed to' + false);
    this.userService.setRetailerRouted(false);
  }

  ngOnDestroy() {
    this.userService.setRetailerRouted(true);
  }

}
