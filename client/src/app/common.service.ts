import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommonService {
  myValue: string = "Hello from common service";
  constructor() { }
}
