<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Configuration } from "@/types/configuration";
import type { FieldType, UpdateFieldTypePayload } from "@/types/fields";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateFieldType } from "@/api/fields";
import { useForm } from "@/forms";
import type { UniqueNameSettings } from "@/types/settings";

const { t } = useI18n();

const props = defineProps<{
  configuration?: Configuration;
  fieldType: FieldType;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const uniqueNameSettings = computed<UniqueNameSettings | undefined>(() => props.fieldType.realm?.uniqueNameSettings ?? props.configuration?.uniqueNameSettings);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: FieldType): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  uniqueNameAlreadyUsed.value = false;
  try {
    const payload: UpdateFieldTypePayload = {
      uniqueName: props.fieldType.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
      displayName: (props.fieldType.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
      description: (props.fieldType.description ?? "") !== description.value ? { value: description.value } : undefined,
    };
    const fieldType: FieldType = await updateFieldType(props.fieldType.id, payload);
    emit("updated", fieldType);
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
}

watch(
  () => props.fieldType,
  (fieldType) => {
    uniqueName.value = fieldType.uniqueName;
    displayName.value = fieldType.displayName ?? "";
    description.value = fieldType.description ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="handleSubmit(submit)">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <UniqueNameInput class="col" required :settings="uniqueNameSettings" v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <DescriptionTextarea v-model="description" />
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
  </div>
</template>
