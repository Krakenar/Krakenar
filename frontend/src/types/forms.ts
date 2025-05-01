import type { ComputedRef, InjectionKey, MaybeRef, Ref } from "vue";
import type { RuleExecutionResult, ValidationResult, ValidationRuleSet } from "logitar-validation";

export const bindFieldKey = Symbol() as InjectionKey<(id: string, options: FieldActions, initialValue?: string) => FieldEvents>;
export const unbindFieldKey = Symbol() as InjectionKey<(id: string) => void>;

export type FieldActions = {
  focus: () => void;
  reinitialize: () => void;
  reset: () => void;
  validate: () => ValidationResult;
};

export type FieldEvents = {
  reinitialized: (id: string, value: string) => void;
  reset: (id: string, value: string) => void;
  updated: (id: string, value: string) => void;
  validated: (id: string, result: ValidationResult) => void;
};

export type FieldOptions = {
  focus?: () => void | null;
  initialValue?: string | null;
  name?: string | null;
  placeholders?: Placeholders;
  rules?: MaybeRef<ValidationRuleSet>;
};

export type FieldValues = {
  initial: string;
  current: string;
  hasChanged: boolean;
};

export type FormContainer = {
  hasChanges: ComputedRef<boolean>;
  isSubmitting: Ref<boolean, boolean>;
  isValid: ComputedRef<boolean>;
  handleSubmit: (submitCallback?: () => void) => void;
  reinitialize: () => void;
  reset: () => void;
  validate: () => Map<string, ValidationResult>;
};

export type FormField = {
  errors: ComputedRef<RuleExecutionResult[]>;
  isValid: ComputedRef<boolean | undefined>;
  value: Ref<string, string>;
  bindField: ((id: string, options: FieldActions, initialValue?: string) => FieldEvents) | undefined;
  focus: () => void;
  handleChange: (e: Event, shouldValidate: boolean) => void;
  reinitialize: () => void;
  reset: () => void;
  unbindField: ((id: string) => void) | undefined;
  validate: () => ValidationResult;
};

export type Placeholders = Record<string, unknown>;
