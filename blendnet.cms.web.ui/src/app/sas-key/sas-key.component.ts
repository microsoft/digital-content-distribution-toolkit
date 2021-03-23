import { Component, OnInit } from '@angular/core';
import { SaskeyService } from '../services/saskey.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-sas-key',
  templateUrl: './sas-key.component.html',
  styleUrls: ['./sas-key.component.css']
})
export class SasKeyComponent implements OnInit {
  sasKey:String ="";
  expiresIn:String= "";

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
    var cpId = localStorage.getItem("contentProviderId");
    this.sasKeyService.generateSASKey(cpId).subscribe(res => {
      if(res.status === 200) {
        this.sasKey = res.body.sasUri;
        var today = new Date();
        today.setHours(today.getHours() + res.body.expiryInHours);
        this.expiresIn = today + "";
        this.showSuccess("SAS Generated sucessfully");
      } else {
        this.showError("SAS generation failed !")
      }
    });
  }

  showSuccess(message) {
    this.toastr.success(message);
  }

  showError(message) {
    this.toastr.error(message);
  }
}
