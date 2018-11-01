import { Directive, ElementRef } from '@angular/core';
import { DataService } from './data.service';

@Directive({
  selector: '[appSodar]'
})
export class SodarDirective {

  private ctx: CanvasRenderingContext2D
  private width: number;
  private height: number;

  constructor(el: ElementRef, private dataService: DataService) {
    this.ctx = el.nativeElement.getContext("2d");
    this.width = el.nativeElement.width;
    this.height = el.nativeElement.height;

    dataService.sodarUpdated.subscribe(x => this.redraw(x));
  }

  private redraw(data: any) {
    let ranges = data.ranges as number[];
    let max = Math.max(...ranges.filter(x => x > 0));
    let min = Math.min(...ranges.filter(x => x > 0));
    let middle = (max - min) / 2;

    let startAngle = -180;
    let endAngle = 0;
    let angleStep = Math.abs(endAngle - startAngle) / (ranges.length - 1);

    let zoom = (this.width * 0.4) / max;

    let center = [this.width / 2, this.height / 2];

    this.ctx.clearRect(0, 0, this.width, this.height);

    //Draw distance circles
    this.ctx.strokeStyle = "gray";
    this.drawDistanceCircle(max, zoom, center);
    this.drawDistanceCircle(middle, zoom, center);
    this.drawDistanceCircle(min, zoom, center);

    this.ctx.strokeStyle = "black";
    //Draw spokes
    for (let i = 0; i < ranges.length; i++) {
      if (ranges[i] < 1)
        continue;

      let angle = startAngle + (angleStep * i);
      let x = (Math.cos(angle * (Math.PI / 180)) * ranges[i] * zoom) + center[0];
      let y = (Math.sin(angle * (Math.PI / 180)) * ranges[i] * zoom) + center[1];

      this.ctx.beginPath();
      this.ctx.moveTo(center[0], center[1]);
      this.ctx.lineTo(x, y);
      this.ctx.stroke();
    }

  }

  drawDistanceCircle(radius: number, zoom: number, center: number[]) {
    this.ctx.beginPath();
    this.ctx.arc(center[0], center[1], radius * zoom, 0, 2 * Math.PI);
    this.ctx.stroke();

    this.ctx.strokeText(radius.toString(), center[0], center[1] + (radius * zoom));
  }
}
