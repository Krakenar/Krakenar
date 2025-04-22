<script setup lang="ts">
import { RouterLink } from "vue-router";
import { TarAvatar } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils, stringUtils } from "logitar-js";
import { computed, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

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
const isSwaggerEnabled: boolean = parseBoolean(import.meta.env.VITE_APP_ENABLE_SWAGGER) ?? false;
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
            <a class="nav-link" :href="swaggerUrl" target="_blank"> <font-awesome-icon icon="fas fa-vial" /> Swagger</a>
          </li>
          <template v-if="user">
            <li class="nav-item">
              <RouterLink :to="{ name: 'Configuration' }" class="nav-link"><font-awesome-icon icon="fas fa-gear" /> {{ t("configuration.title") }}</RouterLink>
            </li>
            <li class="nav-item">
              <RouterLink :to="{ name: 'RealmList' }" class="nav-link"><font-awesome-icon icon="fas fa-chess-rook" /> {{ t("realms.title") }}</RouterLink>
            </li>
            <!--<li class="nav-item">
              <RouterLink :to="{ name: 'RoleList' }" class="nav-link"><font-awesome-icon icon="fas fa-users-gear" /> {{ t("roles.list") }}</RouterLink>
            </li>-->
            <!--<li class="nav-item">
              <RouterLink :to="{ name: 'ApiKeyList' }" class="nav-link"><font-awesome-icon icon="fas fa-key" /> {{ t("apiKeys.list") }}</RouterLink>
            </li>-->
            <!--<li class="nav-item">
              <RouterLink :to="{ name: 'Tokens' }" class="nav-link"> <font-awesome-icon icon="fas fa-id-card" /> {{ t("tokens.title") }} </RouterLink>
            </li>-->
            <!--<li class="nav-item">
              <RouterLink :to="{ name: 'LanguageList' }" class="nav-link"><font-awesome-icon icon="fas fa-language" /> {{ t("languages.list") }}</RouterLink>
            </li>-->
          </template>
        </ul>

        <ul class="navbar-nav mb-2 mb-lg-0">
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
            <!--<li class="nav-item d-block d-lg-none">
              <RouterLink class="nav-link" :to="{ name: 'Profile' }">
                <TarAvatar :display-name="user.displayName" :email-address="user.emailAddress" :size="24" :url="user.pictureUrl" />
                {{ user.displayName }}
              </RouterLink>
            </li>-->
            <li class="nav-item d-block d-lg-none">
              <RouterLink class="nav-link" :to="{ name: 'SignOut' }">
                <font-awesome-icon icon="fas fa-arrow-right-from-bracket" /> {{ t("users.signOut") }}
              </RouterLink>
            </li>
            <li class="nav-item dropdown d-none d-lg-block">
              <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <TarAvatar :display-name="user.displayName" :email-address="user.emailAddress" :size="24" :url="user.pictureUrl" />
              </a>
              <ul class="dropdown-menu dropdown-menu-end">
                <!--<li>
                  <RouterLink class="dropdown-item" :to="{ name: 'Profile' }"><font-awesome-icon icon="fas fa-user" /> {{ user.displayName }}</RouterLink>
                </li>-->
                <li>
                  <RouterLink class="dropdown-item" :to="{ name: 'SignOut' }">
                    <font-awesome-icon icon="fas fa-arrow-right-from-bracket" /> {{ t("users.signOut") }}
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
