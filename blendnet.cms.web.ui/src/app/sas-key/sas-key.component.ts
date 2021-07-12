import { Component, OnInit } from '@angular/core';
import { SaskeyService } from '../services/saskey.service';
import { ToastrService } from 'ngx-toastr';
import { catchError, map } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-sas-key',
  templateUrl: './sas-key.component.html',
  styleUrls: ['./sas-key.component.css']
})
export class SasKeyComponent implements OnInit {
  sasKey:string ="";
  expiresIn:string= "";

  constructor( 
    private sasKeyService: SaskeyService,
    private toastr: ToastrService
    ) { }

  ngOnInit(): void {
  }


  clearSAS() {
    this.sasKey = "";
    this.expiresIn = null;
  }

  generateSAS() {
    var cpId = sessionStorage.getItem("contentProviderId");
    if(cpId) {
      this.sasKeyService.generateSASKey(cpId)
      .subscribe( res => {
        this.sasKey  = res.sasUri;
        var today = new Date();
        today.setMinutes(today.getMinutes() + res.expiryInMinutes);
        this.expiresIn = today + "";
        this.showSuccess("SAS Uri Generated sucessfully");
      },
      error => {
        this.toastr.error(error); 
      });
    } else {
      this.toastr.error("Please select a content provider to generate the SAS"); 
    }
    
  }

  showSuccess(message) {
    this.toastr.success(message);
  }

  showError(message) {
    this.toastr.error(message);
  }
}
