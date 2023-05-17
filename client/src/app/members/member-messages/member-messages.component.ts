import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm
  @Input() loginName?: string;
  @Input() messages: Message[] = [];
  msgContent = '';


  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
  }

  sendMessage() {
    if (!this.loginName)
      return;
    this.messageService.sendMessage(this.loginName, this.msgContent).subscribe({
      next: message => {
        //------------- evil datetime conversion for non-chrome? browsers
        var dateString = message.messageSent.toString();
        dateString = dateString.replace(' ', 'T');
        message.messageSent = new Date(dateString);
        //-------------

        //very evil reversal hack
        this.messages.reverse();
        this.messages.push(message);
        this.messages.reverse();
        this.messageForm?.reset();
        
      }
    });
  }

}

