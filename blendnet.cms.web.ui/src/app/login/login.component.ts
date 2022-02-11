import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { KaizalaService } from '../services/kaizala.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor( private router: Router, private kaizalaService: KaizalaService, private userService: UserService) { 
    if (this.kaizalaService.loggedInValue && this.userService.registeredUserValue) { 
      this.router.navigate(['/home']);
    }

  }

  ngOnInit(): void {
  }


  navigate(role) {
    if(role === 'admin') {
      this.router.navigate(['/admin-login']);
    } else if (role === 'retailer') {
      this.router.navigate(['/common-retailer-login']);
    }
  }

}
