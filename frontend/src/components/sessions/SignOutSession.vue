<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import type { Session } from "@/types/sessions";
import { handleErrorKey } from "@/inject/App";
import { signOutSession } from "@/api/sessions";
import { useAccountStore } from "@/stores/account";
import { useRealmStore } from "@/stores/realm";
import { useToastStore } from "@/stores/toast";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const realm = useRealmStore();
const router = useRouter();
const toasts = useToastStore();
const { d, t } = useI18n();

const props = defineProps<{
  session: Session;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const isCurrentSession = computed<boolean>(() => props.session.id === account.currentUser?.sessionId);

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

const emit = defineEmits<{
  (e: "signed-out", value: Session): void;
}>();
async function executeSignOut(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const session = await signOutSession(props.session.id);
      if (isCurrentSession.value) {
        account.signOut();
        realm.exit();
        router.push({ name: "SignIn" });
      } else {
        emit("signed-out", session);
        toasts.success("sessions.signedOut");
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
      hide();
    }
  }
}
function onSignOut(): void {
  if (isCurrentSession.value) {
    show();
  } else {
    executeSignOut();
  }
}
</script>

<template>
  <span>
    <TarButton
      :disabled="!session.isActive"
      icon="fas fa-arrow-right-from-bracket"
      :loading="isLoading"
      :status="t('loading')"
      :text="t('sessions.signOut.submit')"
      :variant="isCurrentSession ? 'danger' : 'warning'"
      @click="onSignOut"
    />
    <TarModal :close="t('actions.close')" id="sign-out-session" ref="modalRef" :title="t('sessions.signOut.title')">
      <p>
        {{ t("sessions.signOut.confirm") }}
        <br />
        <span class="text-danger">{{ d(session.updatedOn, "medium") }}</span>
      </p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="hide" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-arrow-right-from-bracket"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('sessions.signOut.submit')"
          variant="danger"
          @click="executeSignOut"
        />
      </template>
    </TarModal>
  </span>
</template>
