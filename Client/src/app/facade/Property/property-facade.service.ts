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

  public getProperty(propertyId: string): Observable<IPropertyDetail> {
    return this._propertyAPIService.getProperty(propertyId)
      .pipe(
        take(1),
        map((res) => res.data?.item));
  }

  public createProperty(propertyDto: IPropertyPostDto, imageName: string) {
    return this._propertyAPIService.postProperty(propertyDto)
      .pipe(
        take(1),
        switchMap((res) => this.getProperty(res.id)
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
}
