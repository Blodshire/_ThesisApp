import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from '../../_models/member';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any): boolean | undefined {
    if (this.editForm?.dirty) {
      return false;
    }
    return true;
  }
  member: Member | undefined;
  user: User | null = null;

  constructor(private accountService: AccountService, private membersService: MembersService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
      })
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    if (!this.user) return;
    this.membersService.getMember(this.user.loginName).subscribe({
      next: member => this.member = member
      })
  }

  updateMember() {
    this.membersService.updateMember(this.editForm?.value).subscribe({
      next: next => {
        this.toastr.success('Változtatások sikeresen mentve! :)')
        this.editForm?.reset(this.member);
      }
      })
    
  }
}
