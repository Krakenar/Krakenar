<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import SubjectInput from "./SubjectInput.vue";
import claimTypes from "@/resources/claimTypes.json";
import type { Claim, ValidatedToken } from "@/types/tokens";

const { d, t } = useI18n();

defineProps<{
  token: ValidatedToken;
}>();

const claimTypeNames = computed<Map<string, string>>(() => {
  const map = new Map<string, string>();
  claimTypes.forEach(({ id, name }) => map.set(id, name));
  return map;
});

function formatClaimType(type: string): string {
  const name: string | undefined = claimTypeNames.value.get(type);
  if (name) {
    return name;
  }

  const index = type.indexOf("#");
  return index < 0 ? type : type.substring(index + 1);
}

function formatDateTime(claim: Claim): string {
  return d(new Date(Number(claim.value) * 1000), "medium");
}
function isDateTimeClaim(claim: Claim): boolean {
  return Boolean(claim.type && formatClaimType(claim.type).toLowerCase().startsWith("integer") && !Number.isNaN(claim.value));
}
</script>

<template>
  <div>
    <h3>{{ t("tokens.claims.validated") }}</h3>
    <div class="row">
      <SubjectInput class="col" disabled :model-value="token.subject ?? ''" />
      <EmailAddressInput class="col" disabled :model-value="token.email?.address">
        <template #append>
          <div class="input-group-text">
            <TarCheckbox disabled id="email-address-verified" :label="t('users.email.verified.label')" :model-value="token.email?.isVerified" />
          </div>
        </template>
      </EmailAddressInput>
    </div>
    <h5>{{ t("tokens.claims.other") }}</h5>
    <table v-if="token.claims.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("tokens.claims.name") }}</th>
          <th scope="col">{{ t("tokens.claims.value") }}</th>
          <th scope="col">{{ t("tokens.claims.type.label") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(claim, index) in token.claims" :key="index">
          <td>{{ claim.name }}</td>
          <td>
            <template v-if="isDateTimeClaim(claim)">
              {{ formatDateTime(claim) }}
              <sub>({{ claim.value }})</sub>
            </template>
            <template v-else>{{ claim.value }}</template>
          </td>
          <td>{{ claim.type ? formatClaimType(claim.type) : "—" }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
