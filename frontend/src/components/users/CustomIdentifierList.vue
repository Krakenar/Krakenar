<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import CustomIdentifierEdit from "./CustomIdentifierEdit.vue";
import type { CustomIdentifier } from "@/types/custom";
import type { User } from "@/types/users";
import { removeUserIdentifier } from "@/api/users";

const { t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const isRemoving = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "removed", value: User): void;
  (e: "saved", value: User): void;
}>();

async function remove(identifier: CustomIdentifier): Promise<void> {
  if (!isRemoving.value) {
    isRemoving.value = true;
    try {
      const user: User = await removeUserIdentifier(props.user.id, identifier.key);
      emit("removed", user);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isRemoving.value = false;
    }
  }
}
</script>

<template>
  <div>
    <div class="mb-3">
      <CustomIdentifierEdit :user="user" @error="$emit('error', $event)" @saved="$emit('saved', $event)" />
    </div>
    <table v-if="user.customIdentifiers.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("users.identifiers.key") }}</th>
          <th scope="col">{{ t("users.identifiers.value") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(identifier, index) in user.customIdentifiers" :key="index">
          <td>{{ identifier.key }}</td>
          <td>{{ identifier.value }}</td>
          <td>
            <CustomIdentifierEdit
              class="me-1"
              :id="`edit-identifier-${index}`"
              :identifier="identifier"
              :user="user"
              @error="$emit('error', $event)"
              @saved="$emit('saved', $event)"
            />
            <TarButton
              class="ms-1"
              :disabled="isRemoving"
              icon="fas fa-times"
              :loading="isRemoving"
              :status="t('loading')"
              :text="t('actions.remove')"
              variant="danger"
              @click="remove(identifier)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("users.identifiers.empty") }}</p>
  </div>
</template>
