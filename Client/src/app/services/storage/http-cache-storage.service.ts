import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IStorageEntry } from './models/IStorageEntry.model';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class HttpCacheStorageService extends StorageService {

  private cacheEntries = new Map<string, IStorageEntry<HttpResponse<unknown>>>();

  public getItem<T>(key: string, ignoreExpiry: boolean): T & HttpResponse<unknown> | undefined {
    const appKey = this.generateKey(key);
    const entry = this.cacheEntries.get(appKey);
    if (entry && (ignoreExpiry || !this.isExpired(entry.entryTime))) {
      return entry.value as T & HttpResponse<unknown>;
    }
    return;
  }

  public setItem<T>(key: string, value: T & HttpResponse<unknown>): void {
    const appKey = this.generateKey(key);
    const entry = this.generateEntry(value);
    this.cacheEntries.set(appKey, entry);
  }

  public removeItem(key: string): void {
    const appKey = this.generateKey(key);
    this.cacheEntries.delete(appKey);
  }

  public removeAll(): void {
    this.cacheEntries.clear();
  }

  public getCacheSize(): number {
    return this.cacheEntries.size;
  }

  private generateEntry<T>(value: T & HttpResponse<unknown>): IStorageEntry<HttpResponse<unknown>> {
    return {
      dataType: (value as object).constructor?.name || typeof(value),
      entryTime: Date.now(),
      value: value as HttpResponse<unknown>
    }
  }
}
