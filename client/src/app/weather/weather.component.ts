import { Component } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-weather',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './weather.component.html',
  styleUrls: ['./weather.component.css']
})
export class WeatherComponent {
  forecasts: any[] = [];

  constructor(private http: HttpClient) {
    this.loadForecasts();
  }

  // use async/await here
  async loadForecasts() {
    try {
      this.forecasts = await firstValueFrom(
        this.http.get<any[]>('https://localhost:5001/api/WeatherForecast')
      );
    } catch (error) {
      console.error('Error fetching weather data', error);
    }
  }
}
