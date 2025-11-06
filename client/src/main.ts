import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { WeatherComponent } from './app/weather/weather.component';
import { Routes } from '@angular/router';

const routes: Routes = [
  { path: '', component: AppComponent },
  { path: 'weather', component: WeatherComponent }
];

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
