declare var signalR: any;

import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Injectable()
export class DataService {

  public sodarData: any = {};
  public visionData: SafeResourceUrl;

  private connection: any;

  private sodarUpdatedSource = new Subject<any>();
  public sodarUpdated = this.sodarUpdatedSource.asObservable();

  private imuUpdatedSource = new Subject<any>();
  public imuUpdated = this.imuUpdatedSource.asObservable();

  constructor(private _sanitizer: DomSanitizer) {
    this.connection = new signalR.HubConnectionBuilder().withUrl("/data").build();
    this.connection.start().catch(err => document.write(err));

    this.connection.on("SodarUpdate", (sodarUpdateData) => {
      this.sodarUpdatedSource.next(sodarUpdateData);
    });

    this.connection.on("IMUReading", (imuReading) => {
      this.imuUpdatedSource.next(imuReading);
    });

    this.connection.on("VisionUpdate", (data) => {
      this.visionData = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/png;base64,' + data);
    });
  }

}
