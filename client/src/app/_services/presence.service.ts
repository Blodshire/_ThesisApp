import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSrc = new BehaviorSubject<string[]>([]);
  public onlineUsers$ = this.onlineUsersSrc.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }


  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', { accessTokenFactory: () => user.token }).withAutomaticReconnect().build();

    this.hubConnection.start().catch(
      error => console.log(error)
    );

    this.hubConnection.on('UserIsOnline', loginName => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: loginNames => this.onlineUsersSrc.next([...loginNames, loginName])
      })
    });

    this.hubConnection.on('UserIsOffline', loginName => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: loginNames => this.onlineUsersSrc.next(loginNames.filter(x=> x!==loginName))
      })
    });

    this.hubConnection.on('GetOnlineUsers', loginNames => {
      this.onlineUsersSrc.next(loginNames);
    });

    this.hubConnection.on('NewMessageReceived', ({ loginName, displayName }) => {
      this.toastr.info(displayName + ' üzenetet küldött! Kattins rám a megnyitáshoz.')
        .onTap.pipe(take(1)).subscribe({
          next: () => this.router.navigateByUrl('/members/' + loginName + '?tab=Üzenetek')
        })
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error))
  }
}
