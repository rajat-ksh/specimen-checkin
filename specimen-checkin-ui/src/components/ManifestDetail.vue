<script setup lang="ts">
import { computed, ref } from 'vue'
import type { ManifestDetail } from '../types/manifest'

const props = defineProps<{
  manifest: ManifestDetail | null
  loading: boolean
}>()

const emit = defineEmits<{
  receive: [specimenId: string]
  flag: [specimenId: string]
  close: []
}>()

const actionError = ref<string | null>(null)

const canClose = computed(() => {
  return Boolean(props.manifest?.readyToClose)
})

function statusClass(status: string) {
  return `status-${status.toLowerCase()}`
}
</script>

<template>
  <main class="detail-panel">
    <div v-if="loading" class="empty-state">
      Loading manifest details...
    </div>

    <div v-else-if="!manifest" class="empty-state">
      Select a manifest from the worklist to begin check-in.
    </div>

    <template v-else>
      <header class="detail-header">
        <div>
          <p class="eyebrow">Specimen check-in</p>
          <h1>{{ manifest.code }}</h1>
          <p class="subtitle">
            Sent {{ new Date(manifest.sentAt).toLocaleString() }}
          </p>
        </div>

        <span class="large-status">
          {{ manifest.status }}
        </span>
      </header>

      <section class="summary-grid">
        <div class="summary-card">
          <span>Total</span>
          <strong>{{ manifest.totalSpecimens }}</strong>
        </div>

        <div class="summary-card">
          <span>Received</span>
          <strong>{{ manifest.receivedCount }}</strong>
        </div>

        <div class="summary-card">
          <span>Pending</span>
          <strong>{{ manifest.pendingCount }}</strong>
        </div>

        <div class="summary-card">
          <span>Flagged</span>
          <strong>{{ manifest.flaggedCount }}</strong>
        </div>

        <div class="summary-card">
          <span>Added</span>
          <strong>{{ manifest.addedCount }}</strong>
        </div>
      </section>

      <section class="content-card">
        <div class="section-header">
          <div>
            <h2>Expected Specimens</h2>
            <p>Confirm each physical bottle against the manifest.</p>
          </div>

          <span
            class="ready-pill"
            :class="{ ready: manifest.readyToClose }"
          >
            {{
              manifest.readyToClose
                ? 'Ready to close'
                : 'Reconciliation required'
            }}
          </span>
        </div>

        <div v-if="manifest.specimens.length === 0" class="empty-table">
          No specimens found.
        </div>

        <table v-else>
          <thead>
            <tr>
              <th>Specimen</th>
              <th>Patient</th>
              <th>Site</th>
              <th>Provider</th>
              <th>Status</th>
              <th class="actions-column">Actions</th>
            </tr>
          </thead>

          <tbody>
            <tr
              v-for="specimen in manifest.specimens"
              :key="specimen.id"
            >
              <td>
                <strong>{{ specimen.code }}</strong>
              </td>

              <td>{{ specimen.patient }}</td>
              <td>{{ specimen.site }}</td>
              <td>{{ specimen.provider }}</td>

              <td>
                <span
                  class="specimen-status"
                  :class="statusClass(specimen.status)"
                >
                  {{ specimen.status }}
                </span>
              </td>

              <td class="action-buttons">
                <button
                  type="button"
                  class="receive-button"
                  :disabled="specimen.status === 'Received'"
                  @click="emit('receive', specimen.id)"
                >
                  Receive
                </button>

                <button
                  type="button"
                  class="flag-button"
                  :disabled="specimen.status === 'Flagged'"
                  @click="emit('flag', specimen.id)"
                >
                  Flag missing
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </section>

      <p v-if="actionError" class="error-message">
        {{ actionError }}
      </p>

      <footer class="detail-footer">
        <div>
          <strong>
            {{
              manifest.readyToClose
                ? 'All bottles have been reconciled.'
                : `${manifest.pendingCount} pending specimen(s) remain.`
            }}
          </strong>
        </div>

        <button
          type="button"
          class="close-button"
          :disabled="!canClose"
          @click="emit('close')"
        >
          Verify and close manifest
        </button>
      </footer>
    </template>
  </main>
</template>

<style scoped>
.detail-panel {
  flex: 1;
  min-width: 0;
  min-height: 100vh;
  padding: 28px;
  background: #ffffff;
}

.empty-state {
  display: grid;
  place-items: center;
  min-height: 70vh;
  color: #64748b;
}

.detail-header,
.section-header,
.detail-footer {
  display: flex;
  justify-content: space-between;
  gap: 20px;
  align-items: center;
}

.eyebrow {
  margin: 0 0 4px;
  color: #64748b;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

h1,
h2,
p {
  margin: 0;
}

h1 {
  color: #172033;
}

.subtitle {
  margin-top: 6px;
  color: #64748b;
}

.large-status,
.ready-pill,
.specimen-status {
  padding: 6px 10px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 700;
}

.large-status {
  color: #1e40af;
  background: #dbeafe;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(5, minmax(100px, 1fr));
  gap: 12px;
  margin: 24px 0;
}

.summary-card {
  padding: 16px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
}

.summary-card span {
  display: block;
  color: #64748b;
  font-size: 13px;
}

.summary-card strong {
  display: block;
  margin-top: 6px;
  color: #172033;
  font-size: 24px;
}

.content-card {
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  overflow: hidden;
}

.section-header {
  padding: 18px;
  border-bottom: 1px solid #e2e8f0;
}

.section-header p {
  margin-top: 4px;
  color: #64748b;
  font-size: 14px;
}

.ready-pill {
  color: #92400e;
  background: #fef3c7;
}

.ready-pill.ready {
  color: #166534;
  background: #dcfce7;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 14px 16px;
  border-bottom: 1px solid #eef2f7;
  text-align: left;
  font-size: 14px;
}

th {
  color: #64748b;
  background: #f8fafc;
}

.actions-column {
  width: 210px;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

button {
  border: 0;
  border-radius: 8px;
  padding: 8px 10px;
  cursor: pointer;
  font-weight: 600;
}

button:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}

.receive-button {
  color: #166534;
  background: #dcfce7;
}

.flag-button {
  color: #991b1b;
  background: #fee2e2;
}

.close-button {
  color: white;
  background: #2563eb;
}

.detail-footer {
  margin-top: 18px;
}

.status-pending {
  color: #92400e;
  background: #fef3c7;
}

.status-received {
  color: #166534;
  background: #dcfce7;
}

.status-flagged {
  color: #991b1b;
  background: #fee2e2;
}

.status-added {
  color: #1e40af;
  background: #dbeafe;
}

.empty-table {
  padding: 24px;
  color: #64748b;
}

.error-message {
  margin-top: 12px;
  color: #b91c1c;
}
</style>