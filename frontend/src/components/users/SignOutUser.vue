<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import type { User } from "@/types/users";
import { formatUser } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { signOutUser } from "@/api/users";
import { useAccountStore } from "@/stores/account";
import { useToastStore } from "@/stores/toast";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const isCurrentUser = computed<boolean>(() => props.user.id === account.currentUser?.id);

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

const emit = defineEmits<{
  (e: "signed-out", value: User): void;
}>();
async function onSignOut(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const user = await signOutUser(props.user.id);
      if (isCurrentUser.value) {
        account.signOut();
        router.push({ name: "SignIn" });
      } else {
        emit("signed-out", user);
        toasts.success("users.signOut.success");
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
      hide();
    }
  }
}
</script>

<template>
  <span>
    <TarButton
      :disabled="isLoading"
      icon="fas fa-arrow-right-from-bracket"
      :loading="isLoading"
      :status="t('loading')"
      :text="t('users.signOut.submit')"
      variant="danger"
      @click="show"
    />
    <TarModal :close="t('actions.close')" id="sign-out-user" ref="modalRef" :title="t('users.signOut.title.modal')">
      <p>
        {{ t("users.signOut.confirm") }}
        <br />
        <template v-if="isCurrentUser">{{ t("users.signOut.self") }}</template>
        <template v-else-if="user.realm">{{ t("users.signOut.other.realm") }}</template>
        <template v-else>{{ t("users.signOut.other.admin") }}</template>
        <br />
        <span class="text-danger">{{ formatUser(user) }}</span>
      </p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="hide" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-arrow-right-from-bracket"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('users.signOut.submit')"
          variant="danger"
          @click="onSignOut"
        />
      </template>
    </TarModal>
  </span>
</template>
