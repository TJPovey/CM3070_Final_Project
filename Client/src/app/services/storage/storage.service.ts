
export abstract class StorageService {

  protected static readonly APP_PREFIX = "SNAG_IT";
  protected static readonly APP_VERSION = "1.0";
  protected static readonly MAX_CACHE_AGE_MILLISECONDS = 60 * 60 * 1000; // 1 Hour

  protected static generatePrefix = () =>
  `${StorageService.APP_PREFIX}-${StorageService.APP_VERSION}`;

  protected generateKey = (key: string) => 
    `${StorageService.generatePrefix()}-${key}`;

  protected isExpired = (entryTime: number) =>
    (Date.now() - entryTime) > StorageService.MAX_CACHE_AGE_MILLISECONDS;

  abstract getItem<T>(key: string, ignoreExpiry: boolean): T | undefined;
  abstract setItem<T>(key: string, value: T): void;
  abstract removeItem(key: string): void;
  abstract removeAll(): void;
  abstract getCacheSize(): number;
}
