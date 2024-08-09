export interface IStorageEntry<T> {
    dataType: string;
    entryTime: number;
    value: T;
}