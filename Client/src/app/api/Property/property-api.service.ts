import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IPropertyPostDto } from 'src/app/models/DTOs/Property/IPropertyPostDto';
import { Endpoints } from '../api-endpoints';
import { BackendService } from '../base/backend.service';
import { IPropertyDto } from 'src/app/models/DTOs/Property/IPropertyDto';
import { IPropertyImageAssignmentPutDto } from 'src/app/models/DTOs/Property/IPropertyImageAssignmentPutDto';
import { HttpParams } from '@angular/common/http';
import { ITaskImageAssignmentPutDto } from 'src/app/models/DTOs/Tasks/ITaskImageAssignmentPutDto';
import { ITaskPostDto } from 'src/app/models/DTOs/Tasks/ITaskPostDto';
import { ITaskDto } from 'src/app/models/DTOs/Tasks/ITaskDto';
import { IPropertyUserAssignmentPutDto } from 'src/app/models/DTOs/Property/IPropertyUserAssignmentPutDto';

@Injectable({
  providedIn: 'root'
})
export class PropertyApiService {

  private _backendService = inject(BackendService);

  public getProperty(propertyId: string, ownerId: string): Observable<IPropertyDto> {
    let params = new HttpParams();
    params = params.append('propertyId', propertyId);
    params = params.append('ownerId', ownerId);
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

  public putPropertyUser(propertyId: string, userAssignDto: IPropertyUserAssignmentPutDto): Observable<IPropertyDto> {
    let params = new HttpParams();
    params = params.append('propertyId', propertyId);
    return this._backendService.put(Endpoints.API_Property_Assign_User, userAssignDto, params);
  }

  public getTask(taskId: string, propertyId: string, propertyOwnerId: string): Observable<ITaskDto> {
    let params = new HttpParams();
    params = params.append('taskId', taskId);
    params = params.append('propertyId', propertyId);
    params = params.append('propertyOwnerId', propertyOwnerId);
    return this._backendService.get(Endpoints.API_Task_Get, params, true);
  }

  public postTask(taskDto: ITaskPostDto): Observable<ITaskDto> {
    return this._backendService.post(Endpoints.API_Task_Post, taskDto);
  }

  public putTaskImage(taskId: string, propertyId: string, imageAssignDto: ITaskImageAssignmentPutDto): Observable<ITaskDto> {
    let params = new HttpParams();
    params = params.append('taskId', taskId);
    params = params.append('propertyId', propertyId);
    return this._backendService.put(Endpoints.API_Task_Assign_Image, imageAssignDto, params);
  }

}
