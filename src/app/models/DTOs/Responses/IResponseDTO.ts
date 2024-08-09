import { IResponseDataDTO } from "./IResponseDataDTO";
import { IResponseErrorDTO } from "./IResponseErrorDTO";

export interface IResponseDTO<T> {
    apiVersion: string;
    context: string;
    id: string;
    method: string;
    data: T & IResponseDataDTO;
    error?: IResponseErrorDTO;
}