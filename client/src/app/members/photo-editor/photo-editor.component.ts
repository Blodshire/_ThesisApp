import { Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Member } from '../../_models/member';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from '../../_models/photo';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User | undefined

  constructor(private accountService: AccountService, private membersService: MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.user = user
        }
      }
    })
  }


  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {

    this.hasBaseDropZoneOver = e;
  }

  setMainPhoto(photo: Photo) {
    console.log("clicked on me");
    this.membersService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          this.user.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
          this.member.photoUrl = photo.url;
          this.member.photos.forEach(p => {
            if (p.isMain)
              p.isMain = false;
            if (p.id === photo.id)
              p.isMain = true;
          })
        }
      }
      })
  }

  deletePhoto(photoId: number) {
    this.membersService.deletePhoto(photoId).subscribe({
      next: next => {
        if (this.member) {
          this.member.photos = this.member.photos.filter(x => x.id !== photoId);
        }
      }
      })
  }

  initializeUploader() {

    this.uploader = new FileUploader({
      url: this.baseUrl + 'appusers/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024

    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials=false
    }
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response)
        this.member?.photos.push(photo);
      }
    }
  }
}
