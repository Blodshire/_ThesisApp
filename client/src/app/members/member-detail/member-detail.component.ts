import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from '../../_models/member';
import { Message } from '../../_models/message';
import { MembersService } from '../../_services/members.service';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  currentTab?: TabDirective;
  member: Member ={} as Member;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  messages: Message[] = [];

  constructor(private membersService: MembersService, private route: ActivatedRoute, private messageService: MessageService) { }

  ngOnInit(): void {

    this.route.data.subscribe({
      next: data => this.member = data['member']
      })

    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.activateTab(params['tab']);
      }
      })
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Zoom,
        preview: false
      }
     
    ]
    this.galleryImages = this.getImages();
  }
  getImages() {
    if (!this.member) return [];
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url
      })
    }
    return imageUrls;
  }

  //loadMember() {
  //  const loginName = this.route.snapshot.paramMap.get('loginName');
  //  if (!loginName) return;
  //  this.membersService.getMember(loginName).subscribe({
  //    next: member => {
  //      this.member = member;

  //      console.log(this.getImages());
        
  //    }
  //  })
  //}

  activateTab(heading: string) {
    if (this.memberTabs) {
      var tempTab = this.memberTabs.tabs.find(x => x.heading === heading);
      if (typeof tempTab !== 'undefined') {
        tempTab.active = true;
      }

    }
  }
  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.loginName).subscribe({
        next: messages => {
          this.messages = messages;
          this.messages.reverse();
          console.log(this.messages);
        }
      })
    }
  }
  onTabActivation(data: TabDirective) {
    this.currentTab = data;
    if (this.currentTab.heading === 'Üzenetek') {
      this.loadMessages();
      
    }
  }
}
