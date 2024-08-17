import { inject, Injectable } from '@angular/core';
import { ProfileApiService } from 'src/app/api/profile/profile-api.service';
import { IUserPostDto } from './../../models/DTOs/User/IUserPostDto';
import { BehaviorSubject, Observable, switchMap, take, tap } from 'rxjs';
import { ITokenPostDto } from 'src/app/models/DTOs/User/ITokenPostDto';
import { IUserDetail, IUserDTO } from 'src/app/models/DTOs/User/IUserDTO';
import { LocalStorageService } from 'src/app/services/storage/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class ProfileFacadeService {

  private _profileApi = inject(ProfileApiService);
  private _localStorageService = inject(LocalStorageService);
  private _currentProfile$ = new BehaviorSubject<IUserDetail | null>(null);
  public currentProfile$ = this._currentProfile$.asObservable();
  public currentProfile!: IUserDetail;

  public registerUser(userPostDto: IUserPostDto): Observable<IUserDTO> {
    return this._profileApi.registerUser(userPostDto)
      .pipe(
        switchMap(() => this.login({
          username: userPostDto.username, 
          password: userPostDto.password})));
  }

  public login(tokenDto: ITokenPostDto): Observable<IUserDTO> {
    return this._profileApi.login(tokenDto)
      .pipe(
        take(1),
        tap(res => console.log(res)),
        tap(tokens => this._localStorageService.setItem("accessTokenDetails", tokens.data?.item)),
        switchMap(() => this.getProfile()));
  }

  public getProfile(): Observable<IUserDTO> {
    return this._profileApi.getProfile()
      .pipe(
        take(1),
        tap((res) => this.setProfile(res.data?.item)),);
  }

  private setProfile(profile: IUserDetail | null) {
    this._currentProfile$.next(profile);
    if (profile) {
      this.currentProfile = profile;
    }
  }
}
