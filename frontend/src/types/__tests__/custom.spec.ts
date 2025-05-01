import { describe, expect, it, test } from "vitest";

import { CustomAttributeState, type CustomAttribute } from "../custom";

describe("CustomAttributeState", () => {
  it.concurrent("should construct a default state", () => {
    const state = new CustomAttributeState();
    expect(state.getInitialKey()).toBe("");
    expect(state.getInitialValue()).toBe("");
    expect(state.getCurrentKey()).toBe("");
    expect(state.getCurrentValue()).toBe("");
    expect(state.status).toBe("added");
  });

  it.concurrent("should construct a state from arguments", () => {
    const key: string = "ProfileCompletedOn";
    const value: string = new Date().toISOString();
    const state = new CustomAttributeState(key, value);
    expect(state.getInitialKey()).toBe(key);
    expect(state.getInitialValue()).toBe(value);
    expect(state.getCurrentKey()).toBe(key);
    expect(state.getCurrentValue()).toBe(value);
    expect(state.status).toBeUndefined();
  });

  it.concurrent("should construct a state from custom attribute", () => {
    const customAttribute: CustomAttribute = { key: "ProfileCompletedOn", value: new Date().toISOString() };
    const state = new CustomAttributeState(customAttribute);
    expect(state.getInitialKey()).toBe(customAttribute.key);
    expect(state.getInitialValue()).toBe(customAttribute.value);
    expect(state.getCurrentKey()).toBe(customAttribute.key);
    expect(state.getCurrentValue()).toBe(customAttribute.value);
    expect(state.status).toBeUndefined();
  });

  it.concurrent("should set the current key", () => {
    const state = new CustomAttributeState("key", "value");
    const key: string = "new_key";
    state.setCurrentKey(key);
    expect(state.getCurrentKey()).toBe(key);
    expect(state.status).toBe("updated");
  });

  it.concurrent("should set the current value", () => {
    const state = new CustomAttributeState("key", "value");
    const value: string = "new_value";
    state.setCurrentValue(value);
    expect(state.getCurrentValue()).toBe(value);
    expect(state.status).toBe("updated");
  });

  it.concurrent("should mark the attribute as removed", () => {
    const state = new CustomAttributeState("key", "value");
    state.remove();
    expect(state.status).toBe("removed");

    state.remove();
    expect(state.status).toBe("removed");
  });

  it.concurrent("should un-mark the attribute as removed", () => {
    const state = new CustomAttributeState("key", "value");
    state.restore();
    expect(state.status).toBeUndefined();

    state.remove();
    expect(state.status).toBe("removed");

    state.restore();
    expect(state.status).toBeUndefined();
  });

  it.concurrent("should not mark an added attribute as removed", () => {
    const state = new CustomAttributeState();
    state.remove();
    expect(state.status).toBe("added");
  });
});
