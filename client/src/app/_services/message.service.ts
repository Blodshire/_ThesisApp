import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from '../../environments/environment';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { BusyService } from './busy.service';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private msgThreadSrc = new BehaviorSubject<Message[]>([]);
  private counter = 0;//count how many messages to update when this is opened
  messageThread$ = this.msgThreadSrc.asObservable();
  loading: boolean = true;

  constructor(private http: HttpClient, private busyService: BusyService) {
  }

  createHubConnection(user: User, otherLoginName: string) {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'message?user=' + otherLoginName, {

      accessTokenFactory: () => user.token
    }).withAutomaticReconnect().build();

    this.hubConnection.start().catch(error => console.log(error))
      .finally(() => this.busyService.idle());

    this.hubConnection.on('ReceiveMessageThread', messages => {
      
      //very evil reversal hack part1
      messages.reverse();

      //------------- evil datetime conversion for non-chrome? browsers
      //for (let i = 0; i - this.counter; i++) {
      //  //var dateString = (messages[i].dateRead.toString());
      //  //dateString = dateString.replace(' ', 'T');
      //  //messages[i].dateRead = new Date(dateString);
      //}
      this.counter = 0;
      console.log(messages);
      this.msgThreadSrc.next(messages);
    })

    this.hubConnection.on('NewMessage', message => {
     
      //------------- evil datetime conversion for non-chrome? browsers
      if (message.dateRead) {
        //var dateStringRead = message.dateRead.toString();
        //dateStringRead = dateStringRead.replace(' ', 'T');
        //message.dateRead = new Date(dateStringRead);
        if(this.counter>0)
          this.counter--;
      }
      

      this.messageThread$.pipe(take(1)).subscribe({
        next: messages => {

          //------------- evil datetime conversion for non-chrome? browsers
          //var dateString = message.messageSent.toString();
          //dateString = dateString.replace(' ', 'T');
          //message.messageSent = new Date(dateString);


          //very evil reversal hack part2
          this.msgThreadSrc.next([message, ...messages,]);
        }
      })
    })
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.msgThreadSrc.next([]);
      this.hubConnection.stop();
    }
     
  }
  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResults<Message[]>(this.baseUrl + 'messages', params, this.http)
  }

  getMessageThread(loginName: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + loginName);
  }
  //force with async tag
  sendMessage(loginName: string, content: string) {
    this.counter++;
    console.log(this.counter + 'küldve');
    /* return this.http.post<Message>(this.baseUrl + 'messages', { recipientLoginName: loginName, content })*/
    return this.hubConnection?.invoke('SendMessage', { recipientLoginName: loginName, content }).catch(error => console.log(error));
  }
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
