import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ContentProviderService } from '../services/content-provider.service';

@Component({
  selector: 'app-sas-key',
  templateUrl: './sas-key.component.html',
  styleUrls: ['./sas-key.component.css']
})
export class SasKeyComponent implements OnInit {
  sasKey:string ="";
  expiresIn:string= "";

  constructor( 
    private contentProviderService: ContentProviderService,
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
      this.contentProviderService.generateSASKey(cpId)
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
