import { Component, OnInit } from '@angular/core';
import { LogService } from '../log.service';

@Component({
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.css']
})
export class LogComponent implements OnInit {

  public logEvents = [];

  constructor(private logService: LogService) { }

  ngOnInit() {
    this.logService.logEventEmitted.subscribe(logEvent => {
      this.logEvents.push(logEvent);
    });
  }

}
