import { Component, Input } from '@angular/core';
import { TimeagoIntl } from 'ngx-timeago';
import { strings as hunStrings } from 'ngx-timeago/language-strings/hu';

@Component({
  selector: 'app-time-ago-p',
  templateUrl: './time-ago-p.component.html',
  styleUrls: ['./time-ago-p.component.css']
})
export class TimeAgoPComponent {
  @Input() displayDate: Date | undefined;

  constructor(intl: TimeagoIntl) {
    intl.strings = hunStrings;
    intl.changes.next();
  }
}
