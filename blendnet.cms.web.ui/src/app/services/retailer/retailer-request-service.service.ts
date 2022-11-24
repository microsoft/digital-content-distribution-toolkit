// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { CreatedOrder } from 'src/app/models/created-order.model';
import { environment } from 'src/environments/environment';
import { LogService } from '../log.service';

@Injectable({
  providedIn: 'root'
})
export class RetailerRequestService {

  baseUrl:string = environment.baseUrl+environment.omsApiUrl;
  incentiveBrowseUrl: string = environment.baseUrl+environment.incentiveBrowseApiUrl;
  retailerUrl = environment.baseUrl +  environment.retailerApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  

  getOrders(phoneNumber: string, payload): Observable<any> {
    let url = `${this.baseUrl}/Order/${phoneNumber}/orderlist`;
    this.logger.log('Fetching created orders');
    return this.http.post(url, payload).pipe(
      map((response: any) => {
        return response.map(item => {
          const subs = item.order.orderItems[0].subscription;
          const createdDate = new Date(item.order.orderCreatedDate);
          const now = new Date();
          const diffTime = Math.abs(now.getTime() - createdDate.getTime());
          const diffDays = Math.ceil(diffTime / (1000*60*60*24));
          return new CreatedOrder(item.order.id, subs.title, subs.price, diffDays, subs.contentProviderId);
        })
      })
    )
  }

  completeOrder(payload): Observable<any> {
    let url = `${this.baseUrl}/Order/completeorder`;
    this.logger.log('Completing order');
    return this.http.put(url, payload);
  }

  getIncentivePlan(partnerCode: string, retailerPartnerProvidedId: string): Observable<any> {
    let url = `${this.incentiveBrowseUrl}/retailer/active/${retailerPartnerProvidedId}/REGULAR/${partnerCode}`;
    this.logger.log('Getting incentive plan for order complete');
    return this.http.get(url);
  }

  getConfig(partnerCode: string): Observable<any> {
    let url = `${this.retailerUrl}/RetailerProvider/byPartnerCode/${partnerCode}`;
    this.logger.log('Getting config file');
    return this.http.get(url);
  }

}