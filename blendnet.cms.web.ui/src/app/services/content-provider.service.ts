import { Injectable } from '@angular/core';
import { Contentprovider } from '../models/contentprovider.model';
import { LogService } from './log.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class ContentProviderService {
  private cps: Contentprovider[] = [];

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  getContentProviders(){
    this.logger.log(`Fetching content providers`);
    return this.http.get("https://e593e821-3882-4e2b-b9cd-5d59fc200ed9.mock.pstmn.io/contentproviders");
  }

  createContentProvider(cp: Contentprovider) {
    this.logger.log(`Creating content providers`);
    return this.http.post("https://e593e821-3882-4e2b-b9cd-5d59fc200ed9.mock.pstmn.io/contentprovider", cp);
  }

  editContentProvider(cp: Contentprovider) {
    this.logger.log(`Updating content providers`);
    return this.http.put("https://e593e821-3882-4e2b-b9cd-5d59fc200ed9.mock.pstmn.io/contentprovider", cp);
  }

  deleteContentProvider(cpId: String) {
    this.logger.log(`Deleting content providers`);
    return this.http.delete("https://e593e821-3882-4e2b-b9cd-5d59fc200ed9.mock.pstmn.io/contentprovider?id="+cpId);
  }
}
