import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IPropertyPostDto } from 'src/app/models/DTOs/Property/IPropertyPostDto';
import { Endpoints } from '../api-endpoints';
import { BackendService } from '../base/backend.service';
import { IPropertyDto } from 'src/app/models/DTOs/Property/IPropertyDto';
import { IPropertyImageAssignmentPutDto } from 'src/app/models/DTOs/Property/IPropertyImageAssignmentPutDto';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PropertyApiService {

  private _backendService = inject(BackendService);

  public getProperty(propertyId: string): Observable<IPropertyDto> {
    let params = new HttpParams();
    params = params.append('propertyId', propertyId);
    return this._backendService.get(Endpoints.API_Property_Get, params, true);
  }

  public postProperty(propertyDto: IPropertyPostDto): Observable<IPropertyDto> {
    return this._backendService.post(Endpoints.API_Property_Post, propertyDto);
  }

  public putPropertyImage(propertyId: string, imageAssignDto: IPropertyImageAssignmentPutDto): Observable<IPropertyDto> {
    let params = new HttpParams();
    params = params.append('propertyId', propertyId);
    return this._backendService.put(Endpoints.API_Property_Assign_Image, imageAssignDto, params);
  }

}
