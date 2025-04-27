import { Validator, rules } from "logitar-validation";

const validator = new Validator({ treatWarningsAsErrors: true });

validator.setRule("email", rules.email);
validator.setRule("maximumLength", rules.maximumLength);
validator.setRule("minimumLength", rules.minimumLength);
validator.setRule("required", rules.required);

export default validator;
