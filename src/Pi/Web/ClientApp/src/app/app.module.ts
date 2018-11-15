import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { DataService } from './data.service';
import { SodarDirective } from './sodar.directive';
import { LogComponent } from './log/log.component';
import { LogService } from './log.service';
import { ProgramService } from './program.service';
import { CameraService } from './camera.service';
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    SodarDirective,
    LogComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
    ])
  ],
  providers: [DataService, LogService, ProgramService, CameraService],
  bootstrap: [AppComponent]
})
export class AppModule { }
