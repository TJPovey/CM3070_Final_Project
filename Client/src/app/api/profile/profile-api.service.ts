import { inject, Injectable } from '@angular/core';
import { BackendService } from '../base/backend.service';
import { HttpParams } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { IUserDTO } from 'src/app/models/DTOs/User/IUserDTO';
import { Endpoints } from '../api-endpoints';
import { IUserPostDto } from 'src/app/models/DTOs/User/IUserPostDto';
import { ITokenPostDto } from 'src/app/models/DTOs/User/ITokenPostDto';
import { ITokenDto } from 'src/app/models/DTOs/User/ITokenDto';

@Injectable({
  providedIn: 'root'
})
export class ProfileApiService {

  private _backendService = inject(BackendService);
  
  // public registerUser(params?: HttpParams): Observable<IUserDTO> {
  //   return this._backendService
  //     .get<IUserDTO>(Endpoints.API_User_Post, params, true)
  //     .pipe(shareReplay(1));
  // }

  public registerUser(userDto: IUserPostDto): Observable<IUserDTO> {
    return this._backendService.post(Endpoints.API_User_Post, userDto);
  }

  public login(userDto: ITokenPostDto): Observable<ITokenDto> {
    return this._backendService.post(Endpoints.API_Authorise_User, userDto);
  }

  public getProfile(): Observable<IUserDTO> {
    return this._backendService.get(Endpoints.API_Profile_Get);
  }

}

