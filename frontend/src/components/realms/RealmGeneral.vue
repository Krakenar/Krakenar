<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import UniqueSlugAlreadyUsed from "./UniqueSlugAlreadyUsed.vue";
import UniqueSlugInput from "./UniqueSlugInput.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Realm, UpdateRealmPayload } from "@/types/realms";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateRealm } from "@/api/realms";

const { t } = useI18n();

const props = defineProps<{
  realm: Realm;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const isLoading = ref<boolean>(false);
const uniqueSlug = ref<string>("");
const uniqueSlugAlreadyUsed = ref<boolean>(false);
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Realm): void;
}>();

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueSlugAlreadyUsed.value = false;
    try {
      const payload: UpdateRealmPayload = {
        uniqueSlug: props.realm.uniqueSlug !== uniqueSlug.value ? uniqueSlug.value : undefined,
        displayName: (props.realm.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
        description: (props.realm.description ?? "") !== description.value ? { value: description.value } : undefined,
        url: (props.realm.url ?? "") !== url.value ? { value: url.value } : undefined,
        customAttributes: [],
      };
      const realm: Realm = await updateRealm(props.realm.id, payload);
      emit("updated", realm);
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueSlugAlreadyUsed)) {
        uniqueSlugAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.realm,
  (realm) => {
    displayName.value = realm.displayName ?? "";
    uniqueSlug.value = realm.uniqueSlug;
    description.value = realm.description ?? "";
    url.value = realm.url ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="submit">
      <UniqueSlugAlreadyUsed v-model="uniqueSlugAlreadyUsed" />
      <div class="row">
        <DisplayNameInput class="col" v-model="displayName" />
        <UniqueSlugInput class="col" :name-value="displayName" v-model="uniqueSlug" />
      </div>
      <UrlInput described-by="url-help" id="url" :label="t('realms.url.label')" v-model="url">
        <template #after>
          <div id="url-help" class="form-text">{{ t("realms.url.help") }}</div>
        </template>
      </UrlInput>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <TarButton :disabled="isLoading" icon="fas fa-save" :loading="isLoading" :status="t('loading')" :text="t('actions.save')" type="submit" />
      </div>
    </form>
  </div>
</template>
