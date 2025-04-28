import type { RuleExecutionResult, ValidationResult, ValidationRuleSet } from "logitar-validation";
import { computed, getCurrentInstance, inject, provide, ref, unref, type ComponentInternalInstance } from "vue";

import type { FieldActions, FieldEvents, FieldOptions, FieldValues, FormContainer, FormField } from "@/types/forms";
import validator, { isValidationFailure } from "@/validation";
import { bindFieldKey, unbindFieldKey } from "@/types/forms";

export function useField(id: string, options?: FieldOptions): FormField {
  options ??= {};

  const vm: ComponentInternalInstance | null = getCurrentInstance();

  const bindField: ((id: string, options: FieldActions, initialValue?: string) => FieldEvents) | undefined = inject(bindFieldKey);
  const unbindField: ((id: string) => void) | undefined = inject(unbindFieldKey);

  const initialValue = ref<string>(options.initialValue ?? "");
  const validationResult = ref<ValidationResult>();
  const value = ref<string>(options.initialValue ?? "");

  const errors = computed<RuleExecutionResult[]>(() => (validationResult.value ? Object.values(validationResult.value.rules).filter(isValidationFailure) : []));
  const isValid = computed<boolean | undefined>(() => validationResult.value?.isValid);
  const name = computed<string>(() => options.name?.trim() || id);
  const rules = computed<ValidationRuleSet | undefined>(() => unref(options.rules));

  let events: FieldEvents | undefined;

  function emit(name: string, value: unknown): void {
    if (vm) {
      vm.emit(name, value);
    }
  }

  function focus(): void {
    if (options?.focus) {
      options.focus();
    }
  }

  function handleChange(e: Event, skipValidation?: boolean): void {
    value.value = (e.target as HTMLInputElement)?.value ?? "";
    events?.updated(id, value.value);
    emit("update:model-value", value.value);

    if (!skipValidation) {
      validate();
    }
  }

  function reinitialize(): void {
    validationResult.value = undefined;
    initialValue.value = value.value;
    events?.reinitialized(id, initialValue.value);
  }

  function reset(): void {
    validationResult.value = undefined;
    value.value = initialValue.value;
    events?.reset(id, value.value);
    emit("update:model-value", value.value);
  }

  function validate(): ValidationResult {
    if (!rules.value) {
      return { isValid: true, rules: {}, context: {} };
    }
    validationResult.value = validator.validate(name.value, value.value, rules.value);
    events?.validated(id, validationResult.value);
    emit("validated", validationResult.value);
    return validationResult.value;
  }

  const actions: FieldActions = { focus, reinitialize, reset, validate };
  if (bindField) {
    events = bindField(id, actions, initialValue.value);
  }

  return { errors, isValid, value, bindField, focus, handleChange, reinitialize, reset, unbindField, validate };
}

export function useForm(): FormContainer {
  const fields = ref<Map<string, FieldActions>>(new Map());
  const isSubmitting = ref<boolean>(false);
  const validationResults = ref<Map<string, ValidationResult>>(new Map());
  const values = ref<Map<string, FieldValues>>(new Map());

  const hasChanges = computed<boolean>(() => Object.values([...values.value.values()]).some(({ hasChanged }) => hasChanged));
  const isValid = computed<boolean>(() => Object.values([...validationResults.value.values()]).every((result) => result.isValid));

  function onFieldReinitialize(id: string, value: string): void {
    validationResults.value.delete(id);
    values.value.set(id, { initial: value, current: value, hasChanged: false } as FieldValues);
  }
  function onFieldReset(id: string, value: string): void {
    validationResults.value.delete(id);
    values.value.set(id, { initial: value, current: value, hasChanged: false } as FieldValues);
  }
  function onFieldUpdate(id: string, value: string): void {
    const fieldValues: FieldValues = {
      initial: values.value.get(id)?.initial ?? "",
      current: value,
      hasChanged: false,
    };
    fieldValues.hasChanged = fieldValues.initial !== fieldValues.current;
    values.value.set(id, fieldValues);
  }
  function onFieldValidation(id: string, result: ValidationResult): void {
    validationResults.value.set(id, result);
  }
  const fieldEvents: FieldEvents = {
    reinitialized: onFieldReinitialize,
    reset: onFieldReset,
    updated: onFieldUpdate,
    validated: onFieldValidation,
  };
  function bindField(id: string, actions: FieldActions, initialValue?: string): FieldEvents {
    fields.value.set(id, actions);
    validationResults.value.delete(id);
    values.value.set(id, {
      initial: initialValue ?? "",
      current: initialValue ?? "",
      hasChanged: false,
    } as FieldValues);
    return fieldEvents;
  }
  provide(bindFieldKey, bindField);

  function unbindField(id: string): void {
    fields.value.delete(id);
    validationResults.value.delete(id);
  }
  provide(unbindFieldKey, unbindField);

  function handleSubmit(submitCallback?: () => void): void {
    if (!isSubmitting.value) {
      try {
        isSubmitting.value = true;
        validate();
        if (isValid.value) {
          if (submitCallback) {
            submitCallback();
          }
          reinitialize();
        } else {
          const ids: string[] = [...validationResults.value.entries()].filter(([, value]) => !value.isValid).map(([id]) => id);
          if (ids.length) {
            const field: FieldActions | undefined = fields.value.get(ids[0]);
            field?.focus();
          }
        }
      } finally {
        isSubmitting.value = false;
      }
    }
  }

  function reinitialize(): void {
    Object.values([...fields.value.values()]).forEach((field) => field.reinitialize());
  }

  function reset(): void {
    Object.values([...fields.value.values()]).forEach((field) => field.reset());
  }

  function validate(): Map<string, ValidationResult> {
    [...fields.value.entries()].forEach(([id, field]) => {
      const result: ValidationResult = field.validate();
      validationResults.value.set(id, result);
    });
    return validationResults.value;
  }

  return { hasChanges, isSubmitting, isValid, handleSubmit, reinitialize, reset, validate };
} // TODO(fpion): when modelValue is changed, for example when the form is submitted, field values do not reflect what is returned by the backend.
