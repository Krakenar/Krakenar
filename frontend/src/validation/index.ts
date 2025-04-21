import { Validator, rules } from "logitar-validation";

const validator = new Validator();

validator.setRule("required", rules.required);

export default validator;
