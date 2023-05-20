import { Component, Input } from '@angular/core';
import { TimeagoIntl } from 'ngx-timeago';
import { strings as hunStrings } from 'ngx-timeago/language-strings/hu';

@Component({
  selector: 'app-time-ago-span',
  templateUrl: './time-ago-span.component.html',
  styleUrls: ['./time-ago-span.component.css']
})
export class TimeAgoSpanComponent {
  @Input() displayDate = Date

  constructor(intl: TimeagoIntl) {
    console.log(intl);
    intl.strings = hunStrings;
    intl.changes.next();
  }

}
