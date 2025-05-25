<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DeleteContent from "./DeleteContent.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import PublishButton from "./PublishButton.vue";
import PublishedInfo from "./PublishedInfo.vue";
import StatusInfo from "@/components/shared/StatusInfo.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import UnpublishButton from "./UnpublishButton.vue";
import type { Configuration } from "@/types/configuration";
import type { Content, ContentLocale, SaveContentLocalePayload } from "@/types/contents";
import type { FieldValuePayload } from "@/types/fields";
import type { UniqueNameSettings } from "@/types/settings";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { saveContentLocale } from "@/api/contents/items";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  configuration?: Configuration;
  content: Content;
  locale: ContentLocale;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const fieldValues = ref<FieldValuePayload[]>([]);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const isTypeInvariant = computed<boolean>(() => props.content.contentType.isInvariant ?? false);
const uniqueNameSettings = computed<UniqueNameSettings | undefined>(
  () => props.content.contentType.realm?.uniqueNameSettings ?? props.configuration?.uniqueNameSettings,
);

const emit = defineEmits<{
  (e: "deleted", value: Content): void;
  (e: "error", value: unknown): void;
  (e: "published", value: Content): void;
  (e: "saved", value: Content): void;
  (e: "unpublished", value: Content): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  uniqueNameAlreadyUsed.value = false;
  try {
    const payload: SaveContentLocalePayload = {
      uniqueName: uniqueName.value,
      displayName: displayName.value,
      description: description.value,
      fieldValues: fieldValues.value,
    };
    const content: Content = await saveContentLocale(props.content.id, payload, props.locale.language?.id);
    emit("saved", content);
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.ContentUniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
}

watch(
  () => props.locale,
  (locale) => {
    uniqueName.value = locale.uniqueName;
    displayName.value = locale.displayName ?? "";
    description.value = locale.description ?? "";
    fieldValues.value = locale.fieldValues.map(({ id, value }) => ({
      field: id,
      value,
    }));
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <p v-if="!isTypeInvariant">
      <StatusInfo :actor="locale.createdBy" :date="locale.createdOn" format="status.createdOn" />
      <br />
      <StatusInfo :actor="locale.updatedBy" :date="locale.updatedOn" format="status.updatedOn" />
      <br />
      <PublishedInfo :locale="locale" />
    </p>
    <div v-if="!isTypeInvariant" class="mb-3">
      <DeleteContent
        v-if="locale.language"
        class="me-1"
        :content="content"
        :language="locale.language ?? undefined"
        @deleted="emit('deleted', $event)"
        @error="emit('error', $event)"
      />
      <PublishButton
        class="mx-1"
        :content="content"
        :language="locale.language ?? undefined"
        @deleted="emit('deleted', $event)"
        @published="emit('published', $event)"
      />
      <UnpublishButton
        class="ms-1"
        :content="content"
        :language="locale.language ?? undefined"
        @deleted="emit('deleted', $event)"
        @unpublished="emit('unpublished', $event)"
      />
    </div>
    <form @submit.prevent="handleSubmit(submit)">
      <div class="row">
        <UniqueNameInput class="col" required :settings="uniqueNameSettings" v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <DescriptionTextarea v-model="description" />
      <!-- TODO(fpion): FieldValues -->
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
