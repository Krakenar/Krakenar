<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Template } from "@/types/templates";
import { deleteTemplate } from "@/api/templates";

const { t } = useI18n();

const props = defineProps<{
  template: Template;
}>();

const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const expectedName = computed<string>(() => props.template.displayName ?? props.template.uniqueName);
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
  (e: "deleted", value: Template): void;
  (e: "error", value: unknown): void;
}>();

async function doDelete(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const template: Template = await deleteTemplate(props.template.id);
      emit("deleted", template);
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
    <TarButton icon="fas fa-trash" :text="t('actions.delete')" variant="danger" data-bs-toggle="modal" data-bs-target="#delete-template" />
    <TarModal :close="t('actions.close')" id="delete-template" ref="modalRef" :title="t('templates.delete.title')">
      <p>
        {{ t("templates.delete.confirm") }}
        <br />
        <span class="text-danger">{{ expectedName }}</span>
      </p>
      <TarInput floating id="delete-template-name" :label="t('name')" :placeholder="t('name')" required :status="status" v-model="name" />
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
