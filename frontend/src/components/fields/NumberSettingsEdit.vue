<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import NumberInput from "@/components/shared/NumberInput.vue";
import type { FieldType, NumberSettings, UpdateFieldTypePayload } from "@/types/fields";
import { updateFieldType } from "@/api/fields";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  id: string;
  settings: NumberSettings;
}>();

const maximumValue = ref<number>(0);
const minimumValue = ref<number>(0);
const step = ref<number>(0);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: FieldType): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateFieldTypePayload = {
      number: {
        minimumValue: minimumValue.value,
        maximumValue: maximumValue.value,
        step: step.value || undefined,
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
    minimumValue.value = settings.minimumValue ?? 0;
    maximumValue.value = settings.maximumValue ?? 0;
    step.value = settings.step ?? 0;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="row">
      <NumberInput class="col" id="minimum-value" :label="t('fields.type.number.value.minimum')" :max="maximumValue" step="1" v-model="minimumValue" />
      <NumberInput class="col" id="maximum-value" :label="t('fields.type.number.value.maximum')" :min="minimumValue" step="1" v-model="maximumValue" />
    </div>
    <NumberInput class="col" id="step" :label="t('fields.type.number.step')" :min="0" :max="maximumValue < 0 ? undefined : maximumValue" v-model="step" />
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
