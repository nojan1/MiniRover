declare var signalR: any;

import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';

export interface LogEvent {
  message: string;
  level: string;
  properties: any;
  exceptionMessage: any;
}

@Injectable()
export class LogService {

  private logEventSource = new Subject<LogEvent>();
  public logEventEmitted = this.logEventSource.asObservable();

  constructor() { 
    let connection = new signalR.HubConnectionBuilder().withUrl("/log").build();
    connection.start().catch(err => document.write(err));

    connection.on("LogEventEmitted", (logEvent: LogEvent) => {
      this.logEventSource.next(logEvent);
    });
  }

  public logEvent(event: LogEvent){
    this.logEventSource.next(event);
  }

}
