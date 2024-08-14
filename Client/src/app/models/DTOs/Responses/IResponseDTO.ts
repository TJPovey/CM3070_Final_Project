import { IResponseDataDTO } from "./IResponseDataDTO";
import { IResponseErrorDTO } from "./IResponseErrorDTO";

export interface IResponseDTO<T> {
    apiVersion: string;
    context: string;
    id: string;
    method: string;
    data: IResponseDataDTO<T>;
    error?: IResponseErrorDTO;
}