import { ITokenPostDto } from "../models/DTOs/User/ITokenPostDto";
import { IUserPostDto } from "../models/DTOs/User/IUserPostDto";

export class UserDtoExtensions {

    static ToUserPostDto(
        firstName: string, 
        lastName: string, 
        username: string, 
        email: string, 
        password: string): IUserPostDto {

        const dto = <IUserPostDto>{
            firstName,
            lastName,
            username,
            email,
            password
        };

        return dto;
    }

    static ToTokenPostDto(username: string, password: string): ITokenPostDto {

        const dto = <ITokenPostDto>{
            username,
            password
        };

        return dto;
    }

}