import { TestBed } from '@angular/core/testing';
import { HttpCacheStorageService } from './http-cache-storage.service';
import { IStorageEntry } from './models/IStorageEntry.model';
import { HttpResponse } from '@angular/common/http';

const FIXED_SYSTEM_TIME = '2023-04-08T00:00:00Z';

const mockCacheEntries = [
  {
    key: "key 1", 
    value: new HttpResponse({
      url: "mock/value/1"
    })
  },
  {
    key: "key 2", 
    value: new HttpResponse({
      url: "mock/value/2"
    })
  },
  {
    key: "key 3", 
    value: new HttpResponse({
      url: "mock/value/3"
    })
  },
  {
    key: "key 4", 
    value: new HttpResponse({
      url: "mock/value/4"
    })
  }
];

describe('CacheStorageService', () => {
  let service: HttpCacheStorageService;


  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HttpCacheStorageService);

    jest.useFakeTimers();
    jest.setSystemTime(Date.parse(FIXED_SYSTEM_TIME));

    mockCacheEntries.forEach(({key, value}) => {
      service.setItem(key, value);
    });
  });
  

  afterEach(() => {
    jest.useRealTimers();
  });


  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getCacheSize', () => {

    it('should return the number of entries in the cacheEntries Map', () => {
      expect(service.getCacheSize()).toEqual(mockCacheEntries.length);
    });
    
  });

  describe('generateEntry', () => {

    it('should return an object where the dataType is the name of the contructor name or type name', () => {
      const value = new HttpResponse();
      const entry: IStorageEntry<HttpResponse<unknown>> = (service as any).generateEntry(value);
      expect(entry.dataType).toBe(((value as object).constructor?.name || typeof(value)));
    });

    it('should return an object where the entryTime equals the time the object was created', () => {
      const value = new HttpResponse();
      const entry: IStorageEntry<HttpResponse<unknown>> = (service as any).generateEntry(value);
      expect(entry.entryTime).toEqual(new Date(Date.parse(FIXED_SYSTEM_TIME)).getTime());
    });

    it('should return an object where the value parameter is equal to the input value', () => {
      const value = new HttpResponse({
        url: "mock/value"
      });
      const entry: IStorageEntry<HttpResponse<unknown>> = (service as any).generateEntry(value);
      expect(entry.value).toEqual(value);
      expect(entry.value.url).toEqual(value.url);
    });
  });


  describe('setItem', () => {

    it('should store an item in the cacheEntries Map', () => {
      
      const key = "key 5";
      const value = new HttpResponse({
        url: "mock/value/5"
      });

      service.setItem(key, value);
      const cachEntry = service.getItem(key, true);
      
      expect(cachEntry).toEqual(value.clone());
    });

  });

  describe('getItem', () => {

    it('should return undefined for an item which does not exists in the cacheEntries Map', () => {
      const value = service.getItem("key that doesn't exist", true);
      expect(value).toBeUndefined();
    });
    
    it('should return an existing item which exists in the cacheEntries Map', () => {

      const mockEntry = mockCacheEntries[0];
      const value = service.getItem(mockEntry.key, true);
      expect(value).toEqual(mockEntry.value);
      expect(value?.url).toEqual(mockEntry.value.url);
    });
    

    it('should return an existing item which exists in the cacheEntries Map if within expiry time', () => {

      const mockEntry = mockCacheEntries[0];
      const value = service.getItem(mockEntry.key, false);
      expect(value).toEqual(mockEntry.value);
      expect(value?.url).toEqual(mockEntry.value.url);
    });

    it('should return undefined when an item exists in the cacheEntries Map but has expired', () => {

      const mockEntry = mockCacheEntries[0];
      const value = service.getItem(mockEntry.key, false);
      
      expect(value).toEqual(mockEntry.value);
      expect(value?.url).toEqual(mockEntry.value.url);

      jest.advanceTimersByTime((HttpCacheStorageService as any).MAX_CACHE_AGE_MILLISECONDS + 1000);

      const valueNowExpired = service.getItem(mockEntry.key, false);
      expect(valueNowExpired).toBeUndefined();
    });
  });

  describe('removeAll', () => {

    it('should remove all the cache entries from the cacheEntries Map', () => {

      const entrySize = service.getCacheSize();
      expect(entrySize).toEqual(mockCacheEntries.length);

      service.removeAll();
      const newEntrySize = service.getCacheSize();
      expect(newEntrySize).toEqual(0);
    });
  });

  describe('removeItem', () => {

    it('should remove the entry with the specified key from the cacheEntries Map', () => {

      const mockEntryKey = mockCacheEntries[0].key;
      const mockValue = service.getItem(mockEntryKey, true);
      expect(mockValue).toBeDefined();

      service.removeItem(mockEntryKey);
      const removedItem = service.getItem(mockEntryKey, true);
      expect(removedItem).toBeUndefined();
    });

  });

});
