import type { ValidationResult } from "logitar-validation";
import type { InjectionKey } from "vue";

export const bindFieldKey = Symbol() as InjectionKey<(id: string, options: FieldOptions) => void>;
export const unbindFieldKey = Symbol() as InjectionKey<(id: string) => void>;

export type FieldOptions = {
  focus: () => void;
  reinitialize: () => void;
  reset: () => void;
  validate: () => ValidationResult;
};

// input --> changed/dirty --> form
// input --> validated --> form
