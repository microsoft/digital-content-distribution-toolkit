import { Injectable } from '@angular/core';
import { Contentprovider } from '../models/contentprovider.model';
import { LogService } from './log.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContentProviderService {
  baseUrl = environment.contentProviderApiUrl;
  browseContentBaseUrl = environment.browrseContent;
  private selectedCP = new BehaviorSubject<Contentprovider>(null);
  sharedSelectedCP$ = this.selectedCP.asObservable();
  
  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  getContentProviders() : Observable<Contentprovider[]>{
    let url = this.baseUrl;
    this.logger.log(`Fetching content providers`);
    return this.http.get<Contentprovider[]>(url);
  }

  browseContentProviders() {
    let url = this.browseContentBaseUrl + '/contentproviders';
    this.logger.log(`Browsing the content providers`);
    return this.http.get<Contentprovider[]>(url);
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



  changeDefaultCP(selectedCP: Contentprovider) {
    this.selectedCP.next(selectedCP)
  }
}
