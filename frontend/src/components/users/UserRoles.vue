<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import RoleSelect from "@/components/roles/RoleSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Role } from "@/types/roles";
import type { UpdateUserPayload, User } from "@/types/users";
import { updateUser } from "@/api/users";

const { t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const isSubmitting = ref<boolean>(false);
const role = ref<Role>();

const excludedIds = computed<string[]>(() => props.user.roles.map(({ id }) => id));

const emit = defineEmits<{
  (e: "added", value: User): void;
  (e: "error", value: unknown): void;
  (e: "removed", value: User): void;
}>();

async function remove(role: Role): Promise<void> {
  if (!isSubmitting.value) {
    isSubmitting.value = true;
    try {
      const payload: UpdateUserPayload = {
        customAttributes: [],
        roles: [
          {
            role: role.id,
            action: "Remove",
          },
        ],
      };
      const user: User = await updateUser(props.user.id, payload);
      emit("removed", user);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isSubmitting.value = false;
    }
  }
}

async function submit(): Promise<void> {
  if (!isSubmitting.value && role.value) {
    isSubmitting.value = true;
    try {
      const payload: UpdateUserPayload = {
        customAttributes: [],
        roles: [
          {
            role: role.value.id,
            action: "Add",
          },
        ],
      };
      const user: User = await updateUser(props.user.id, payload);
      role.value = undefined;
      emit("added", user);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isSubmitting.value = false;
    }
  }
}
</script>

<template>
  <div>
    <form @submit.prevent="submit">
      <RoleSelect class="mb-3" :exclude="excludedIds" :model-value="role?.id" @selected="role = $event">
        <template #append>
          <TarButton
            :disabled="isSubmitting || !role"
            icon="fas fa-plus"
            :loading="isSubmitting"
            :status="t('loading')"
            :text="t('actions.add')"
            type="submit"
            variant="success"
          />
        </template>
      </RoleSelect>
    </form>
    <table v-if="user.roles.length" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("roles.sort.options.UniqueName") }}</th>
          <th scope="col">{{ t("roles.sort.options.DisplayName") }}</th>
          <th scope="col">{{ t("roles.sort.options.UpdatedOn") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="role in user.roles" :key="role.id">
          <td>
            <RouterLink target="_blank" :to="{ name: 'RoleEdit', params: { id: role.id } }">
              <font-awesome-icon icon="fas fa-edit" /> {{ role.uniqueName }}
            </RouterLink>
          </td>
          <td>
            <template v-if="role.displayName">{{ role.displayName }}</template>
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
          <td><StatusBlock :actor="role.updatedBy" :date="role.updatedOn" /></td>
          <td>
            <TarButton
              :disabled="isSubmitting"
              icon="fas fa-times"
              :loading="isSubmitting"
              :status="t('loading')"
              :text="t('actions.remove')"
              variant="danger"
              @click="remove(role)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("users.roles.empty") }}</p>
  </div>
</template>
