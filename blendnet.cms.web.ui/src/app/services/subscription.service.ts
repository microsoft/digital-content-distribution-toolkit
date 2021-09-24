import { Injectable } from '@angular/core';
import { LogService } from './log.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  baseUrl = environment.baseUrl + environment.contentProviderApiUrl;
  
  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  getSubscriptionsForCP() {
    let url = this.baseUrl + "/" + sessionStorage.getItem("contentProviderId") + "/Subscription";
    this.logger.log(`Fetching subscriptions for content providers`);
    return this.http.get(url, { observe: 'response'});
  }

  createSubscription(subscription) : Observable<HttpResponse<any>>{
    let url = this.baseUrl +  "/" + sessionStorage.getItem("contentProviderId") + "/Subscription";
    this.logger.log(`Creating subscription`);
    return this.http.post(url, subscription, { observe: 'response'});
  }

  editSubscription(subscription) : Observable<HttpResponse<any>> {
    let url = this.baseUrl + "/" + sessionStorage.getItem("contentProviderId") + 
    "/Subscription/" + subscription.id;
    this.logger.log(`Editing subscription`);
    return this.http.put(url, subscription, { observe: 'response'});
  }

  deleteSubscription(subscription): Observable<HttpResponse<any>>  {
    let url = this.baseUrl + "/" + sessionStorage.getItem("contentProviderId") + 
    "/Subscription/" + subscription.id;
    this.logger.log(`Deleting subscription`);
    return this.http.delete(url, { observe: 'response'});
  }



}
