<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Sender } from "@/types/senders";
import { deleteSender } from "@/api/senders";

const { t } = useI18n();

const props = defineProps<{
  sender: Sender;
}>();

const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const expectedName = computed<string>(() => {
  if (props.sender.displayName) {
    return props.sender.displayName;
  } else if (props.sender.email) {
    return props.sender.email.address;
  } else if (props.sender.phone) {
    return props.sender.phone.e164Formatted;
  }
  return "";
});
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
  (e: "deleted", value: Sender): void;
  (e: "error", value: unknown): void;
}>();

async function doDelete(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const sender: Sender = await deleteSender(props.sender.id);
      emit("deleted", sender);
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
    <TarButton icon="fas fa-trash" :text="t('actions.delete')" variant="danger" data-bs-toggle="modal" data-bs-target="#delete-sender" />
    <TarModal :close="t('actions.close')" id="delete-sender" ref="modalRef" :title="t('senders.delete.title')">
      <p>
        {{ t("senders.delete.confirm") }}
        <br />
        <span class="text-danger">{{ expectedName }}</span>
      </p>
      <TarInput floating id="delete-sender-name" :label="t('name')" :placeholder="t('name')" required :status="status" v-model="name" />
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
