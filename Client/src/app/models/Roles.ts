export enum USER_ROLE {
    PLATFORM_ADMINISTRATOR = "PlatformAdministrator",
    OWNER = "Owner",
    CONTRIBUTER = "Contributer",
    READER = "Reader"
};

export const ROLES_MAP = new Map ([
    ["1", USER_ROLE.PLATFORM_ADMINISTRATOR],
    ["2", USER_ROLE.OWNER],
    ["3", USER_ROLE.CONTRIBUTER],
    ["4", USER_ROLE.READER]
]);