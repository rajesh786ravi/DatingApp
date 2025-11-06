import { Routes } from '@angular/router';
import { WeatherComponent } from './weather/weather.component';
import { AboutComponent } from './about/about.component';

export const routes: Routes = [
    { path: '', redirectTo: 'weather', pathMatch: 'full' }, // redirect to weather
    { path: 'weather', component: WeatherComponent },
    { path: 'about', component: AboutComponent }
];
