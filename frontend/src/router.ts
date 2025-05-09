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
