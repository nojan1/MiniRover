import { Directive, ElementRef } from '@angular/core';
import { DataService } from './data.service';

const MAX_DISTANCE = 80;

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
    let center = [this.width / 2, this.height / 2];
    let maxRadius = (this.width / 2) * 0.9;

    this.ctx.clearRect(0, 0, this.width, this.height);
    this.drawDistanceRangeMarkings(data.ranges, maxRadius, center);
    this.drawDistanceCircles(5, maxRadius, center);
  }

  drawDistanceRangeMarkings(rawRanges:any, maxRadius: number, center: number[]): any {
    let angles = Object.keys(rawRanges).map(x => +x) as number[];
    let ranges = Object.values(rawRanges as number[]);

    let maxAngle = Math.max(...angles) - 180;
    let minAngle = Math.min(...angles) - 180;

    let angleStep = Math.abs(maxAngle - minAngle) / (ranges.length - 1);

    let angleFor = (i) => minAngle + (angleStep * i);

    this.ctx.fillStyle = "red";
    for (let i = 0; i < ranges.length; i++) {
      if (ranges[i] < 1 || ranges[i] > MAX_DISTANCE)
        continue;

      let straightAngle = angleFor(i);
      let startAngle = (straightAngle - ((straightAngle - angleFor(i - 1)) / 2)) * (Math.PI / 180);
      let endAngle = (straightAngle + ((angleFor(i + 1) - straightAngle) / 2)) * (Math.PI / 180);
      let radius = (ranges[i] / MAX_DISTANCE) * maxRadius;

      this.ctx.beginPath();
      this.ctx.arc(center[0], center[1], maxRadius, startAngle, endAngle);
      this.ctx.arc(center[0], center[1], radius, endAngle, startAngle, true);
      this.ctx.fill();
    }
  }

  drawDistanceCircles(numCircles: number, maxRadius: number, center: number[]) {
    let circleSpacing = maxRadius / numCircles;
    let distanceSpacing = MAX_DISTANCE / numCircles;

    this.ctx.strokeStyle = "darkgreen";
    for (let i = 0; i < numCircles; i++) {
      let radius = maxRadius - (i * circleSpacing);
      let distance = MAX_DISTANCE - (i * distanceSpacing);

      this.ctx.beginPath();
      this.ctx.arc(center[0], center[1], radius, 0, 2 * Math.PI);
      this.ctx.stroke();

      this.ctx.strokeText(distance.toString(), center[0] - 5, center[1] + radius + 10);
    }

  }
}
