import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  notificationBaseUrl = environment.baseUrl +  environment.notificationApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { 


  }

  getAllNotifications(): Observable<any>{
    let url = this.notificationBaseUrl + '/notifications';
    this.logger.log(`Fetching all notifications`);
    return this.http.get(url);
  }



  sendBroadcast(notification)  {
    let url = this.notificationBaseUrl + '/sendbroadcast';
    this.logger.log(`Broadcasting new notification `);
    return this.http.post(url, notification);
  }

}