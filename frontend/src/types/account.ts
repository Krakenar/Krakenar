export type CurrentUser = {
  id: string;
  sessionId: string;
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type SignInAccountPayload = {
  username: string;
  password: string;
};
