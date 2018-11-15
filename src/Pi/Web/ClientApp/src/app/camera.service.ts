import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CameraService {

  constructor(private http: HttpClient) { }

  public setSource(source: number) {
    return this.http.post("/api/camera/source/" + source.toString(), {});
  }

}
