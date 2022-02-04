import { Injectable } from '@angular/core';
import { Contentprovider, ContentProviderLtdInfo } from '../models/contentprovider.model';
import { LogService } from './log.service';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SasToken } from '../models/sas-token.model';

@Injectable({
  providedIn: 'root'
})
export class ContentProviderService {
  baseUrl = environment.baseUrl +  environment.contentProviderApiUrl;
  browseContentBaseUrl = environment.baseUrl +  environment.browrseContent;
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

  browseContentProviders() : Observable<ContentProviderLtdInfo[]>{
    let url = this.browseContentBaseUrl + '/contentproviders';
    this.logger.log(`Browsing the content providers`);
    return this.http.get<ContentProviderLtdInfo[]>(url);
  }

  createContentProvider(cp: Contentprovider) : Observable<string>{
    let url = this.baseUrl;
    this.logger.log(`Creating content providers`);
    return this.http.post<string>(url, cp);
  }

  editContentProvider(cp: Contentprovider) : Observable<Contentprovider> {
    let url = this.baseUrl + '/' + cp.id;
    this.logger.log(`Editing content providers`);
    return this.http.post<any>(url, cp);
  }

  // deleteContentProvider(cpId: string): Observable<HttpResponse<any>>  {
  //   let url = this.baseUrl + '/' + cpId;
  //   this.logger.log(`Deleting content providers`);
  //   return this.http.delete(url,{ observe: 'response'});
  // }

  changeDefaultCP(selectedCP: Contentprovider) {
    this.selectedCP.next(selectedCP)
  }
 
  generateSASKey(contentProviderId)  : Observable<SasToken> {
    let url = this.baseUrl + "/" + contentProviderId+ "/generateSaS";
    this.logger.log(`Fetching SAS key`);
    return this.http.get<SasToken>(url);
  }

  getBaseHref() {
    return environment.baseHref;
  }
}
