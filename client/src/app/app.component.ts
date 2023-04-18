import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'this is a test client you dUmMy';
  users: any; //This is "dangerous" persay
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.http.get('http://localhost:5014/api/appusers').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request completed! :)')
      });
    }
  
}
