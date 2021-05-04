import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KaizalaService } from '../services/kaizala.service';


@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private kaizalaService: KaizalaService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // add authorization header with jwt token if available
        let currentUser = this.kaizalaService.currentUserValue;
        if (currentUser && currentUser.authenticationToken) {
            request = request.clone({
                setHeaders: { 
                    Authorization: `Bearer ${currentUser.authenticationToken}`
                }
            });
        }

        return next.handle(request);
    }
}