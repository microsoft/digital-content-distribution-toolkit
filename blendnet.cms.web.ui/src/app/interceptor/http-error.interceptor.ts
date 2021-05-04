import {
    HttpEvent,
    HttpInterceptor,
    HttpHandler,
    HttpRequest,
    HttpResponse,
    HttpErrorResponse
   } from '@angular/common/http';
   import { Observable, throwError } from 'rxjs';
   import { retry, catchError } from 'rxjs/operators';
   
   export class HttpErrorInterceptor implements HttpInterceptor {
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      return next.handle(request)
        .pipe(
          retry(1),
          catchError((error: HttpErrorResponse) => {
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