import { Injectable } from '@angular/core';
import { Contentprovider } from '../models/contentprovider.model';
import { LogService } from './log.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContentProviderService {
  baseUrl = environment.contentProviderApiUrl;
  defaultCP: Contentprovider = null;
  private data = new BehaviorSubject(this.defaultCP);
  data$ = this.data.asObservable();
  
  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  getContentProviders(){
    let url = this.baseUrl;
    this.logger.log(`Fetching content providers`);
    return this.http.get(url);
  }

  createContentProvider(cp: Contentprovider) : Observable<HttpResponse<any>>{
    let url = this.baseUrl;
    this.logger.log(`Creating content providers`);
    return this.http.post(url, cp, { observe: 'response'});
  }

  editContentProvider(cp: Contentprovider) : Observable<HttpResponse<any>> {
    let url = this.baseUrl + '/' +cp.id;
    this.logger.log(`Editing content providers`);
    return this.http.post(url, cp, { observe: 'response'});
  }

  deleteContentProvider(cpId: string): Observable<HttpResponse<any>>  {
    let url = this.baseUrl + '/' + cpId;
    this.logger.log(`Deleting content providers`);
    return this.http.delete(url,{ observe: 'response'});
  }



  changeDefaultCP(data: Contentprovider) {
    this.data.next(data)
  }
}
