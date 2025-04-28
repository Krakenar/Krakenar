<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import StatusDetail from "@/components/shared/StatusDetail.vue";
import UserSummary from "@/components/users/UserSummary.vue";
import type { Configuration } from "@/types/configuration";
import type { User } from "@/types/users";
import { formatUser } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readUser } from "@/api/users";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const user = ref<User>();

function setMetadata(updated: User): void {
  if (user.value) {
    user.value.version = updated.version;
    user.value.updatedBy = updated.updatedBy;
    user.value.updatedOn = updated.updatedOn;
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    user.value = await readUser(id);
    if (!user.value.realm) {
      configuration.value = await readConfiguration();
    }
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="user">
      <h1>{{ formatUser(user) }}</h1>
      <StatusDetail :aggregate="user" />
      <UserSummary :user="user" />
      <TarTabs>
        <TarTab active id="authentication" :title="t('users.authentication')">
          <p>TODO(fpion): implement</p>
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
