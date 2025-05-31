import type { Session } from "./sessions";

export type Statistics = {
  realmCount: number;
  userCount: number;
  sessionCount: number;
  messageCount: number;
  contentCount: number;
  sessions: Session[];
};
