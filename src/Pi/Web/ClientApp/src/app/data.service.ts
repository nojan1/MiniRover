declare var signalR: any;

import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';

@Injectable()
export class DataService {

  public sodarData: any = {};
  
  private connection: any;

  private sodarUpdatedSource = new Subject<any>();
  public sodarUpdated = this.sodarUpdatedSource.asObservable();

  constructor() {
    this.connection = new signalR.HubConnectionBuilder().withUrl("/data").build();
    this.connection.start().catch(err => document.write(err));

    this.connection.on("SodarUpdate", (sodarUpdateData) => {
      this.sodarUpdatedSource.next(sodarUpdateData);
    });
  }

}
