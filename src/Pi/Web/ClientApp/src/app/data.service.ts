declare var signalR: any;

import { Injectable } from '@angular/core';

@Injectable()
export class DataService {

  public sodarData: any = {};
  private connection: any;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder().withUrl("/data").build();
    this.connection.start().catch(err => document.write(err));

    this.connection.on("SodarUpdate", (sodarUpdateData) => {
      this.sodarData = sodarUpdateData;
    });
  }

}
