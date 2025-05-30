import { beforeEach, describe, it, expect } from "vitest";
import { nanoid } from "nanoid";
import { setActivePinia, createPinia } from "pinia";

import type { Realm } from "@/types/realms";
import { useRealmStore } from "./../realm";
import type { Actor } from "@/types/actor";

const actor: Actor = {
  type: "User",
  id: nanoid(),
  isDeleted: false,
  displayName: "Francis Pion",
};
const now: string = new Date().toISOString();
const realm: Realm = {
  id: nanoid(),
  version: 0,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  uniqueSlug: "new-world",
  secretChangedBy: actor,
  secretChangedOn: now,
  uniqueNameSettings: {},
  passwordSettings: {
    requiredLength: 8,
    requiredUniqueChars: 8,
    requireDigit: true,
    requireLowercase: true,
    requireUppercase: true,
    requireNonAlphanumeric: true,
    hashingStrategy: "PBKDF2",
  },
  requireUniqueEmail: true,
  requireConfirmedAccount: true,
  customAttributes: [],
};

describe("realmStore", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it.concurrent("should be initially empty", () => {
    const realmStore = useRealmStore();
    expect(realmStore.currentRealm).toBeUndefined();
    expect(realmStore.realms).toEqual([]);
  });

  it.concurrent("should enter a realm", () => {
    const realmStore = useRealmStore();
    realmStore.enter(realm);
    expect(realmStore.currentRealm).toBeDefined();
    expect(realmStore.currentRealm?.id).toBe(realm.id);
  });

  it.concurrent("should exit a realm", () => {
    const realmStore = useRealmStore();
    realmStore.enter(realm);
    expect(realmStore.currentRealm).toBeDefined();
    realmStore.exit();
    expect(realmStore.currentRealm).toBeUndefined();
  });
});
