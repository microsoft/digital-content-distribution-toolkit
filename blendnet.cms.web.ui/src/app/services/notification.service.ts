import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Notification } from '../models/notification.model';
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

  getAllNotifications(): Observable<Notification[]>{
    let url = this.notificationBaseUrl + '/notifications';
    this.logger.log(`Fetching all notifications`);
    return this.http.get<any>(url)
      .pipe(map(response => {
       return response.data.map(notification => {
          return new Notification(notification.title, notification.body, notification.type, notification.attachmentUrl, notification.tags, notification.createdDate, null);
        });
      }));
  }



  sendBroadcast(notification)  {
    let url = this.notificationBaseUrl + '/sendbroadcast';
    this.logger.log(`Broadcasting new notification `);
    return this.http.post(url, notification);
  }

}