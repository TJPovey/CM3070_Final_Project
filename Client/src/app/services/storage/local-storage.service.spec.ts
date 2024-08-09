import { TestBed } from '@angular/core/testing';
import { LocalStorageService } from './local-storage.service';

const FIXED_SYSTEM_TIME = '2023-04-08T00:00:00Z';

const mockCacheEntries = [
  {
    key: "key 1", 
    value: "value 1"
  },
  {
    key: "key 2",
    value: 2
  },
  {
    key: "key 3",
    value: new Map([
      ["map key 1", "map value 1"],
      ["map key 2", "map value 2"],
      ["map key 3", "map value 3"],
    ])
  },
  {
    key: "key 4",
    value: {
      param1: "param value 1",
      param2: "param value 2",
    }
  }
];


describe('LocalStorageService', () => {

  let service: LocalStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LocalStorageService);

    jest.useFakeTimers();
    jest.setSystemTime(Date.parse(FIXED_SYSTEM_TIME));

    mockCacheEntries.forEach(({key, value}) => {
      service.setItem(key, value);
    });
  });

  afterEach(() => {
    jest.useRealTimers();
    service.removeAll();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('toStorageEntry', () => {

    it('should return an object where the dataType is the name of the contructor name or type name', () => {
      const value = "Mock Value";
      const entry = (service as any).toStorageEntry(value);
      expect(entry.dataType).toBe(((value as unknown as object).constructor?.name || typeof(value)));
    });

    it('should return an object where the entryTime equals the time the object was created', () => {
      const value = "Mock Value";
      const entry = (service as any).toStorageEntry(value);
      expect(entry.entryTime).toEqual(new Date(Date.parse(FIXED_SYSTEM_TIME)).getTime());
    });

    it('should return an object where the value parameter is equal to the input value', () => {
      const value = "Mock Value";
      const entry = (service as any).toStorageEntry(value);
      expect(entry.value).toEqual(value);
    });

    it('should return an object where the value parameter is an array of map entries when the input type is a Map', () => {
      const value = new Map([
        ["key 1", "value 1"],
        ["key 2", "value 2"],
        ["key 3", "value 3"],
      ]);
      const mapEntries = Array.from(value.entries());
      const entry = (service as any).toStorageEntry(value);
      expect(entry.value).toEqual(mapEntries);
    });
  });

  describe("setItem", () => {

    it('should store an item in localStorage', () => {
      
      const key = "mock key 1";
      const value = "mock value 1"

      service.setItem(key, value);
      const cachEntry = service.getItem(key, true);

      expect(cachEntry).toEqual(value);
    });

  });

  describe('getItem', () => {

    it('should return undefined for an item which does not exists in localStorage', () => {
      const value = service.getItem("key that doesn't exist", true);
      expect(value).toBeUndefined();
    });
    
    it('should return an existing item which exists in localStorage', () => {
      const mockEntry = mockCacheEntries[0];
      const value = service.getItem(mockEntry.key, true);
      expect(value).toEqual(mockEntry.value);
    });
    

    it('should return an existing item which exists in localStorage if within expiry time', () => {
      const mockEntry = mockCacheEntries[0];
      const value = service.getItem(mockEntry.key, false);
      expect(value).toEqual(mockEntry.value);
    });

    it('should return an entry where the value is a map when an object with value map has been added to localStorage', () => {
      const mockEntry = mockCacheEntries[2];
      const value = service.getItem(mockEntry.key, false);
      expect((value?.constructor as any).name).toEqual(Map.name);
      expect(value).toEqual(mockEntry.value);
    });

    it('should return undefined when an item exists in localStorage but has expired', () => {

      const mockEntry = mockCacheEntries[0];
      const value = service.getItem(mockEntry.key, false);
      
      expect(value).toEqual(mockEntry.value);

      jest.advanceTimersByTime((LocalStorageService as any).MAX_CACHE_AGE_MILLISECONDS + 1000);

      const valueNowExpired = service.getItem(mockEntry.key, false);
      expect(valueNowExpired).toBeUndefined();
    });
  });

  describe('removeItem', () => {

    it('should remove the entry with the specified key from localStorage', () => {

      const mockEntryKey = mockCacheEntries[0].key;
      const mockValue = service.getItem(mockEntryKey, true);
      expect(mockValue).toBeDefined();

      service.removeItem(mockEntryKey);
      const removedItem = service.getItem(mockEntryKey, true);
      expect(removedItem).toBeUndefined();
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

  describe("getAllKeysWithPrefix", () => {

    it("should return an array", () => {
      const entries: string[] = (service as any).getAllKeysWithPrefix();
      expect(Array.isArray(entries)).toBe(true);
    });

    it("should have length equal to mockCacheEntries length", () => {
      const entries: string[] = (service as any).getAllKeysWithPrefix();
      expect(entries.length).toBe(mockCacheEntries.length);
    });

    it("should have the app generated prefix for every entry", () => {
      const entries: string[] = (service as any).getAllKeysWithPrefix();
      const prefix: string = (LocalStorageService as any).generatePrefix(); 
      entries.forEach(entry => expect(entry.indexOf(`${prefix}`)).toBe(0));
    });

  });

  describe("reviver", () => {

    it("should return the value from a localStorage entry", () => {

      const mockEntry = mockCacheEntries[0];
      const storageEntry = (service as any).reviver(mockEntry.key, mockEntry.value);

      expect(storageEntry).toEqual(mockEntry.value);
    });

    it("should return a Map when the input is a Map", () => {

      const mockEntry = mockCacheEntries[2];
      const storageEntry = (service as any).reviver(mockEntry.key, mockEntry.value);

      expect(storageEntry).toEqual(mockEntry.value);
    });

  });

});
