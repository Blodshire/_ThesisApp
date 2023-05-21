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
  loading = false;


  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
  }

  sendMessage() {
    if (!this.loginName)
      return;
    this.loading = true;
    this.messageService.sendMessage(this.loginName, this.msgContent)?.then(() =>
      this.messageForm?.reset()
    ).finally(() => this.loading = false);
   
  }

}

