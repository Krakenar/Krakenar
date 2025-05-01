<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import InvalidCredentials from "@/components/users/InvalidCredentials.vue";
import PasswordInput from "@/components/users/PasswordInput.vue";
import UsernameInput from "@/components/users/UsernameInput.vue";
import type { CurrentUser, SignInAccountPayload } from "@/types/account";
import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api";
import { handleErrorKey } from "@/inject/App";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";
import { useForm } from "@/forms";

const INVALID_CREDENTIAL_CODES = [
  ErrorCodes.IncorrectUserPassword,
  ErrorCodes.InvalidCredentials,
  ErrorCodes.UserHasNoPassword,
  ErrorCodes.UserIsDisabled,
  ErrorCodes.UserNotFound,
];

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const invalidCredentials = ref<boolean>(false);
const password = ref<string>("");
const passwordRef = ref<InstanceType<typeof PasswordInput> | null>(null);
const username = ref<string>("");

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
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
    if (failure.status === StatusCodes.BadRequest) {
      const problemDetails = failure.data as ProblemDetails;
      if (problemDetails.error && INVALID_CREDENTIAL_CODES.includes(problemDetails.error.code as ErrorCodes)) {
        invalidCredentials.value = true;
        password.value = "";
        passwordRef.value?.focus();
        return;
      }
    }
    handleError(e);
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("users.signIn.title") }}</h1>
    <InvalidCredentials v-model="invalidCredentials" />
    <form @submit.prevent="handleSubmit(submit)">
      <UsernameInput required v-model="username" />
      <PasswordInput ref="passwordRef" required v-model="password" />
      <div class="mb-3">
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          icon="fas fa-arrow-right-to-bracket"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('users.signIn.submit')"
          type="submit"
        />
      </div>
    </form>
  </main>
</template>
