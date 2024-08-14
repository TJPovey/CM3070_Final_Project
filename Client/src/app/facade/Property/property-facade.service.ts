import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, combineLatest, distinctUntilChanged, map, Observable, Subject, switchMap } from 'rxjs';
import { PropertyService } from 'src/app/api/Property/property.service';
import { IPropertyListItemDTO } from 'src/app/models/DTOs/Property/IPropertyListGetDTO';
import { IResponseCollectionDTO } from 'src/app/models/DTOs/Responses/IResponseCollectionDTO';
import { PaginationOptions } from 'src/app/models/Pagination/IPaginationOptions';
import { ProfileFacadeService } from '../Profile/profile-facade.service';
import { IPropertyAssignment } from 'src/app/models/DTOs/User/IUserDTO';

@Injectable({
  providedIn: 'root'
})
export class PropertyFacadeService {

  private _propertyAPIService = inject(PropertyService);
  private _profileFacadeService = inject(ProfileFacadeService);

  public propertyCollection$: Observable<IPropertyAssignment[]>;

  constructor() {
    this.propertyCollection$ = this._profileFacadeService.currentProfile$
      .pipe(map(user => user?.propertyAssignments || []))
  }

}
