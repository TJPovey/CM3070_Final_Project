import { Observable, of, tap } from "rxjs";
import { ProfileFacadeService } from "src/app/facade/Profile/profile-facade.service";
import { inject } from '@angular/core';
import { IUserDTO } from "src/app/models/DTOs/User/IUserDTO";
import { LocalStorageService } from "../storage/local-storage.service";
import { ITokenDetail } from "src/app/models/DTOs/User/ITokenDto";

export function initializeApplication() {

    const profileService = inject(ProfileFacadeService);
    const localStorage = inject(LocalStorageService);

    const tokenDetails = localStorage.getItem<ITokenDetail>("accessTokenDetails", false);

    if (tokenDetails?.accessToken) {
        return (): Observable<IUserDTO> => profileService.getProfile();
    } else {
        return (): Observable<null> => of(null);
    }
}
  