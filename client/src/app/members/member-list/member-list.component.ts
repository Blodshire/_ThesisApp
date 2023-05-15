import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from '../../_models/member';
import { Pagination } from '../../_models/pagination';
import { User } from '../../_models/user';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  //members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{ value: 'male', display: 'férfiakat' }, {value:'female', display: 'nőket'}]

  constructor(private membersService: MembersService,) {
    this.userParams = membersService.getUserParams();
  }

  ngOnInit(): void {
    //this.members$ = this.membersService.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    if (!this.userParams)
      return;
    this.membersService.setUserParams(this.userParams);
    this.membersService.getMembers(this.userParams).subscribe({
      next: response => {
        if (response.result && response.pagination) {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      }
      })
  }

  resetFilters() {
      this.userParams = this.membersService.resetUserParams();
      this.loadMembers();
    
  }

  pageChanged(event: any) {
    if (this.userParams?.pageNumber !== event.page && this.userParams) {
      this.userParams.pageNumber = event.page;
      this.membersService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
