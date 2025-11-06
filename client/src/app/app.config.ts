import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { FormsModule } from '@angular/forms'; // ✅ Required for ngModel
import { authInterceptor } from './auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    // ✅ One provideHttpClient with interceptors
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),

    // ✅ Import FormsModule for ngModel
    importProvidersFrom(FormsModule)
  ]
};
