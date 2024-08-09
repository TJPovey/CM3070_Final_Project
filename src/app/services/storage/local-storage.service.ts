import { Injectable } from '@angular/core';
import { StorageService } from './storage.service';
import { IStorageEntry } from './models/IStorageEntry.model';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService extends StorageService {

  public getItem<T>(key: string, ignoreExpiry: boolean): T | undefined {
    const appKey = this.generateKey(key);
    const fromloc = localStorage.getItem(appKey);
    if (fromloc) {
      const value = JSON.parse(fromloc, this.reviver<T>) as IStorageEntry<T>;
      if (ignoreExpiry || !this.isExpired(value.entryTime)) {
        return value.value;
      }
    }
    return;
  }


  public setItem<T>(key: string, value: T): void {
    const appKey = this.generateKey(key);
    const entry = this.toStorageEntry<T>(value);
    const stringValue = JSON.stringify(entry);
    localStorage.setItem(appKey, stringValue);
  }


  public removeItem(key: string): void {
    const appKey = this.generateKey(key);
    localStorage.removeItem(appKey);
  }

  public removeAll(): void {
    this.getAllKeysWithPrefix().forEach(x => localStorage.removeItem(x))
  }

  public getCacheSize(): number {
      return this.getAllKeysWithPrefix().length;
  }

  private getAllKeysWithPrefix(): string[] {
    return Object.keys(localStorage)
      .filter(x => x.startsWith(StorageService.generatePrefix()))
  }


  private toStorageEntry<T>(value: T): IStorageEntry<T> {
    return {
      dataType: (value as object).constructor?.name || typeof(value),
      entryTime: Date.now(),
      value: (value instanceof Map ? Array.from(value.entries()) : value) as T
    };
  }


  private reviver<T>(key: any, value: any): T {
    if(typeof value === 'object' && value !== null) {
      if (value.dataType === 'Map') {
        return {
          ...value,
          value: new Map(value.value)
        }
      }
    }
    return value;
  }

}
