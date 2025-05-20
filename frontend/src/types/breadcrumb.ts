import type { RouteLocationRaw } from "vue-router";

export type Breadcrumb = {
  route: RouteLocationRaw;
  text: string;
};
