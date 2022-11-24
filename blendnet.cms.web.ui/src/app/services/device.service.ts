// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Device } from '../models/device.model';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  baseUrl = environment.baseUrl + environment.deviceUrl;
  deviceContentUrl = environment.baseUrl + environment.deviceContentUrl;
  constructor(private logger: LogService,
    private http: HttpClient) { }



    getDevices(): Observable<Device[]> {
      let url = this.baseUrl ;
      this.logger.log(`Fetching all devices`);
      return this.http.get<Device[]>(url);
    }

    createDevice(device): Observable<string>{ 
      let url = this.baseUrl ;
      this.logger.log(`Creating a device`);
      return this.http.post(url, device,{responseType: 'text'});
    }

    filterUpdate(deviceDate) {
      let url = this.baseUrl + '/filterupdate';
      this.logger.log(`Updating device filters`);
      return this.http.post(url, deviceDate);
    }

    getDeviceHistory(deviceId) {
      let url = this.baseUrl + '/' + deviceId +'/commands';
      this.logger.log(`Getting device history`);
      return this.http.get(url,{});
    }

    cancelCommand(deviceId, commandId) {
      let url = this.baseUrl + '/' + deviceId +'/cancelcommand/' + commandId;
      this.logger.log(`Cancelling command for device`);
      return this.http.post(url,{});
    }

    getContentsOnDeviceByCP(deviceId, contentProviderId) {
      let url = this.deviceContentUrl + '/' + deviceId +'/' + contentProviderId;
      this.logger.log(`Getting all the contents available for a device`);
      return this.http.post(url, {});    
    }


}
