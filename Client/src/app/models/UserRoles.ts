export const UserRoles: Map<string, UserRole> = new Map([
    ["d64f869b-8978-4fdd-bd26-f3cd474d04aa", { id: "d64f869b-8978-4fdd-bd26-f3cd474d04aa", name: "Owner" }],
    ["98328385-13f9-404c-86a4-9e652cd0d693", { id: "98328385-13f9-404c-86a4-9e652cd0d693", name: "Contributer" }],
    ["d773dd1a-745f-49f8-8f19-aac2c7f86744", { id: "d773dd1a-745f-49f8-8f19-aac2c7f86744", name: "Reader" }]
]);

export interface UserRole {
    id: string;
    name: string;
};
