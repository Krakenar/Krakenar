import { createRouter, createWebHistory } from "vue-router";

import HomeView from "./views/HomeView.vue";

import { useAccountStore } from "./stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.VITE_APP_BASE_URL ?? import.meta.env.BASE_URL),
  routes: [
    {
      name: "Home",
      path: "/",
      component: HomeView,
    },
    // Account
    {
      name: "SignIn",
      path: "/sign-in",
      component: () => import("./views/account/SignInView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/sign-out",
      component: () => import("./views/account/SignOutView.vue"),
    },
    // Configuration
    {
      name: "Configuration",
      path: "/configuration",
      component: () => import("./views/ConfigurationEdit.vue"),
    },
    // API Keys
    {
      name: "ApiKeyList",
      path: "/api-keys",
      component: () => import("./views/apiKeys/ApiKeyList.vue"),
    },
    {
      name: "ApiKeyEdit",
      path: "/api-keys/:id",
      component: () => import("./views/apiKeys/ApiKeyEdit.vue"),
    },
    // Content Types
    {
      name: "ContentTypeList",
      path: "/contents/types",
      component: () => import("./views/contents/ContentTypeList.vue"),
    },
    {
      name: "ContentTypeEdit",
      path: "/contents/types/:id",
      component: () => import("./views/contents/ContentTypeEdit.vue"),
    },
    // Dictionaries
    {
      name: "DictionaryList",
      path: "/dictionaries",
      component: () => import("./views/dictionaries/DictionaryList.vue"),
    },
    {
      name: "DictionaryEdit",
      path: "/dictionaries/:id",
      component: () => import("./views/dictionaries/DictionaryEdit.vue"),
    },
    // Field Types
    {
      name: "FieldTypeList",
      path: "/fields/types",
      component: () => import("./views/fields/FieldTypeList.vue"),
    },
    {
      name: "FieldTypeEdit",
      path: "/fields/types/:id",
      component: () => import("./views/fields/FieldTypeEdit.vue"),
    },
    // Languages
    {
      name: "LanguageList",
      path: "/languages",
      component: () => import("./views/languages/LanguageList.vue"),
    },
    {
      name: "LanguageEdit",
      path: "/languages/:id",
      component: () => import("./views/languages/LanguageEdit.vue"),
    },
    // Messages
    {
      name: "MessageList",
      path: "/messages",
      component: () => import("./views/messages/MessageList.vue"),
    },
    {
      name: "MessageView",
      path: "/messages/:id",
      component: () => import("./views/messages/MessageView.vue"),
    },
    // Realms
    {
      name: "RealmList",
      path: "/realms",
      component: () => import("./views/realms/RealmList.vue"),
    },
    {
      name: "RealmEdit",
      path: "/realms/:id",
      component: () => import("./views/realms/RealmEdit.vue"),
    },
    // Roles
    {
      name: "RoleList",
      path: "/roles",
      component: () => import("./views/roles/RoleList.vue"),
    },
    {
      name: "RoleEdit",
      path: "/roles/:id",
      component: () => import("./views/roles/RoleEdit.vue"),
    },
    // Senders
    {
      name: "SenderList",
      path: "/senders",
      component: () => import("./views/senders/SenderList.vue"),
    },
    {
      name: "SenderEdit",
      path: "/senders/:id",
      component: () => import("./views/senders/SenderEdit.vue"),
    },
    // Sessions
    {
      name: "SessionList",
      path: "/sessions",
      component: () => import("./views/sessions/SessionList.vue"),
    },
    {
      name: "SessionEdit",
      path: "/sessions/:id",
      component: () => import("./views/sessions/SessionEdit.vue"),
    },
    // Templates
    {
      name: "TemplateList",
      path: "/templates",
      component: () => import("./views/templates/TemplateList.vue"),
    },
    {
      name: "TemplateEdit",
      path: "/templates/:id",
      component: () => import("./views/templates/TemplateEdit.vue"),
    },
    // Users
    {
      name: "UserList",
      path: "/users",
      component: () => import("./views/users/UserList.vue"),
    },
    {
      name: "UserEdit",
      path: "/users/:id",
      component: () => import("./views/users/UserEdit.vue"),
    },
    // Tokens
    {
      name: "Tokens",
      path: "/tokens",
      component: () => import("./views/TokenView.vue"),
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import("./views/NotFound.vue"),
      meta: { isPublic: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const account = useAccountStore();
  if (!to.meta.isPublic && !account.currentUser) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
});

export default router;
