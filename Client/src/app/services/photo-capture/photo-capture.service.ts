import { Injectable } from '@angular/core';
import { Camera, CameraResultType, CameraSource, Photo } from '@capacitor/camera';
import { Geolocation, Position } from '@capacitor/geolocation';
import { Subject } from 'rxjs';
import { BlobServiceClient } from '@azure/storage-blob';
import { GUID } from 'src/app/helpers/Guid';
// import {
//   BlobHTTPHeaders,
//   BlobServiceClient,
//   BlockBlobClient,
//   ContainerClient,
// } from '@azure/storage-blob';

@Injectable({
  providedIn: 'root'
})
export class PhotoCaptureService {

  private _photos = new Map<string, Blob>();
  private _nextPhoto$ = new Subject<string>();
  public nextPhoto$ = this._nextPhoto$.asObservable();

  public async addNewToGallery(): Promise<string | null> {

    // Take a photo
    const capturedPhoto = await Camera.getPhoto({
      resultType: CameraResultType.Base64,
      source: CameraSource.Camera,
      quality: 100,
      allowEditing: true
    }).catch(err => null);

    if (capturedPhoto?.base64String) {

      const blob = this.dataURItoBlob(capturedPhoto.base64String, capturedPhoto.format);
      const fileName = `${GUID.GeneratGuid()}.${capturedPhoto.format}`;

      console.log(capturedPhoto);
      console.log(blob);

      this._photos.set(fileName, blob);

      return fileName;
    }

    return null;
  }

  public getImageBlob(fileName: string) {
    return this._photos.get(fileName);
  }


  // insipred from https://stackoverflow.com/a/7261048
  private dataURItoBlob(b64Data: string , format: string): Blob {
    const sliceSize = 512
    const byteCharacters = atob(b64Data);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      const slice = byteCharacters.slice(offset, offset + sliceSize);
  
      const byteNumbers = new Array(slice.length);
      for (let i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }
  
      const byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }
      
    const blob = new Blob(byteArrays, {type: `image/${format}`});
    return blob;
  }


  private async getUserPosition(): Promise<Position> {
    const coordinates = await Geolocation.getCurrentPosition();
    return coordinates;
  }
}
