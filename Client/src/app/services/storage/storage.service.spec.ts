import { StorageService } from './storage.service';

class MockStorageService extends StorageService {

  getItem<T>(key: string, ignoreExpiry: boolean): T | undefined {
    // Mock implementation
    return undefined;
  }

  setItem<T>(key: string, value: T): void {
    // Mock implementation
  }

  removeItem(key: string): void {
    // Mock implementation
  }

  removeAll(): void {
    // Mock implementation
  }

  getCacheSize(): number {
    return 0;
  }
}

describe('StorageService', () => {
  let storageService: MockStorageService;

  beforeEach(() => {
    storageService = new MockStorageService();
  });

  it('should generate prefix correctly', () => {
    const prefix = (MockStorageService as any).generatePrefix();
    expect(prefix).toBe(`${(MockStorageService as any).APP_PREFIX}-${(MockStorageService as any).APP_VERSION}`);
  });

  it('should generate key correctly', () => {
    const testKey = "testKey";
    const key = (storageService as any).generateKey(testKey);
    expect(key).toBe(`${(MockStorageService as any).APP_PREFIX}-${(MockStorageService as any).APP_VERSION}-${testKey}`);
  });

  it('should check if an entry is expired', () => {
    const entryTime = Date.now() - (MockStorageService as any).MAX_CACHE_AGE_MILLISECONDS - 1000;
    const isExpired = (storageService as any).isExpired(entryTime);
    expect(isExpired).toBe(true);
  });
});
