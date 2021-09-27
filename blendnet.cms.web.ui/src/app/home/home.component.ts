import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { KaizalaService } from '../services/kaizala.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {


  constructor(
    private kaizalaService: KaizalaService,
    private router: Router
  ) {
    if(!this.kaizalaService.isLoggedIn()) {
      this.router.navigate(['/login']);
    }
   }

  ngOnInit(): void {

  }

}
