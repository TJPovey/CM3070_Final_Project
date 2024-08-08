import { Injectable } from '@angular/core';
import { Camera, CameraResultType, CameraSource, Photo } from '@capacitor/camera';
import { Filesystem, Directory } from '@capacitor/filesystem';
import { Preferences } from '@capacitor/preferences';
import { Geolocation, Position } from '@capacitor/geolocation';
import { Subject } from 'rxjs';

export interface UserPhoto {
  id: string;
  filepath: string;
  webviewPath?: string;
  location?: Position;
}


@Injectable({
  providedIn: 'root'
})
export class PhotoCaptureService {

  private _photos = new Map<string, UserPhoto>();
  private _nextPhoto$ = new Subject<UserPhoto>();
  public nextPhoto$ = this._nextPhoto$.asObservable();

  public async addNewToGallery() {
    // Take a photo
    const capturedPhoto = await Camera.getPhoto({
      resultType: CameraResultType.Uri,
      source: CameraSource.Camera,
      quality: 100,
      allowEditing: false
    }).catch(err => null);

    if (capturedPhoto) {

      const position = await this.getUserPosition();

      const photo = {
        id: this.generatGuid(),
        filepath: "when blob storage has been implemented...",
        webviewPath: capturedPhoto.webPath,
        location: position
      };

      this._photos.set(photo.id, photo);
      this._nextPhoto$.next(photo);
    }
  }

  public getPhotoById(id: string) {
    return this._photos.get(id);
  }

  private async getUserPosition(): Promise<Position> {
    const coordinates = await Geolocation.getCurrentPosition();
    return coordinates;
  }

  private generatGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString();
    });
  }
}
