import { HttpClient, HttpResponse } from '@angular/common/http';
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


  getContentByCpIdAndFilters() {
    let url = this.baseUrl + "/"+ localStorage.getItem("contentProviderId") + "/getcontents";
    this.logger.log(`Fetching content by contentprovider and filters`);
    return this.http.get(url,  { observe: 'response'});
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
    this.logger.log(`Fetching content by contentprovider and filters`);
    return this.http.post(url, formData, 
      {reportProgress: true, observe: 'events'});
  }

}
