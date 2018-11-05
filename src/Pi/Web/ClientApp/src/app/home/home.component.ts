import { Component } from '@angular/core';
import { DataService } from '../data.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  public imuReading: any = {};

  constructor(public dataService: DataService) {
    dataService.imuUpdated.subscribe(x => this.imuReading = x);
  }
}
