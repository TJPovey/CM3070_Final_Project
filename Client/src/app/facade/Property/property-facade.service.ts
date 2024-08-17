import { inject, Injectable } from '@angular/core';
import { BlobServiceClient } from '@azure/storage-blob';
import { forkJoin, from, map, Observable, of, switchMap, take, tap } from 'rxjs';
import { PropertyApiService } from 'src/app/api/Property/property-api.service';
import { IPropertyDetail, IPropertyDto } from 'src/app/models/DTOs/Property/IPropertyDto';
import { IPropertyImageAssignmentPutDto } from 'src/app/models/DTOs/Property/IPropertyImageAssignmentPutDto';
import { IPropertyPostDto } from 'src/app/models/DTOs/Property/IPropertyPostDto';
import { IPropertyAssignment } from 'src/app/models/DTOs/User/IUserDTO';
import { PhotoCaptureService } from 'src/app/services/photo-capture/photo-capture.service';
import { ProfileFacadeService } from '../Profile/profile-facade.service';
import { ITaskPostDto } from 'src/app/models/DTOs/Tasks/ITaskPostDto';
import { ITaskImageAssignmentPutDto } from 'src/app/models/DTOs/Tasks/ITaskImageAssignmentPutDto';
import { ITaskDetail, ITaskDto } from 'src/app/models/DTOs/Tasks/ITaskDto';
import { IPropertyUserAssignmentPutDto } from 'src/app/models/DTOs/Property/IPropertyUserAssignmentPutDto';

@Injectable({
  providedIn: 'root'
})
export class PropertyFacadeService {

  private _propertyAPIService = inject(PropertyApiService);
  private _profileFacadeService = inject(ProfileFacadeService);
  private _photoCaptureService = inject(PhotoCaptureService);
  public propertyCollection$: Observable<IPropertyAssignment[]>;

  private _propertyCollection = new Map<string, IPropertyAssignment>();

  constructor() {
    this.propertyCollection$ = this._profileFacadeService.currentProfile$
      .pipe(
        map(user => user?.propertyAssignments || []),
        tap(assignments => {
          assignments.forEach(res => this._propertyCollection.set(res.property.id, res));
        }));
  }

  public getPropertyAssignment(propertyId: string): IPropertyAssignment | undefined {
    return this._propertyCollection.get(propertyId);
  }

  public getProperty(propertyId: string, ownerId: string): Observable<IPropertyDetail> {
    return this._propertyAPIService.getProperty(propertyId, ownerId)
      .pipe(
        take(1),
        map((res) => res.data?.item));
  }

  public createProperty(propertyDto: IPropertyPostDto, imageName: string) {
    return this._propertyAPIService.postProperty(propertyDto)
      .pipe(
        take(1),
        switchMap((res) => this.getProperty(res.id, this._profileFacadeService.currentProfile.id)
      .pipe(
        take(1),
        switchMap((res) => from(this.uploadPropertyImage(imageName, res))
      .pipe(
        take(1),
        switchMap((res) => this.assignImageToProperty(res.id, { imageName })
      ))
      .pipe(
        take(1),
        switchMap((res) => {
          return forkJoin([this._profileFacadeService.getProfile(), of(res)])
        }))
      ))));
  }

  public assignUserToProperty(
    propertyId: string,
    ownerId: string,
    propertyUserDto: IPropertyUserAssignmentPutDto): Observable<IPropertyDetail> {
      return this._propertyAPIService.putPropertyUser(propertyId, propertyUserDto)
        .pipe(switchMap(res => this.getProperty(propertyId, ownerId)));
  }

  private assignImageToProperty(
    propertyId: string,
    propertyImageDto: IPropertyImageAssignmentPutDto): Observable<IPropertyDto> {
      return this._propertyAPIService.putPropertyImage(propertyId, propertyImageDto);
  }

  private async uploadPropertyImage(imageName: string, propertyDetails: IPropertyDetail): Promise<IPropertyDetail> {

    if (propertyDetails.writeToken) {
      const blobServiceClient = new BlobServiceClient(propertyDetails.writeToken);
      const blobClient = blobServiceClient.getContainerClient(propertyDetails.id);
      const blockBlobClient = blobClient.getBlockBlobClient(`images/${imageName}`);
      const blob = this._photoCaptureService.getImageBlob(imageName);

      if (blob) {
        await blockBlobClient.uploadData(blob, {
          blobHTTPHeaders: {
            blobContentType: blob.type,
          }
        });
      }
    }

    return propertyDetails;
  }

  public getTask(taskId: string, propertyId: string, ownerId: string): Observable<ITaskDetail> {
    return this._propertyAPIService.getTask(taskId, propertyId, ownerId)
      .pipe(
        take(1),
        map((res) => res.data?.item));
  }

  public createTask(taskDto: ITaskPostDto, imageName: string, writeToken: string, ownerId: string) {
    return this._propertyAPIService.postTask(taskDto)
      .pipe(
        take(1),
        switchMap((res) => this.getTask(res.id, taskDto.propertyId, taskDto.userPropertyOwnerId)
      .pipe(
        take(1),
        switchMap((res) => from(this.uploadTaskImage(imageName, writeToken, res))
      .pipe(
        take(1),
        switchMap((res) => this.assignImageToTask(res.id, res.propertyId, { imageName })
      .pipe(
        take(1),
        switchMap(() => this.getProperty(taskDto.propertyId, ownerId))
      )))
      ))));
  }

  private assignImageToTask(
    taskId: string,
    propertyId: string,
    taskImageDto: ITaskImageAssignmentPutDto): Observable<ITaskDto> {
      return this._propertyAPIService.putTaskImage(taskId, propertyId, taskImageDto);
  }

  private async uploadTaskImage(imageName: string, writeToken: string, taskDetails: ITaskDetail): Promise<ITaskDetail> {

    const blobServiceClient = new BlobServiceClient(writeToken);
    const blobClient = blobServiceClient.getContainerClient(taskDetails.propertyId);
    const blockBlobClient = blobClient.getBlockBlobClient(`tasks/${taskDetails.id}/${imageName}`);
    const blob = this._photoCaptureService.getImageBlob(imageName);

    if (blob) {
      await blockBlobClient.uploadData(blob, {
        blobHTTPHeaders: {
          blobContentType: blob.type,
        }
      });
    }

    return taskDetails;
  }
}
