import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class ProgramService {

  constructor(private http: HttpClient) { }

  public getPrograms(): Observable<string[]> {
    return this.http.get<string[]>("/api/program");
  }

  public runProgram(name: string) {
    return this.http.post("/api/program/" + name + "/run", {});
  }

  public stopProgram(){
    return this.http.post("/api/program/stop", {});
  }

}
