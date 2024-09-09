import { HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ITokenDto } from 'src/app/models/DTOs/User/ITokenDto';
import { ITokenPostDto } from 'src/app/models/DTOs/User/ITokenPostDto';
import { IUserDTO } from 'src/app/models/DTOs/User/IUserDTO';
import { IUserPostDto } from 'src/app/models/DTOs/User/IUserPostDto';
import { Endpoints } from '../api-endpoints';
import { BackendService } from '../base/backend.service';

@Injectable({
  providedIn: 'root'
})
export class ProfileApiService {

  private _backendService = inject(BackendService);
  
  public registerUser(userDto: IUserPostDto): Observable<IUserDTO> {
    return this._backendService.post(Endpoints.API_User_Post, userDto);
  }

  public login(userDto: ITokenPostDto): Observable<ITokenDto> {
    return this._backendService.post(Endpoints.API_Authorise_User, userDto);
  }

  public getProfile(params?: HttpParams): Observable<IUserDTO> {
    return this._backendService.get(Endpoints.API_Profile_Get, params, true);
  }
}

