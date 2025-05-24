<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import MultipleCheckbox from "./MultipleCheckbox.vue";
import SelectOptionList from "./SelectOptionList.vue";
import type { FieldType, SelectOption, SelectSettings, UpdateFieldTypePayload } from "@/types/fields";
import { updateFieldType } from "@/api/fields/types";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  id: string;
  settings: SelectSettings;
}>();

const isMultiple = ref<boolean>(false);
const options = ref<SelectOption[]>([]);

const hasChanges = computed<boolean>(
  () => props.settings.isMultiple !== isMultiple.value || JSON.stringify(props.settings.options) !== JSON.stringify(options.value),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: FieldType): void;
}>();

const { isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateFieldTypePayload = {
      select: {
        options: options.value,
        isMultiple: isMultiple.value,
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
    options.value = settings.options.map((option) => ({ ...option }));
    isMultiple.value = settings.isMultiple;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <MultipleCheckbox v-model="isMultiple" />
    <SelectOptionList v-model="options" />
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
