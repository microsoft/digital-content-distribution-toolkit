import { HttpClient} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LogService } from './log.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContentService {

  baseUrl = environment.baseUrl +  environment.contentApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }


  getContentByCpIdAndFilters(unprocessedContentFilters) {
    let url = this.baseUrl + "/"+ sessionStorage.getItem("contentProviderId") + "/contentlist";
    this.logger.log(`Fetching content by contentprovider and filters`);
    return this.http.post(url, unprocessedContentFilters, { observe: 'response'});
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
    return this.http.post(url, contendIds, { observe: 'response'});
  }

  broadcastContent(broadcastContentRequest) {
    let url = this.baseUrl + "/broadcast";
    this.logger.log(`Broadcasting`);
    return this.http.post(url, broadcastContentRequest, { observe: 'response'});
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

  cancelBroadcast(id) {
    let url = this.baseUrl + "/" + id  + "/cancelbroadcast";
    this.logger.log(`Cancelling the broadcast`);
    return this.http.post(url, id);
  }

  getContentDetails(contentId, commandId) {
    let url = this.baseUrl + '/' +contentId  + '/command/' + commandId;
    this.logger.log(`Getting the details for the content `);
    return this.http.get(url);
  }

  changeContentActiveStatus(contentId, status) {
    let url = this.baseUrl + '/' + contentId  + '/changeactivestatus';
    var payload = {
      status: status
    }
    this.logger.log(`Changing the content active status`);
    return this.http.post(url, payload);
  }

}
