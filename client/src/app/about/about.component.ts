import { Component, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { interval } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h2>About Component</h2>
    <p>This is the About page.</p>
    <button (click)="goBack()">Go Back to Weather</button>
  `,
  styles: [`
    h2 { color: green; }
    button { margin-top: 1rem; }
  `]
})
export class AboutComponent {
  private destroyRef = inject(DestroyRef);

  constructor() {
    interval(2000)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => console.log('About unsubscribed ✅'))
      )
      .subscribe(val => console.log('About Tick:', val));
  }

  goBack() {
    window.location.href = '/weather'; // quick navigation
  }
}
