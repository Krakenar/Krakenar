<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Content, ContentLocale } from "@/types/contents";
import type { Language } from "@/types/languages";
import { deleteContent } from "@/api/contents/items";

const { t } = useI18n();

const props = defineProps<{
  content: Content;
  language?: Language;
}>();

const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const expectedName = computed<string>(() => {
  const locale: ContentLocale | undefined =
    (props.language ? props.content.locales.find((locale) => locale.language?.id === props.language?.id) : undefined) ?? props.content.invariant;
  return locale.displayName ?? locale.uniqueName;
});
const id = computed<string>(() => (props.language ? `delete-content-${props.language.id}` : "delete-content"));
const status = computed<InputStatus | undefined>(() => {
  if (!name.value) {
    return undefined;
  }
  return name.value === expectedName.value ? "valid" : "invalid";
});

function cancel(): void {
  name.value = "";
  hide();
}

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "deleted", value: Content): void;
  (e: "error", value: unknown): void;
}>();

async function doDelete(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const content: Content = await deleteContent(props.content.id, props.language?.id);
      emit("deleted", content);
      hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isDeleting.value = false;
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-trash" :text="t('actions.delete')" variant="danger" data-bs-toggle="modal" :data-bs-target="`#${id}`" />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t('contents.item.delete.title')">
      <p>
        {{ t("contents.item.delete.confirm") }}
        <br />
        <span class="text-danger">{{ expectedName }}</span>
      </p>
      <TarInput floating :id="`${id}-name`" :label="t('name')" :placeholder="t('name')" required :status="status" v-model="name" />
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isDeleting || status !== 'valid'"
          icon="fas fa-trash"
          :loading="isDeleting"
          :status="t('loading')"
          :text="t('actions.delete')"
          variant="danger"
          @click="doDelete"
        />
      </template>
    </TarModal>
  </span>
</template>
