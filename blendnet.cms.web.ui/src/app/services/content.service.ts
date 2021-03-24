import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LogService } from './log.service';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContentService {

  
  baseUrl = environment.contentApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }


  getContentByCpIdAndFilters(): Observable<HttpResponse<any>> {
    let url = this.baseUrl + "/"+ localStorage.getItem("contentProviderId") + "/getcontents";
    this.logger.log(`Fetching content by contentprovider and filters`);
    return this.http.get(url,  { observe: 'response'});
  }

}
