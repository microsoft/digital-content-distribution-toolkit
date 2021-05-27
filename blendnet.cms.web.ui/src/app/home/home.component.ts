import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { KaizalaService } from '../services/kaizala.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  currentUser: any;
  userName: string = "";
  roles: string = "";
  hasNoAdminRoles: boolean = false;


  slides = [
    // {
    //   card : [
    //   {'image': 'https://cdn.bollywoodmdb.com/movies/largethumb/250x267/2019/hindi-medium-2/poster.jpg'}, 
    //   {'image': 'https://cdn.bollywoodmdb.com/movies/largethumb/250x267/2019/baaghi-3/baaghi-3-poster-7.jpg'},
    //   {'image': 'https://cdn.bollywoodmdb.com/movies/largethumb/250x267/2020/thappad/poster.jpg'},
    // //   ]
    // // },
    // // {
    // //   card : [
    //   {'image': 'https://cdn.bollywoodmdb.com/movies/largethumb/250x267/2019/bhoot-part-one-the-haunted-ship/poster.jpg'}, 
    //   {'image': 'https://cdn.bollywoodmdb.com/movies/largethumb/250x267/2020/shubh-mangal-zyada-saavdhan/poster.jpg'},
    //   {'image': 'https://cdn.bollywoodmdb.com/movies/largethumb/250x267/2020/thappad/poster.jpg'}
    //   ]
    // }
    {'image' : 'https://media-exp1.licdn.com/dms/image/C5112AQGu6QbC3PVCvQ/article-cover_image-shrink_600_2000/0/1565591910931?e=1626912000&v=beta&t=Rv4h2NgyAA0Zz-nQr0bcJZavvEHUbxSyLK06EXF6YoA'},
    {'image' : 'https://i.pinimg.com/originals/7d/ca/b0/7dcab0174ae242ad42284fa723ce00ea.jpg'},
    { 'image' : 'https://telegraphstar.com/wp-content/uploads/2020/03/Bwood-Films.jpg'}
  ];

  constructor(
    private kaizalaService: KaizalaService
  ) {
    this.kaizalaService.currentUser.subscribe(user => this.currentUser = user);
    this.userName = localStorage.getItem("currentUserName");
   }

  ngOnInit(): void {
    this.roles = localStorage.getItem("roles");
    this.hasNoAdminRoles = !(localStorage.getItem("roles").includes(environment.roles.SuperAdmin) ||
    localStorage.getItem("roles").includes(environment.roles.ContentAdmin));
  }

}
