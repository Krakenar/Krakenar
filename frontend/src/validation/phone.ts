import parsePhoneNumber, { PhoneNumber, type CountryCode } from "libphonenumber-js";
import type { RuleExecutionOutcome, ValidationRule } from "logitar-validation";

const phone: ValidationRule = (value: unknown, args: unknown): RuleExecutionOutcome => {
  if (typeof value !== "string") {
    return { severity: "error", message: "{{name}} must be a string." };
  }

  if (value.length > 0) {
    let countryCode: CountryCode = "CA";
    switch (typeof args) {
      case "string":
        countryCode = args as CountryCode;
        break;
      case "undefined":
        break;
      default:
        return { severity: "warning", message: "The arguments should be a two-letter country code (ex.: CA)." };
    }

    let phoneNumber: PhoneNumber | undefined;
    try {
      phoneNumber = parsePhoneNumber(value, countryCode);
    } catch (_) {}

    if (!phoneNumber || !phoneNumber.isValid()) {
      return { severity: "error", message: "{{name}} must be a valid phone number." };
    }
  }

  return { severity: "information" };
};

export default phone;
