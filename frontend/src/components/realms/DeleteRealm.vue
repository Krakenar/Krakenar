<script setup lang="ts">
import { TarButton, TarCheckbox, type CheckboxOptions, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Realm } from "@/types/realms";
import { deleteRealm } from "@/api/realms";
import { useRealmStore } from "@/stores/realm";

const realmStore = useRealmStore();
const { t } = useI18n();

const props = defineProps<{
  realm: Realm;
}>();

const confirm = ref<boolean>(false);
const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const expectedName = computed<string>(() => props.realm.displayName ?? props.realm.uniqueSlug);
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
  (e: "deleted", value: Realm): void;
  (e: "error", value: unknown): void;
}>();

async function doDelete(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const realm: Realm = await deleteRealm(props.realm.id);
      realmStore.deleteRealm(realm);
      emit("deleted", realm);
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
    <TarButton icon="fas fa-trash" :text="t('actions.delete')" variant="danger" data-bs-toggle="modal" data-bs-target="#delete-realm" />
    <TarModal :close="t('actions.close')" id="delete-realm" ref="modalRef" :title="t('realms.delete.title')">
      <p>
        {{ t("realms.delete.confirm") }}
        <br />
        <span class="text-danger">{{ expectedName }}</span>
      </p>
      <TarInput class="mb-3" floating id="delete-realm-name" :label="t('name')" :placeholder="t('name')" required :status="status" v-model="name" />
      <p>
        <strong class="text-danger">{{ t("realms.delete.warning") }}</strong>
      </p>
      <TarCheckbox class="text-danger" id="delete-realm-confirm" :label="t('realms.delete.agree')" required v-model="confirm" />
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isDeleting || status !== 'valid' || !confirm"
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
