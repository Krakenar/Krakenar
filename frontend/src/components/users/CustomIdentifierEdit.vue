<script setup lang="ts">
import { TarButton, TarModal, type ButtonVariant } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import CustomIdentifierAlreadyUsed from "./CustomIdentifierAlreadyUsed.vue";
import CustomIdentifierKey from "./CustomIdentifierKey.vue";
import CustomIdentifierValue from "./CustomIdentifierValue.vue";
import type { CustomIdentifier } from "@/types/custom";
import type { SaveUserIdentifierPayload, User } from "@/types/users";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { saveUserIdentifier } from "@/api/users";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    identifier?: CustomIdentifier;
    user: User;
  }>(),
  {
    id: "add-identifier",
  },
);

const customIdentifierAlreadyUsed = ref<boolean>(false);
const key = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const value = ref<string>("");

const title = computed<string>(() => t(`users.identifiers.${props.identifier ? "edit" : "add"}`));
const variant = computed<ButtonVariant>(() => (props.identifier ? "primary" : "success"));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: User): void;
}>();

function setModel(identifier?: CustomIdentifier): void {
  key.value = identifier?.key ?? props.identifier?.key ?? "";
  value.value = identifier?.value ?? props.identifier?.value ?? "";
}

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  customIdentifierAlreadyUsed.value = false;
  setModel();
  reset();
}

const { hasChanges, isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  customIdentifierAlreadyUsed.value = false;
  try {
    const payload: SaveUserIdentifierPayload = {
      value: value.value,
    };
    const user: User = await saveUserIdentifier(props.user.id, key.value, payload);
    emit("saved", user);
    onReset();
    hide();
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.CustomIdentifierAlreadyUsed)) {
      customIdentifierAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
}

watch(() => props.identifier, setModel, { deep: true, immediate: true });
</script>

<template>
  <span>
    <TarButton
      :icon="`fas fa-${identifier ? 'edit' : 'plus'}`"
      :text="t(`actions.${identifier ? 'edit' : 'add'}`)"
      :variant="variant"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="title">
      <CustomIdentifierAlreadyUsed v-model="customIdentifierAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <CustomIdentifierKey :disabled="Boolean(identifier)" :required="!identifier" v-model="key" />
        <CustomIdentifierValue required v-model="value" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="`fas fa-${identifier ? 'save' : 'plus'}`"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(`actions.${identifier ? 'save' : 'add'}`)"
          type="submit"
          :variant="variant"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </span>
</template>
