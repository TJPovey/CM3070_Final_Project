import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Endpoints } from '../api/api-endpoints';
import { ITokenDetail } from '../models/DTOs/User/ITokenDto';
import { LocalStorageService } from '../services/storage/local-storage.service';
import { environment } from 'src/environments/environment.local';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {

  const localStorageService = inject(LocalStorageService);

  const accessToken = localStorageService.getItem<ITokenDetail>("accessTokenDetails", false);

  // method to set auth headers
  const authReq = () => req.clone({
      headers: req.headers.set('Authorization', `Bearer ${accessToken?.accessToken}`)
  });

  // continue if registration or token related
  if (req.url.includes(Endpoints.API_Authorise_User) ||
      req.url.includes(Endpoints.API_User_Post)) {
      return next(req.clone());
  }

  // continue url isn't to our API
  if (!req.url.includes(`${environment.gateway}${environment.gatewayPort}`)){
    return next(req.clone());
  }

  return next(authReq());
}

