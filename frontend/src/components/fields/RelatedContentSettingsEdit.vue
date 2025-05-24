<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import ContentTypeFormSelect from "@/components/contents/ContentTypeFormSelect.vue";
import MultipleCheckbox from "./MultipleCheckbox.vue";
import type { FieldType, RelatedContentSettings, UpdateFieldTypePayload } from "@/types/fields";
import { updateFieldType } from "@/api/fields/types";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  id: string;
  settings: RelatedContentSettings;
}>();

const contentTypeId = ref<string>("");
const isMultiple = ref<boolean>(false);

const hasChanges = computed<boolean>(() => props.settings.isMultiple !== isMultiple.value || hasFormChanges.value);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: FieldType): void;
}>();

const { hasChanges: hasFormChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateFieldTypePayload = {
      relatedContent: {
        contentTypeId: contentTypeId.value,
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
    contentTypeId.value = settings.contentTypeId;
    isMultiple.value = settings.isMultiple;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <MultipleCheckbox v-model="isMultiple" />
    <ContentTypeFormSelect required v-model="contentTypeId" />
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
