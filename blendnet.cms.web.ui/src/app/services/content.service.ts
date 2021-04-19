import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LogService } from './log.service';
import { environment } from '../../environments/environment';
import { Observable, Subject, timer } from 'rxjs';
import { Content } from '../models/content.model';
import { switchMap, tap, share, retry, takeUntil } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ContentService {

  
  baseUrl = environment.contentApiUrl;
  // private unprocessedContent$: Observable<Content[]>;
  // private stopPolling = new Subject();


  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }


  getContentByCpIdAndFilters(unprocessedContentFilters) {
    let url = this.baseUrl + "/"+ localStorage.getItem("contentProviderId") + "/contentlist";
    this.logger.log(`Fetching content by contentprovider and filters`);
    return this.http.post(url, unprocessedContentFilters, { observe: 'response'});
  //   this.unprocessedContent$ = timer(1, 3000).pipe(
  //     switchMap(() => this.http.get<Content[]>(url)),
  //     retry(),
  //     share(),
  //     takeUntil(this.stopPolling)
  //  );
  }
  // getAllUnprocessedContent() : Observable<Content[]>{
  //   return this.unprocessedContent$;
  // }

  // ngOnDestroy() {
  //   this.stopPolling.next();
  // }

  uploadContent(formData){
    let url = this.baseUrl + "/"+ localStorage.getItem("contentProviderId") ;
    this.logger.log(`Uploading Content JSON`);
    return this.http.post(url, formData, 
      {reportProgress: true, observe: 'events'});
  }

  processContent(contendIds) {
    let url = this.baseUrl + "/transform";
    this.logger.log(`Processing`);
    return this.http.post(url, contendIds, { observe: 'response'});
  }

  boradcastContent(broadcastContentRequest) {
    let url = this.baseUrl + "/broadcast";
    this.logger.log(`Broadcasting`);
    return this.http.post(url, broadcastContentRequest, { observe: 'response'});
  }

  getContentToken(id) {
    let url = this.baseUrl + "/" + id + "/token";
    this.logger.log(`Fetching content token`);
    return this.http.get(url, {responseType: 'text'});
  }

}
