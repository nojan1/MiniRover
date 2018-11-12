import { Component } from '@angular/core';
import { DataService } from '../data.service';
import { ProgramService } from '../program.service';
import { LogService } from '../log.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  public programs = [];
  public selectedProgram: any;

  public imuReading: any = {};

  constructor(private dataService: DataService, private programService: ProgramService, private logService: LogService) {
    dataService.imuUpdated.subscribe(x => this.imuReading = x);

    programService.getPrograms()
      .subscribe(x => this.programs = x);
  }

  public runProgram() {
    if (this.selectedProgram){
      this.programService.runProgram(this.selectedProgram)
        .subscribe(null, (error) => {
          this.logService.logEvent({
            message: "Error ocurred when running " + error,
            exceptionMessage: null,
            level: "Error",
            properties: null
          });
        });
      }else{
        this.logService.logEvent({
          message: "No program selected",
          exceptionMessage: null,
          level: "Warning",
          properties: null
        });
      }
  }

  public stopProgram(){
    this.programService.stopProgram()
      .subscribe(x => {
        this.logService.logEvent({
          message: "Program stopped",
          exceptionMessage: null,
          level: "Information",
          properties: null
        });
      });
  }
}
