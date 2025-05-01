<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import CustomAttributeEdit from "./CustomAttributeEdit.vue";
import { CustomAttributeState, type CustomAttribute, type ICustomAttributeState } from "@/types/custom";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  attributes: CustomAttribute[];
  save: (customAttributes: CustomAttribute[]) => Promise<void>;
}>();

const states = ref<CustomAttributeState[]>([]);

const hasChanges = computed<boolean>(() => states.value.some((state) => Boolean(state.status)));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

function onAdd(): void {
  states.value.push(new CustomAttributeState());
}
function onRemove(index: number): void {
  const state: ICustomAttributeState | undefined = states.value[index];
  if (state) {
    if (state.status === "added") {
      states.value.splice(index, 1);
    } else {
      state.remove();
    }
  }
}
function onRestore(index: number): void {
  const state: ICustomAttributeState | undefined = states.value[index];
  state?.restore();
}
function onUpdate(index: number, state: CustomAttributeState): void {
  states.value.splice(index, 1, state);
}

const { isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  try {
    const customAttributes: CustomAttribute[] = [];
    states.value.forEach((state) => {
      const initialKey: string = state.getInitialKey();
      const key: string = state.getCurrentKey();
      customAttributes.push({ key, value: state.status === "removed" ? "" : state.getCurrentValue() });
      if (initialKey !== "" && initialKey !== key) {
        customAttributes.push({ key: initialKey, value: "" });
      }
    });
    await props.save(customAttributes);
    reset();
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  () => props.attributes,
  (attributes) => {
    states.value = attributes.map((attribute) => new CustomAttributeState(attribute));
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <div class="mb-3">
      <TarButton icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="onAdd" />
    </div>
    <form v-if="states.length > 0" @submit.prevent="handleSubmit(submit)">
      <CustomAttributeEdit
        v-for="(state, index) in states"
        :key="index"
        :id="`edit-attribute-${index}`"
        :model-value="state"
        @removed="onRemove(index)"
        @restored="onRestore(index)"
        @update:model-value="onUpdate(index, $event as CustomAttributeState)"
      />
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
    <p v-else>{{ t("customAttributes.empty") }}</p>
  </div>
</template>
