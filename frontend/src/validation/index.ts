import { Validator, rules, type RuleExecutionResult } from "logitar-validation";

import phone from "./phone";

const validator = new Validator({ treatWarningsAsErrors: true });

validator.setRule("allowedCharacters", rules.allowedCharacters);
validator.setRule("confirm", rules.confirm);
validator.setRule("containsDigits", rules.containsDigits);
validator.setRule("containsLowercase", rules.containsLowercase);
validator.setRule("containsNonAlphanumeric", rules.containsNonAlphanumeric);
validator.setRule("containsUppercase", rules.containsUppercase);
validator.setRule("email", rules.email);
validator.setRule("identifier", rules.identifier);
validator.setRule("maximumLength", rules.maximumLength);
validator.setRule("maximumValue", rules.maximumValue);
validator.setRule("minimumLength", rules.minimumLength);
validator.setRule("minimumValue", rules.minimumValue);
validator.setRule("pattern", rules.pattern);
validator.setRule("phone", phone);
validator.setRule("required", rules.required);
validator.setRule("slug", rules.slug);
validator.setRule("uniqueCharacters", rules.uniqueCharacters);
validator.setRule("url", rules.url);

export default validator;

export function isValidationFailure(result: RuleExecutionResult): boolean {
  switch (result.severity) {
    case "warning":
    case "error":
    case "critical":
      return true;
  }
  return false;
}
