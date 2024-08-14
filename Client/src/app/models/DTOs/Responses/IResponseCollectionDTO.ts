import { IResponseDataDTO } from "./IResponseDataDTO";

export interface IResponseCollectionDTO<T> extends IResponseDataDTO<T> {
    currentItemCount: number;
    items: T[];
    pageIndex: number;
    startIndex: number;
    totalItems: number;
    totalPages: number;
}