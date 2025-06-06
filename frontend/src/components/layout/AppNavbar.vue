<script setup lang="ts">
import { RouterLink } from "vue-router";
import { TarAvatar } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils, stringUtils } from "logitar-js";
import { computed, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

import ApiKeyIcon from "@/components/apiKeys/ApiKeyIcon.vue";
import ContentIcon from "@/components/contents/ContentIcon.vue";
import ContentTypeIcon from "@/components/contents/ContentTypeIcon.vue";
import DictionaryIcon from "@/components/dictionaries/DictionaryIcon.vue";
import FieldTypeIcon from "@/components/fields/FieldTypeIcon.vue";
import LanguageIcon from "@/components/languages/LanguageIcon.vue";
import MessageIcon from "@/components/messages/MessageIcon.vue";
import RealmIcon from "@/components/realms/RealmIcon.vue";
import RoleIcon from "@/components/roles/RoleIcon.vue";
import SenderIcon from "@/components/senders/SenderIcon.vue";
import SessionIcon from "@/components/sessions/SessionIcon.vue";
import SwitchSelect from "@/components/realms/SwitchSelect.vue";
import TemplateIcon from "@/components/templates/TemplateIcon.vue";
import TokenIcon from "@/components/tokens/TokenIcon.vue";
import UserIcon from "@/components/users/UserIcon.vue";
import UsersIcon from "@/components/users/UsersIcon.vue";
import locales from "@/resources/locales.json";
import type { CurrentUser } from "@/types/account";
import type { Locale } from "@/types/i18n";
import { useAccountStore } from "@/stores/account";
import { useI18nStore } from "@/stores/i18n";

const { combineURL } = stringUtils;
const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;

const account = useAccountStore();
const apiBaseUrl: string = import.meta.env.VITE_APP_API_BASE_URL ?? "";
const environment = import.meta.env.MODE.toLowerCase();
const i18n = useI18nStore();
const isSwaggerEnabled: boolean = window.KRAKENAR_ENABLE_SWAGGER ?? parseBoolean(import.meta.env.VITE_APP_ENABLE_SWAGGER) ?? false;
const { availableLocales, locale, t } = useI18n();

const otherLocales = computed<Locale[]>(() => {
  const otherLocales = new Set<string>(availableLocales.filter((item) => item !== locale.value));
  return orderBy(
    locales.filter(({ code }) => otherLocales.has(code)),
    "nativeName",
  );
});
const swaggerUrl = computed<string | undefined>(() => (isSwaggerEnabled ? combineURL(apiBaseUrl, "/swagger") : undefined));
const user = computed<CurrentUser | undefined>(() => account.currentUser);

watchEffect(() => {
  if (i18n.locale) {
    locale.value = i18n.locale.code;
  } else {
    const currentLocale = locales.find(({ code }) => code === locale.value);
    if (!currentLocale) {
      throw new Error(`The locale "${locale.value}" is not supported.'`);
    }
    i18n.setLocale(currentLocale);
  }
});
</script>

<template>
  <nav class="navbar navbar-expand-lg bg-body-tertiary" data-bs-theme="dark">
    <div class="container-fluid">
      <RouterLink :to="{ name: 'Home' }" class="navbar-brand">
        <img src="@/assets/img/logo.png" :alt="`${t('brand')} Logo`" height="32" />
        {{ t("brand") }}
        <span v-if="environment !== 'production'" class="badge text-bg-warning">{{ environment }}</span>
      </RouterLink>
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarSupportedContent"
        aria-controls="navbarSupportedContent"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarSupportedContent">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li v-if="swaggerUrl" class="nav-item">
            <a class="nav-link" :href="swaggerUrl" target="_blank"><font-awesome-icon icon="fas fa-vial" /> Swagger</a>
          </li>
          <template v-if="user">
            <li class="nav-item">
              <RouterLink :to="{ name: 'Dashboard' }" class="nav-link"><font-awesome-icon icon="fas fa-gauge" /> {{ t("dashboard") }}</RouterLink>
            </li>
            <li class="nav-item">
              <RouterLink :to="{ name: 'RealmList' }" class="nav-link"><RealmIcon /> {{ t("realms.title") }}</RouterLink>
            </li>
            <li class="nav-item dropdown">
              <a href="#" class="nav-link dropdown-toggle" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <font-awesome-icon icon="fas fa-users-line" /> {{ t("navbar.identity") }}
              </a>
              <ul class="dropdown-menu">
                <li>
                  <RouterLink :to="{ name: 'UserList' }" class="dropdown-item"><UsersIcon /> {{ t("users.title") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'RoleList' }" class="dropdown-item"><RoleIcon /> {{ t("roles.title") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'SessionList' }" class="dropdown-item"><SessionIcon /> {{ t("sessions.title.list") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'ApiKeyList' }" class="dropdown-item"><ApiKeyIcon /> {{ t("apiKeys.title") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'Tokens' }" class="dropdown-item"><TokenIcon /> {{ t("tokens.title") }}</RouterLink>
                </li>
              </ul>
            </li>
            <li class="nav-item dropdown">
              <a href="#" class="nav-link dropdown-toggle" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <font-awesome-icon icon="fas fa-globe" /> {{ t("navbar.localization") }}
              </a>
              <ul class="dropdown-menu">
                <li>
                  <RouterLink :to="{ name: 'LanguageList' }" class="dropdown-item"><LanguageIcon /> {{ t("languages.title") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'DictionaryList' }" class="dropdown-item"><DictionaryIcon /> {{ t("dictionaries.title") }}</RouterLink>
                </li>
              </ul>
            </li>
            <li class="nav-item dropdown">
              <a href="#" class="nav-link dropdown-toggle" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <font-awesome-icon icon="fas fa-message" /> {{ t("navbar.communication") }}
              </a>
              <ul class="dropdown-menu">
                <li>
                  <RouterLink :to="{ name: 'SenderList' }" class="dropdown-item"><SenderIcon /> {{ t("senders.title") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'TemplateList' }" class="dropdown-item"><TemplateIcon /> {{ t("templates.title") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'MessageList' }" class="dropdown-item"><MessageIcon /> {{ t("messages.title") }}</RouterLink>
                </li>
              </ul>
            </li>
            <li class="nav-item dropdown">
              <a href="#" class="nav-link dropdown-toggle" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <font-awesome-icon icon="fas fa-book" /> {{ t("navbar.content") }}
              </a>
              <ul class="dropdown-menu">
                <li>
                  <RouterLink :to="{ name: 'FieldTypeList' }" class="dropdown-item"><FieldTypeIcon /> {{ t("fields.type.title") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'ContentTypeList' }" class="dropdown-item"><ContentTypeIcon /> {{ t("contents.type.title") }}</RouterLink>
                </li>
                <li>
                  <RouterLink :to="{ name: 'ContentList' }" class="dropdown-item"><ContentIcon /> {{ t("contents.item.title") }}</RouterLink>
                </li>
              </ul>
            </li>
          </template>
        </ul>
        <ul class="navbar-nav mb-2 mb-lg-0">
          <template v-if="user">
            <li class="nav-item">
              <RouterLink :to="{ name: 'Configuration' }" class="nav-link"><font-awesome-icon icon="fas fa-gear" /> {{ t("configuration.title") }}</RouterLink>
            </li>
            <li class="nav-item">
              <SwitchSelect />
            </li>
          </template>
          <template v-if="i18n.locale">
            <li v-if="otherLocales.length > 1" class="nav-item dropdown">
              <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">{{ i18n.locale.nativeName }}</a>
              <ul class="dropdown-menu dropdown-menu-end">
                <li v-for="option in otherLocales" :key="option.code">
                  <a class="dropdown-item" href="#" @click.prevent="i18n.setLocale(option)">{{ option.nativeName }}</a>
                </li>
              </ul>
            </li>
            <li v-else-if="otherLocales.length === 1" class="nav-item">
              <a class="nav-link" href="#" @click.prevent="i18n.setLocale(otherLocales[0])">{{ otherLocales[0].nativeName }}</a>
            </li>
          </template>
          <template v-if="user">
            <li class="nav-item d-block d-lg-none">
              <RouterLink class="nav-link" :to="{ name: 'Profile' }">
                <TarAvatar :display-name="user.displayName" :email-address="user.emailAddress" :size="24" :url="user.pictureUrl" />
                {{ user.displayName }}
              </RouterLink>
            </li>
            <li class="nav-item d-block d-lg-none">
              <RouterLink class="nav-link" :to="{ name: 'SignOut' }">
                <font-awesome-icon icon="fas fa-arrow-right-from-bracket" /> {{ t("users.signOut.title.page") }}
              </RouterLink>
            </li>
            <li class="nav-item dropdown d-none d-lg-block">
              <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <TarAvatar :display-name="user.displayName" :email-address="user.emailAddress" :size="24" :url="user.pictureUrl" />
              </a>
              <ul class="dropdown-menu dropdown-menu-end">
                <li>
                  <RouterLink class="dropdown-item" :to="{ name: 'Profile' }"><UserIcon /> {{ user.displayName }}</RouterLink>
                </li>
                <li>
                  <RouterLink class="dropdown-item" :to="{ name: 'SignOut' }">
                    <font-awesome-icon icon="fas fa-arrow-right-from-bracket" /> {{ t("users.signOut.title.page") }}
                  </RouterLink>
                </li>
              </ul>
            </li>
          </template>
          <template v-else>
            <li class="nav-item">
              <RouterLink :to="{ name: 'SignIn' }" class="nav-link">
                <font-awesome-icon icon="fas fa-arrow-right-to-bracket" /> {{ t("users.signIn.title") }}
              </RouterLink>
            </li>
          </template>
        </ul>
      </div>
    </div>
  </nav>
</template>
