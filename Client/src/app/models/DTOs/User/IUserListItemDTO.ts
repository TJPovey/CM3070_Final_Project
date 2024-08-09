import { USER_ROLE } from "../../Roles";

export interface IUserListItemDTO {
    fullName: string;
    id: string;
    roleId: string;
    roleName: USER_ROLE;
}