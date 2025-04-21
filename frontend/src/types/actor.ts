export type Actor = {
  realmId?: string;
  type: ActorType;
  id: string;
  isDeleted: boolean;
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type ActorType = "System" | "User" | "ApiKey";
