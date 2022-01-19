import { Injectable } from '@angular/core';
import { LogService } from './log.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CPSubscription } from '../models/subscription.model';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  baseUrl = environment.baseUrl + environment.contentProviderApiUrl;
  
  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  getSubscriptionsForCP(): Observable<CPSubscription[]> {
    let url = this.baseUrl + "/" + sessionStorage.getItem("contentProviderId") + "/Subscription";
    this.logger.log(`Fetching subscriptions for content providers`);
    return this.http.get<CPSubscription[]>(url);
  }

  createSubscription(subscription) {
    let url = this.baseUrl +  "/" + sessionStorage.getItem("contentProviderId") + "/Subscription";
    this.logger.log(`Creating subscription`);
    return this.http.post(url, subscription);
  }

  editSubscription(subscription){
    let url = this.baseUrl + "/" + sessionStorage.getItem("contentProviderId") + 
    "/Subscription/" + subscription.id;
    this.logger.log(`Editing subscription`);
    return this.http.put(url, subscription);
  }

  deleteSubscription(id)  {
    let url = this.baseUrl + "/" + sessionStorage.getItem("contentProviderId") + 
    "/Subscription/" + id;
    this.logger.log(`Deleting subscription`);
    return this.http.delete(url);
  }


  updateEndDate(id, date)  {
    let url = this.baseUrl + "/" + sessionStorage.getItem("contentProviderId") +   "/Subscription/" + id + '/updateEndDate';
    this.logger.log(`Updating end date for a subscription`);
    return this.http.post(url, date);
  }


}
