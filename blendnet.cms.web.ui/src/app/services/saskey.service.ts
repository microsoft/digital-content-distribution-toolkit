import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class SaskeyService {

  baseUrl = environment.contentProviderApiUrl;
  
  constructor(
    private logger: LogService,
    private http: HttpClient) 
    { }

    generateSASKey(contentProviderId)  : Observable<HttpResponse<any>>{
      let url = this.baseUrl + "/" +contentProviderId+ "/generateSaS";
      this.logger.log(`Fetching SAS key`);
      return this.http.post(url, contentProviderId, { observe: 'response'});
    }
}


