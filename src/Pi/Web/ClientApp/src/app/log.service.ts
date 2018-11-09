declare var signalR: any;

import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';

@Injectable()
export class LogService {

  private logEventSource = new Subject<any>();
  public logEventEmitted = this.logEventSource.asObservable();

  constructor() { 
    let connection = new signalR.HubConnectionBuilder().withUrl("/log").build();
    connection.start().catch(err => document.write(err));

    connection.on("LogEventEmitted", (logEvent) => {
      this.logEventSource.next(logEvent);
    });
  }

}
