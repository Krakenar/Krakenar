<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";
import type { DateTimeSettings, FieldType, UpdateFieldTypePayload } from "@/types/fields";
import { updateFieldType } from "@/api/fields/types";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  id: string;
  settings: DateTimeSettings;
}>();

const maximumValue = ref<Date>();
const minimumValue = ref<Date>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: FieldType): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateFieldTypePayload = {
      dateTime: {
        minimumValue: minimumValue.value?.toISOString(),
        maximumValue: maximumValue.value?.toISOString(),
      },
    };
    const fieldType: FieldType = await updateFieldType(props.id, payload);
    emit("updated", fieldType);
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  () => props.settings,
  (settings) => {
    minimumValue.value = typeof settings.minimumValue === "string" ? new Date(settings.minimumValue) : undefined;
    maximumValue.value = typeof settings.maximumValue === "string" ? new Date(settings.maximumValue) : undefined;
  },
  { deep: true, immediate: true },
);

const minimumDate = new Date(1900, 0, 1);
const maximumDate = new Date(9999, 11, 31, 23, 59, 59);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="row">
      <DateTimeInput
        class="col"
        id="minimum-value"
        :label="t('fields.type.dateTime.value.minimum')"
        :min="minimumDate"
        :max="maximumValue ?? maximumDate"
        v-model="minimumValue"
      />
      <DateTimeInput
        class="col"
        id="maximum-value"
        :label="t('fields.type.dateTime.value.maximum')"
        :min="minimumValue ?? minimumDate"
        :max="maximumDate"
        v-model="maximumValue"
      />
    </div>
    <div class="mb-3">
      <TarButton
        :disabled="isSubmitting || !hasChanges"
        icon="fas fa-save"
        :loading="isSubmitting"
        :status="t('loading')"
        :text="t('actions.save')"
        type="submit"
      />
    </div>
  </form>
</template>
