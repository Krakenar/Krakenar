<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { SearchResults } from "@/types/search";
import type { Sender, SearchSendersPayload, SenderKind } from "@/types/senders";
import { formatSender } from "@/helpers/format";
import { searchSenders } from "@/api/senders";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    kind?: SenderKind;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "sender",
    label: "senders.select.label",
    placeholder: "senders.select.placeholder",
  },
);

const senders = ref<Sender[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    senders.value.map((sender) => ({ text: formatSender(sender), value: sender.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Sender | undefined): void;
  (e: "update:modelValue", value: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:modelValue", id);

  const sender: Sender | undefined = senders.value.find((sender) => sender.id === id);
  emit("selected", sender);
}

async function refresh(kind?: SenderKind): Promise<void> {
  try {
    const payload: SearchSendersPayload = {
      ids: [],
      kind,
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Sender> = await searchSenders(payload);
    senders.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(() => props.kind, refresh, { immediate: true });
</script>

<template>
  <FormSelect
    :disabled="options.length < 1"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder ?? label)"
    :required="required"
    @update:modelValue="onModelValueUpdate"
  />
</template>
