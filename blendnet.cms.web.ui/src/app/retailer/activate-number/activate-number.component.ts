import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ActivateNumberService } from 'src/app/services/retailer/activate-number.service';

@Component({
  selector: 'app-activate-number',
  templateUrl: './activate-number.component.html',
  styleUrls: ['./activate-number.component.css']
})
export class ActivateNumberComponent implements OnInit {

  phoneNumber;
  error;
  limitReached: boolean;
  activateNumberRequestSuccess: boolean;

  constructor(
    private activateNumberService: ActivateNumberService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.phoneNumber = new FormControl('', [Validators.required, Validators.pattern(/^[0][1-9]\d{9}$|^[1-9]\d{9}$/)]);
    this.error = "";
    this.activateNumberRequestSuccess = false;
    this.limitReached = false;
  }

  activateNumberCalled() {
    return (this.activateNumberRequestSuccess || this.error);
  }

  getButtonText(){
    if(this.activateNumberCalled()) {
      return "Return To Home";
    }
    return "Activate";
  }

  activateNumber() {
    if(this.activateNumberCalled()){
      this.router.navigate(['/retailer/retailer-home']);
    }

    this.activateNumberService.activateNumber(this.phoneNumber.value).subscribe(
      res => {
        this.activateNumberRequestSuccess = true;
        this.error = false;
        this.limitReached = false;
      },
      err => {
        this.error = err;
      }
    )
  }
}