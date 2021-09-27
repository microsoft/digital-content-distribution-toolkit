import {
    HttpEvent,
    HttpInterceptor,
    HttpHandler,
    HttpRequest,
    HttpResponse,
    HttpErrorResponse
   } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { KaizalaService } from '../services/kaizala.service';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {

  urlsToNotUse: Array<string>;

    constructor(private kaizalaService: KaizalaService) {
      this.urlsToNotUse= [
        environment.kaizalaApi0,
        environment.kaizalaApi1,
        environment.kaizalaApi2
      ];
     }

    private isValidRequestForInterceptor(requestUrl: string): boolean {
      // let positionIndicator: string = 'api/';
      // let position = requestUrl.indexOf(positionIndicator);
      // if (position > 0) {
        // let destination: string = requestUrl.substr(position + positionIndicator.length);
        for (let address of this.urlsToNotUse) {
          if (new RegExp(address).test(requestUrl)) {
            return false;
          }
        // }
      }
      return true;
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      return next.handle(request)
        .pipe(
          retry(1),
          catchError((error: HttpErrorResponse) => {
            // if (this.isValidRequestForInterceptor(request.url) && [401, 403].indexOf(error.status) !== -1) {
            //   // auto logout if 401 Unauthorized or 403 Forbidden response returned from api
            //   this.kaizalaService.logout();
            //   location.reload(true);
            // }
            if (error.status === 401) {
              let errorMessage = "User is unauthorised";
              return throwError(errorMessage);
            }

            if (error.status === 403) {
              let errorMessage = "User is forbidden";
              return throwError(errorMessage);
            }

            let errorMessage = '';
            if (error.error instanceof ErrorEvent) {
              // client-side error
              errorMessage = `Error: ${error.error.message}`;
            } else {
              // server-side error
              var errMsg = "";
              if(Array.isArray(error.error)) {
                  error.error.forEach( err =>
                  errMsg = errMsg +  "\n" + err
                  );
              } else {
                errMsg = error.error.title ?  error.error.title : 
                (error.error.message ? error.error.message :"Something went wrong. Please try again!");
              }
              errorMessage = errMsg;
            }
            
            console.log(errorMessage);
            return throwError(errorMessage);
          })
        )
    }
}