import type { Session } from "./sessions";

export type RealmStatistics = {
  userCount: number;
  sessionCount: number;
  messageCount: number;
  contentCount: number;
  sessions: Session[];
};

export type Statistics = {
  realmCount: number;
  userCount: number;
  sessionCount: number;
  messageCount: number;
  sessions: Session[];
  realm?: RealmStatistics | null;
};
