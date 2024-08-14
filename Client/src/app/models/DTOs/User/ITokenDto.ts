import { IResponseDTO } from "../Responses/IResponseDTO";

export interface ITokenDto extends IResponseDTO<ITokenDetail> {

}

export interface ITokenDetail {
    tokenType: string;
    expiresIn?: string;
    accessToken: string;
}