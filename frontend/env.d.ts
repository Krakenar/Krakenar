/// <reference types="vite/client" />
interface ImportMetaEnv {
  readonly VITE_APP_API_BASE_URL?: string;
  readonly VITE_APP_BASE_URL?: string;
  readonly VITE_APP_ENABLE_SWAGGER?: string;
  readonly VITE_APP_VERSION: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
