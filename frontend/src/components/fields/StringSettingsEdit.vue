<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import NumberInput from "@/components/shared/NumberInput.vue";
import type { FieldType, StringSettings, UpdateFieldTypePayload } from "@/types/fields";
import { updateFieldType } from "@/api/fields/types";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  id: string;
  settings: StringSettings;
}>();

const maximumLength = ref<number>(0);
const minimumLength = ref<number>(0);
const pattern = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: FieldType): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateFieldTypePayload = {
      string: {
        minimumLength: minimumLength.value || undefined,
        maximumLength: maximumLength.value || undefined,
        pattern: pattern.value,
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
    minimumLength.value = settings.minimumLength ?? 0;
    maximumLength.value = settings.maximumLength ?? 0;
    pattern.value = settings.pattern ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="row">
      <NumberInput
        class="col"
        id="minimum-length"
        :label="t('fields.type.string.length.minimum')"
        :min="0"
        :max="maximumLength"
        step="1"
        v-model="minimumLength"
      />
      <NumberInput
        class="col"
        id="maximum-length"
        :label="t('fields.type.string.length.maximum')"
        :min="minimumLength < 0 ? 0 : minimumLength"
        step="1"
        v-model="maximumLength"
      />
    </div>
    <FormInput id="pattern" :label="t('fields.type.string.pattern')" v-model="pattern" />
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
