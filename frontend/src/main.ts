import "bootstrap/dist/css/bootstrap.min.css";
import "logitar-vue3-ui/style.css"
import persistedState from "pinia-plugin-persistedstate";
import { createApp } from "vue";
import { createPinia } from "pinia";

import "./assets/styles/main.css";
import App from "./App.vue";
import fontAwesome from "./fontAwesome";
import i18n from "./i18n";
import jsonViewer from "./jsonViewer";
import router from "./router";

declare global {
  interface Window {
    KRAKENAR_BASE_URL?: string;
    KRAKENAR_ENABLE_SWAGGER?: boolean;
    KRAKENAR_VERSION?: string;
  }
}

const app = createApp(App);

const pinia = createPinia();
pinia.use(persistedState);

app.use(fontAwesome);
app.use(i18n);
app.use(jsonViewer);
app.use(pinia);
app.use(router);

app.mount("#krakenar");
