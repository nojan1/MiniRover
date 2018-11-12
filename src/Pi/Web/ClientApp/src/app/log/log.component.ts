import { Component, OnInit } from '@angular/core';
import { LogService } from '../log.service';

const MAX_LOG_EVENTS = 150;

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

      if(this.logEvents.length > MAX_LOG_EVENTS){
        this.logEvents.shift();
      }
    });
  }

}
