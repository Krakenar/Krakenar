<script setup lang="ts">
import { TarAccordion, TarAccordionItem, TarCheckbox } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import MessageLocale from "./MessageLocale.vue";
import MessageVariables from "./MessageVariables.vue";
import TemplateIcon from "@/components/templates/TemplateIcon.vue";
import type { Message } from "@/types/messages";
import type { Template } from "@/types/templates";
import { formatTemplate } from "@/helpers/format";

const { t } = useI18n();

const props = defineProps<{
  message: Message;
}>();

const viewAsHtml = ref<boolean>(props.message.body.type === "text/html");

const template = computed<Template>(() => props.message.template);
</script>

<template>
  <section>
    <table class="table table-striped">
      <tbody>
        <tr>
          <th scope="row">{{ t("templates.select.label") }}</th>
          <td>
            <RouterLink v-if="template.version > 0" :to="{ name: 'TemplateEdit', params: { id: template.id } }" target="_blank">
              <TemplateIcon /> {{ formatTemplate(template) }}
            </RouterLink>
            <span v-else class="text-muted"><TemplateIcon /> {{ formatTemplate(template) }}</span>
          </td>
        </tr>
        <tr>
          <th scope="row">{{ t("templates.subject.label") }}</th>
          <td>{{ message.subject }}</td>
        </tr>
      </tbody>
    </table>
    <p>
      <i class="text-warning"><font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("messages.contents.warning") }}</i>
    </p>
    <TarAccordion>
      <TarAccordionItem active id="body" :title="t(`templates.content.type.options.${message.body.type}`)">
        <div class="mb-3">
          <TarCheckbox id="view-as-html" :label="t('messages.contents.viewAsHtml')" switch v-model="viewAsHtml" />
        </div>
        <div v-if="viewAsHtml" v-html="message.body.text"></div>
        <div v-else v-text="message.body.text"></div>
      </TarAccordionItem>
      <TarAccordionItem id="localization" :title="t('messages.contents.localization')">
        <div class="mb-3">
          <TarCheckbox disabled id="ignore-user-locale" :label="t('messages.ignoreUserLocale')" :model-value="message.ignoreUserLocale" switch />
        </div>
        <MessageLocale :locale="message.locale" />
      </TarAccordionItem>
      <TarAccordionItem id="variables" :title="t('messages.variables.title')">
        <MessageVariables v-if="message.variables.length > 0" :variables="message.variables" />
        <p v-else>{{ t("messages.variables.empty") }}</p>
      </TarAccordionItem>
    </TarAccordion>
  </section>
</template>
