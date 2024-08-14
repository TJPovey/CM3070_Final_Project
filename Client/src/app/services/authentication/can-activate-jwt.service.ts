import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { of } from 'rxjs';
import { ITokenDetail } from 'src/app/models/DTOs/User/ITokenDto';
import { AppRoute } from 'src/app/pages/app-routes.enum';
import { LocalStorageService } from '../storage/local-storage.service';


export const authenticateGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const router = inject(Router);  
    const localStorageService = inject(LocalStorageService);

    if (localStorageService.getItem<ITokenDetail>("accessTokenDetails", false)) {
      return of(true);
    } else {
      return router.navigate([AppRoute.Register]);
    }
};

export const authenticateGuardForChild: CanActivateChildFn = 
  (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => authenticateGuard(route, state);