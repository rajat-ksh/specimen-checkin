<script setup lang="ts">
import { computed, reactive } from 'vue'
import type { AddSpecimenRequest } from '../types/manifest'

const props = defineProps<{
  open: boolean
}>()

const emit = defineEmits<{
  close: []
  submit: [request: AddSpecimenRequest]
}>()

const form = reactive<AddSpecimenRequest>({
  code: '',
  patient: '',
  site: '',
  provider: ''
})

const errors = reactive<Record<keyof AddSpecimenRequest, string>>({
  code: '',
  patient: '',
  site: '',
  provider: ''
})

function resetForm() {
  form.code = ''
  form.patient = ''
  form.site = ''
  form.provider = ''

  errors.code = ''
  errors.patient = ''
  errors.site = ''
  errors.provider = ''
}

function validateField(field: keyof AddSpecimenRequest) {
  errors[field] = form[field].trim() ? '' : `${field.charAt(0).toUpperCase() + field.slice(1)} is required.`
}

function validateForm() {
  validateField('code')
  validateField('patient')
  validateField('site')
  validateField('provider')

  return !errors.code && !errors.patient && !errors.site && !errors.provider
}

const isSubmitDisabled = computed(() => {
  return (
    !form.code.trim() ||
    !form.patient.trim() ||
    !form.site.trim() ||
    !form.provider.trim()
  )
})

function submit() {
  if (!validateForm()) {
    return
  }

  emit('submit', {
    code: form.code.trim(),
    patient: form.patient.trim(),
    site: form.site.trim(),
    provider: form.provider.trim()
  })

  resetForm()
}
</script>

<template>
  <div v-if="open" class="overlay">
    <div class="dialog">
      <div class="dialog-header">
        <div>
          <p class="eyebrow">Discrepancy</p>
          <h2>Add off-manifest specimen</h2>
        </div>

        <button type="button" class="icon-button" @click="emit('close')">
          ×
        </button>
      </div>

      <div class="form-grid">
        <label>
          <span>Specimen code</span>
          <input
            v-model="form.code"
            :class="{'input-error': errors.code}"
            @blur="validateField('code')"
            placeholder="SP-999"
          />
          <p v-if="errors.code" class="error-text">{{ errors.code }}</p>
        </label>

        <label>
          <span>Patient</span>
          <input
            v-model="form.patient"
            :class="{'input-error': errors.patient}"
            @blur="validateField('patient')"
            placeholder="Synthetic patient name"
          />
          <p v-if="errors.patient" class="error-text">{{ errors.patient }}</p>
        </label>

        <label>
          <span>Site</span>
          <input
            v-model="form.site"
            :class="{'input-error': errors.site}"
            @blur="validateField('site')"
            placeholder="Collection site"
          />
          <p v-if="errors.site" class="error-text">{{ errors.site }}</p>
        </label>

        <label>
          <span>Provider</span>
          <input
            v-model="form.provider"
            :class="{'input-error': errors.provider}"
            @blur="validateField('provider')"
            placeholder="Provider name"
          />
          <p v-if="errors.provider" class="error-text">{{ errors.provider }}</p>
        </label>
      </div>

      <div class="dialog-actions">
        <button type="button" class="secondary-button" @click="emit('close')">
          Cancel
        </button>

        <button
          type="button"
          class="primary-button"
          @click="submit"
          :disabled="isSubmitDisabled"
        >
          Add specimen
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  display: grid;
  place-items: center;
  padding: 20px;
  background: rgba(15, 23, 42, 0.45);
}

.dialog {
  width: min(520px, 100%);
  padding: 22px;
  border-radius: 14px;
  background: white;
  box-shadow: 0 20px 60px rgba(15, 23, 42, 0.22);
}

.dialog-header,
.dialog-actions {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
}

.eyebrow {
  margin: 0 0 4px;
  color: #64748b;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

h2 {
  margin: 0;
}

.icon-button {
  border: 0;
  background: transparent;
  font-size: 24px;
  cursor: pointer;
}

.form-grid {
  display: grid;
  gap: 14px;
  margin: 20px 0;
}

label span {
  display: block;
  margin-bottom: 6px;
  color: #475569;
  font-size: 13px;
  font-weight: 700;
}

input {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
}

.input-error {
  border-color: #f97316;
}

.error-text {
  margin: 6px 0 0;
  color: #dc2626;
  font-size: 12px;
}

.dialog-actions {
  justify-content: flex-end;
}

button {
  padding: 9px 12px;
  border: 0;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 700;
}

.secondary-button {
  color: #334155;
  background: #e2e8f0;
}

.primary-button {
  color: white;
  background: #2563eb;
}

button:disabled,
button[disabled] {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>