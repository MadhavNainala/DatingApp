import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Dating App';
  users : any;
  constructor(private http: HttpClient){

  }

  ngOnInit() : void{
    this.http.get('http://localhost:5001/api/users').subscribe({
      next : response => this.users = response,
      error : error => console.log(error),
      complete : () => console.log('request completed')

    })
  }
}
