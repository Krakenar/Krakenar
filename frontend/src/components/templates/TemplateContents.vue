<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import ContentTextarea from "./ContentTextarea.vue";
import ContentTypeFormSelect from "./ContentTypeFormSelect.vue";
import SubjectInput from "./SubjectInput.vue";
import type { Content, Template, UpdateTemplatePayload } from "@/types/templates";
import { updateTemplate } from "@/api/templates";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  template: Template;
}>();

const content = ref<Content>({ type: "text/plain", text: "" });
const subject = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Template): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateTemplatePayload = {
      subject: props.template.subject !== subject.value ? subject.value : undefined,
      content: props.template.content.type !== content.value.type || props.template.content.text !== content.value.text ? content.value : undefined,
    };
    const template: Template = await updateTemplate(props.template.id, payload);
    emit("updated", template);
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  () => props.template,
  (template) => {
    subject.value = template.subject;
    content.value = { ...template.content };
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="handleSubmit(submit)">
      <div class="row">
        <SubjectInput class="col" required v-model="subject" />
        <ContentTypeFormSelect class="col" required v-model="content.type" />
      </div>
      <ContentTextarea required v-model="content.text" />
      <div class="mb-3">
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          icon="fas fa-save"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.save')"
          type="submit"
        />
      </div>
    </form>
  </div>
</template>
