export type CurrentUser = {
  id: string;
  sessionId: string;
  displayName: string;
  emailAddress?: string | null;
  pictureUrl?: string | null;
};

export type SignInAccountPayload = {
  username: string;
  password: string;
};
