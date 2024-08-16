import { APP_INITIALIZER, enableProdMode } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouteReuseStrategy, provideRouter, withPreloading, PreloadAllModules, withRouterConfig } from '@angular/router';
import { IonicRouteStrategy, provideIonicAngular } from '@ionic/angular/standalone';
import { defineCustomElements } from '@ionic/pwa-elements/loader';

import { routes } from './app/app.routes';
import { AppComponent } from './app/app.component';
import { environment } from './environments/environment.local';
import { Ion } from '@cesium/engine';
import { provideHttpClient, withInterceptors, withInterceptorsFromDi, withXsrfConfiguration } from '@angular/common/http';
import { httpCacheInterceptor } from './app/interceptors/http-cache-interceptor';
import { tokenInterceptor } from './app/interceptors/http-auth-interceptor';
import { initializeApplication } from './app/services/authentication/app-initializer';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';


defineCustomElements(window);

(window as Record<string, any>)['CESIUM_BASE_URL'] = '/assets/cesium/';

Ion.defaultAccessToken = environment.cesiumKey;

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    provideIonicAngular({ mode: 'ios'}),
    provideRouter(
      routes, 
      withPreloading(PreloadAllModules),
      withRouterConfig({
        paramsInheritanceStrategy: 'always'  // Allows to inherit params from parent routes
      })),
    provideHttpClient(
      withXsrfConfiguration({
        cookieName: 'XSRF-TOKEN',
        headerName: 'X-CSRFTOKEN',
      }),
      withInterceptors([
        tokenInterceptor,
        httpCacheInterceptor,
      ]),
      withInterceptorsFromDi()
    ),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApplication,
      multi: true,
    },
    provideAnimationsAsync(),
  ],
});
