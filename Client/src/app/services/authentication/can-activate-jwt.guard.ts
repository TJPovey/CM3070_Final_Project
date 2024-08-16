import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot, UrlSegment } from '@angular/router';
import { of } from 'rxjs';
import { ITokenDetail } from 'src/app/models/DTOs/User/ITokenDto';
import { LocalStorageService } from '../storage/local-storage.service';
import { AppRoute } from 'src/app/screens/app-routes.enum';


export const authenticateGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const router = inject(Router);  
    const localStorageService = inject(LocalStorageService);

    if (localStorageService.getItem<ITokenDetail>("accessTokenDetails", false)) {

      // if (state.url.includes(AppRoute.Login)) {
      //   return router.navigate([AppRoute.Home]);
      // }

      return of(true);
    } else {
      return router.navigate([AppRoute.Login]);
    }
};

export const authenticateGuardForChild: CanActivateChildFn = 
  (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => authenticateGuard(route, state);