import { IResponseDataDTO } from "./IResponseDataDTO";
import { IResponseErrorDTO } from "./IResponseErrorDTO";

export interface IResponseDTO {
    apiVersion: string;
    context: string;
    id: string;
    method: string;
    data: IResponseDataDTO;
    error?: IResponseErrorDTO;
}