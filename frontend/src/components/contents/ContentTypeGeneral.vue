<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import InvariantCheckbox from "./InvariantCheckbox.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ContentType, UpdateContentTypePayload } from "@/types/contents";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateContentType } from "@/api/contents";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  contentType: ContentType;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const isInvariant = ref<boolean>(false);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(() => props.contentType.isInvariant !== isInvariant.value || hasFormChanges.value);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: ContentType): void;
}>();

const { hasChanges: hasFormChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  uniqueNameAlreadyUsed.value = false;
  try {
    const payload: UpdateContentTypePayload = {
      isInvariant: props.contentType.isInvariant !== isInvariant.value ? isInvariant.value : undefined,
      uniqueName: props.contentType.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
      displayName: (props.contentType.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
      description: (props.contentType.description ?? "") !== description.value ? { value: description.value } : undefined,
    };
    const contentType: ContentType = await updateContentType(props.contentType.id, payload);
    emit("updated", contentType);
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
}

watch(
  () => props.contentType,
  (contentType) => {
    isInvariant.value = contentType.isInvariant;
    uniqueName.value = contentType.uniqueName;
    displayName.value = contentType.displayName ?? "";
    description.value = contentType.description ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="handleSubmit(submit)">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <InvariantCheckbox v-model="isInvariant" />
      <div class="row">
        <UniqueNameInput class="col" identifier required v-model="uniqueName" />
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
