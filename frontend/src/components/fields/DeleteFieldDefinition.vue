<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { ContentType } from "@/types/contents";
import type { FieldDefinition } from "@/types/fields";
import { deleteFieldDefinition } from "@/api/fields/definitions";

const { t } = useI18n();

const props = defineProps<{
  contentType: ContentType;
  field: FieldDefinition;
}>();

const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const expectedName = computed<string>(() => props.field.displayName ?? props.field.uniqueName);
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
  (e: "deleted", value: ContentType): void;
  (e: "error", value: unknown): void;
}>();

async function doDelete(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const contentType: ContentType = await deleteFieldDefinition(props.contentType.id, props.field.id);
      emit("deleted", contentType);
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
    <TarButton icon="fas fa-trash" :text="t('actions.delete')" variant="danger" data-bs-toggle="modal" :data-bs-target="`#delete-field-${field.id}`" />
    <TarModal :close="t('actions.close')" :id="`delete-field-${field.id}`" ref="modalRef" :title="t('fields.definition.delete.title')">
      <p>
        {{ t("fields.definition.delete.confirm") }}
        <br />
        <span class="text-danger">{{ expectedName }}</span>
      </p>
      <TarInput floating :id="`delete-field-${field.id}-name`" :label="t('name')" :placeholder="t('name')" required :status="status" v-model="name" />
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
