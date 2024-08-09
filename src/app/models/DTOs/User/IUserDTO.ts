import { USER_ROLE } from "../../Roles";
import { IResponseDTO } from "../Responses/IResponseDTO";

export interface IUserDTO extends IResponseDTO<IUserDetail> {

}

export interface IUserDetail {
    username: string;
    email: string;
    name: IName;
    propertyCollection: IUserSiteListItem[];
}

export interface IName {
    firstName: string;
    lastName: string;
    fullName: string;
}

export interface IUserSiteListItem {
    id: string;
    title: string;
    roleId: string;
    roleName: USER_ROLE;
}