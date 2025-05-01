import type { RuleExecutionOutcome } from "logitar-validation";
import { describe, expect, test } from "vitest";

import phone from "../phone";

describe("phone", () => {
  test.each([undefined, null, {}, [], true, 0, 0n])("should return invalid when the value is not a string", (value) => {
    const outcome = phone(value) as RuleExecutionOutcome;
    expect(outcome.severity).toBe("error");
    expect(outcome.message).toBe("{{name}} must be a string.");
  });

  test.each([null, {}, [], true, 0, 0n])("should return warning when the args are not valid", (args) => {
    const outcome = phone("test@example.com", args) as RuleExecutionOutcome;
    expect(outcome.severity).toBe("warning");
    expect(outcome.message).toBe("The arguments should be a two-letter country code (ex.: CA).");
  });

  test.each(["    ", "+1514845"])("should return invalid when the value is not a valid phone number", (value) => {
    const outcome = phone(value) as RuleExecutionOutcome;
    expect(outcome.severity).toBe("error");
    expect(outcome.message).toBe("{{name}} must be a valid phone number.");
  });

  test.each(["", "+15148454636", "5148454636", "1 514-845-4636", "(514) 845-4636"])("should return valid when the value is a valid phone number", (value) => {
    const outcome = phone(value) as RuleExecutionOutcome;
    expect(outcome.severity).toBe("information");
    expect(outcome.message).toBeUndefined();
  });
});
