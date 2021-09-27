import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-retailer-order-success',
  templateUrl: './retailer-order-success.component.html',
  styleUrls: ['./retailer-order-success.component.css']
})
export class RetailerOrderSuccessComponent implements OnInit {

  baseHref = environment.baseHref;

  constructor() { }

  ngOnInit(): void {
  }

}
