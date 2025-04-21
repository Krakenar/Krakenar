<script setup lang="ts">
import { TarAlert, TarButton } from "logitar-vue3-ui";
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import InvalidCredentials from "@/components/users/InvalidCredentials.vue";
import PasswordInput from "@/components/users/PasswordInput.vue";
import UsernameInput from "@/components/users/UsernameInput.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { CurrentUser, SignInAccountPayload } from "@/types/account";
import { handleErrorKey } from "@/inject/App";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";

const INVALID_CREDENTIAL_CODES = ["IncorrectUserPassword", "InvalidCredentials", "UserHasNoPassword", "UserIsDisabled", "UserNotFound"];

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const invalidCredentials = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const password = ref<string>("");
const passwordRef = ref<InstanceType<typeof PasswordInput> | null>(null);
const username = ref<string>("");

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    invalidCredentials.value = false;
    try {
      const payload: SignInAccountPayload = {
        username: username.value,
        password: password.value,
      };
      const currentUser: CurrentUser = await signIn(payload);
      account.signIn(currentUser);
      const redirect: string | undefined = route.query.redirect?.toString();
      router.push(redirect ?? { name: "Home" });
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === 400) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error && INVALID_CREDENTIAL_CODES.includes(problemDetails.error.code)) {
          invalidCredentials.value = true;
          password.value = "";
          passwordRef.value?.focus();
          return;
        }
      }
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("users.signIn.title") }}</h1>
    <InvalidCredentials v-model="invalidCredentials" />
    <form @submit.prevent="submit">
      <UsernameInput v-model="username" />
      <PasswordInput ref="passwordRef" v-model="password" />
      <div class="mb-3">
        <TarButton
          :disabled="isLoading"
          icon="fas fa-arrow-right-to-bracket"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('users.signIn.submit')"
          type="submit"
        />
      </div>
    </form>
  </main>
</template>
