import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  baseUrl = environment.baseUrl + environment.deviceUrl;
  constructor(private logger: LogService,
    private http: HttpClient) { }



    getDevices() {
      let url = this.baseUrl ;
      this.logger.log(`Fetching all devices`);
      return this.http.get(url, { observe: 'response'});
    }


    createDevice(device): Observable<HttpResponse<any>>{ 
      let url = this.baseUrl ;
      this.logger.log(`Creating a device`);
      return this.http.post(url, device,{ observe: 'response', responseType: 'text'});
    }

    filterUpdate(deviceDate) {
      let url = this.baseUrl + '/filterupdate';
      this.logger.log(`Updating device filters`);
      return this.http.post(url, deviceDate,{ observe: 'response'});
    }

    getDeviceHistory(deviceId) {
      let url = this.baseUrl + '/' + deviceId +'/commands';
      this.logger.log(`Getting device history`);
      return this.http.get(url,{ observe: 'response'});
    }

    assignDeviceToRetailer() {

    }
}
