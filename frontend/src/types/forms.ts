import type { InjectionKey, MaybeRef } from "vue";
import type { ValidationResult, ValidationRuleSet } from "logitar-validation";

export const bindFieldKey = Symbol() as InjectionKey<(id: string, options: FieldActions) => void>;
export const unbindFieldKey = Symbol() as InjectionKey<(id: string) => void>;

export type FieldActions = {
  focus: () => void;
  reinitialize: () => void;
  reset: () => void;
  validate: () => ValidationResult;
};

export type FieldOptions = {
  focus?: () => void | null;
  initialValue?: string | null;
  name?: string | null;
  rules?: MaybeRef<ValidationRuleSet>;
};

// input --> changed/dirty --> form
// input --> validated --> form
