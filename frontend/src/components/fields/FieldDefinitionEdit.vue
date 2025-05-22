<script setup lang="ts">
import { TarButton, TarModal, type ButtonVariant } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import FieldTypeFormSelect from "./FieldTypeFormSelect.vue";
import FieldTypeInput from "./FieldTypeInput.vue";
import IndexedCheckbox from "./IndexedCheckbox.vue";
import InvariantCheckbox from "./InvariantCheckbox.vue";
import PlaceholderInput from "./PlaceholderInput.vue";
import RequiredCheckbox from "./RequiredCheckbox.vue";
import UniqueCheckbox from "./UniqueCheckbox.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ContentType } from "@/types/contents";
import type { CreateOrReplaceFieldDefinitionPayload, FieldDefinition, FieldType } from "@/types/fields";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createFieldDefinition, replaceFieldDefinition } from "@/api/fields";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  contentType: ContentType;
  field?: FieldDefinition;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const fieldType = ref<FieldType>();
const isIndexed = ref<boolean>(false);
const isInvariant = ref<boolean>(false);
const isRequired = ref<boolean>(false);
const isUnique = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const placeholder = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const id = computed<string>(() => props.field?.id ?? "create-field-definition");
const variant = computed<ButtonVariant>(() => (props.field ? "primary" : "success"));

function setModel(field: FieldDefinition | undefined): void {
  uniqueNameAlreadyUsed.value = false;
  fieldType.value = field?.fieldType ?? undefined;
  isInvariant.value = field?.isInvariant ?? false;
  isRequired.value = field?.isRequired ?? false;
  isIndexed.value = field?.isIndexed ?? false;
  isUnique.value = field?.isUnique ?? false;
  uniqueName.value = field?.uniqueName ?? "";
  displayName.value = field?.displayName ?? "";
  description.value = field?.description ?? "";
  placeholder.value = field?.placeholder ?? "";
}

function hide(): void {
  modalRef.value?.hide();
}

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  setModel(props.field);
  reset();
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: ContentType): void;
}>();

const { hasChanges, isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  if (fieldType.value) {
    uniqueNameAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceFieldDefinitionPayload = {
        fieldType: fieldType.value?.id,
        isInvariant: isInvariant.value,
        isRequired: isRequired.value,
        isIndexed: isIndexed.value,
        isUnique: isUnique.value,
        uniqueName: uniqueName.value,
        displayName: displayName.value,
        description: description.value,
        placeholder: placeholder.value,
      };
      const updated: ContentType = props.field
        ? await replaceFieldDefinition(props.contentType.id, props.field.id, payload)
        : await createFieldDefinition(props.contentType.id, payload);
      emit("saved", updated);
      onReset();
      hide();
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.FieldUniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    }
  }
}

watch(() => props.field, setModel, { deep: true, immediate: true });
</script>

<template>
  <span>
    <TarButton
      :icon="`fas fa-${field ? 'edit' : 'plus'}`"
      :text="t(`actions.${field ? 'edit' : 'create'}`)"
      :variant="variant"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" size="x-large" :title="t(`fields.definition.${field ? 'edit' : 'create'}`)">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <FieldTypeFormSelect v-if="!field" :model-value="fieldType?.id" required @selected="fieldType = $event" />
        <FieldTypeInput v-else-if="fieldType" :field-type="fieldType" :id="`${field.id}-type`" />
        <InvariantCheckbox :id="`${id}-invariant`" v-model="isInvariant" />
        <RequiredCheckbox :id="`${id}-required`" v-model="isRequired" />
        <IndexedCheckbox :id="`${id}-indexed`" v-model="isIndexed" />
        <UniqueCheckbox :id="`${id}-unique`" v-model="isUnique" />
        <div class="row">
          <UniqueNameInput class="col" help="fields.definition.help.uniqueName" :id="`${id}-unique-name`" identifier required v-model="uniqueName" />
          <DisplayNameInput class="col" help="fields.definition.help.displayName" :id="`${id}-display-name`" v-model="displayName" />
        </div>
        <PlaceholderInput :id="`${id}-placeholder`" v-model="placeholder" />
        <DescriptionTextarea help="fields.definition.help.description" :id="`${id}-description`" rows="5" v-model="description" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="`fas fa-${field ? 'save' : 'plus'}`"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(`actions.${field ? 'save' : 'create'}`)"
          type="submit"
          :variant="variant"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </span>
</template>
