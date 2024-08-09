import { enableProdMode } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouteReuseStrategy, provideRouter, withPreloading, PreloadAllModules } from '@angular/router';
import { IonicRouteStrategy, provideIonicAngular } from '@ionic/angular/standalone';
import { defineCustomElements } from '@ionic/pwa-elements/loader';

import { routes } from './app/app.routes';
import { AppComponent } from './app/app.component';
import { environment } from './environments/environment';
import { Ion } from '@cesium/engine';
import { provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { httpCacheInterceptor } from './app/interceptors/http-cache-interceptor';

defineCustomElements(window);

(window as Record<string, any>)['CESIUM_BASE_URL'] = '/assets/cesium/';

Ion.defaultAccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIzZGFiODFkMi1lM2VhLTRmMGMtYmQ1MS1lZDI5NmYxYTdjNGEiLCJpZCI6MTU3MzYsInNjb3BlcyI6WyJhc3IiLCJnYyJdLCJpYXQiOjE1Njg3MzE5NzB9.9qw8Aceuwh0AHDn-JC2SPZnjcAhznFs35-y_JosSTSA";


if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    provideIonicAngular(),
    provideRouter(routes, withPreloading(PreloadAllModules)),
    provideHttpClient(
      withInterceptors([
        httpCacheInterceptor,
      ]),
      withInterceptorsFromDi()
    ),
  ],
});
