import { describe, expect, it } from "vitest";
import { nanoid } from "nanoid";

import { isExpired } from "../apiKeys";
import type { ApiKey } from "@/types/apiKeys";
import type { Actor } from "@/types/actor";

const actor: Actor = {
  type: "User",
  id: nanoid(),
  isDeleted: false,
  displayName: "Administrator",
};
const now: string = new Date().toISOString();
const apiKey: ApiKey = {
  id: nanoid(),
  version: 0,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  name: "Test",
  customAttributes: [],
  roles: [],
};

describe("isExpired", () => {
  it.concurrent("should return false when the API key does not expire", () => {
    expect(apiKey.expiresOn).toBeUndefined();
    expect(isExpired(apiKey)).toBe(false);
  });

  it.concurrent("should return false when the API key is not expired", () => {
    apiKey.expiresOn = new Date().toISOString();
    const moment: Date = new Date();
    moment.setFullYear(moment.getFullYear() - 1);
    expect(isExpired(apiKey, moment)).toBe(false);
  });

  it.concurrent("should return true when the API key is expired", () => {
    apiKey.expiresOn = new Date().toISOString();
    expect(isExpired(apiKey)).toBe(true);
  });
});
