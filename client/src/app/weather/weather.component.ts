import { Component, signal, effect } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { CommonService } from '../common.service';

const count = signal(0);
const user = signal({ name: "Rajesh", age: 30 });
const firstName = signal('Rajesh');
const lastName = signal('Sandhiya');


@Component({
  selector: 'app-weather',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './weather.component.html',
  styleUrls: ['./weather.component.css'],
  providers: [CommonService]
})
export class WeatherComponent {
  forecasts: any[] = [];

  constructor(private http: HttpClient, public service: CommonService) {
    debugger
    this.loadForecasts();
    this.service.myValue = "Changed";

    effect(() => {
      debugger
      const fullName = `${firstName()} ${lastName()}`;
      console.log('Full name is:', fullName);
    });

    firstName.set('Raja');  // Logs: Full name is: Raja Sandhiya
    lastName.set('S');      // Logs: Full name is: Raja S

  }

  // use async/await here
  async loadForecasts() {
    try {
      this.forecasts = await firstValueFrom(
        this.http.get<any[]>('https://localhost:5001/api/WeatherForecast')
      );
    } catch (error) {
      // console.log("Signal count: " + count());
      // count.set(5);
      // console.log("Signal count after set: " + count());
      // count.update(s => s = 100);
      // console.log("Signal count after update: " + count());


      console.log(user());
      user.set({ name: "Rajesh Sandhiya", age: 30 });
      console.log(user());
      console.error('Error fetching weather data', error);
      user.update(current => ({ ...current, age: 35 }));
      console.log(user());
    }
  }
}
