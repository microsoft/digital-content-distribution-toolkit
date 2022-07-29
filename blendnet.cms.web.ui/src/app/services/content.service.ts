import { HttpClient} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LogService } from './log.service';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Content } from '../models/content.model';
import { CommandDetail } from '../models/command-detail.model';
import { ContentView } from '../models/content-view.model';
import { map } from 'rxjs/operators';
import { title } from 'process';

@Injectable({
  providedIn: 'root'
})
export class ContentService {

  baseUrl = environment.baseUrl +  environment.contentApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }


  getContentByCpIdAndFilters(unprocessedContentFilters): Observable<ContentView[]> {
    let url = this.baseUrl + "/"+ sessionStorage.getItem("contentProviderId") + "/contentlist";
    this.logger.log(`Fetching content by contentprovider and filters`);
    return this.http.post<Content[]>(url, unprocessedContentFilters)
      .pipe(map(contents => {
        return contents.map(content => {
          return new ContentView(content.id, content.contentId, content.title, content.createdDate, content.modifiedDate, content.isActive, 
            content.contentBroadcastStatus, content.contentBroadcastStatusUpdatedBy,
            content.contentTransformStatus, content.contentTransformStatusUpdatedBy,
            content.contentUploadStatus, content.contentUploadStatusUpdatedBy,
            null, null, null, null , content.contentBroadcastedBy, content.dashUrl);
        });
      }));
  }
 
  uploadContent(formData){
    let url = this.baseUrl + "/"+ sessionStorage.getItem("contentProviderId") ;
    this.logger.log(`Uploading Content JSON`);
    return this.http.post(url, formData, 
      {reportProgress: true, observe: 'events'});
  }

  processContent(contendIds) {
    let url = this.baseUrl + "/transform";
    this.logger.log(`Processing`);
    return this.http.post(url, contendIds);
  }

  broadcastContent(broadcastContentRequest) {
    let url = this.baseUrl + "/broadcast";
    this.logger.log(`Broadcasting`);
    return this.http.post(url, broadcastContentRequest);
  }

  getContentToken(id) {
    let url = this.baseUrl + "/" + id + "/token";
    this.logger.log(`Fetching content token`);
    return this.http.get(url, {responseType: 'text'});
  }

  deleteContent(id) {
    let url = this.baseUrl + "/" + id ;
    this.logger.log(`Deleting the Content`);
    return this.http.delete(url);
  }

  cancelBroadcast(id): Observable<string> {
    let url = this.baseUrl + "/" + id  + "/cancelbroadcast";
    this.logger.log(`Cancelling the broadcast`);
    return this.http.post<string>(url, id);
  }

  getCommandDetails(contentId, commandId): Observable<CommandDetail>{
    let url = this.baseUrl + '/' +contentId  + '/command/' + commandId;
    this.logger.log(`Getting the details for the content `);
    return this.http.get<CommandDetail>(url);
  }

  changeContentActiveStatus(contentId, status) {
    let url = this.baseUrl + '/' + contentId  + '/changeactivestatus';
    var payload = {
      status: status
    }
    this.logger.log(`Changing the content active status`);
    return this.http.post(url, payload);
  }

  getContentById(id){
    let url = this.baseUrl + "/" + id;
    this.logger.log(`Fetching content by content id`);
    return this.http.get(url);
  }

  updateMetaData(id, content) {
    let url = this.baseUrl + "/" + id;
    this.logger.log(`Updating content metadata`);
    return this.http.put(url, content);
  }

}
