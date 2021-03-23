import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class ContentService {

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  
}
