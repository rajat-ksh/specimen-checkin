<script setup lang="ts">
import type { ManifestListItem } from '../types/manifest'

defineProps<{
  manifests: ManifestListItem[]
  selectedManifestId: string | null
  loading: boolean
}>()

const emit = defineEmits<{
  select: [manifestId: string]
}>()

function formatDate(value: string) {
  return new Date(value).toLocaleDateString()
}
</script>

<template>
  <aside class="manifest-panel">
    <div class="panel-header">
      <div>
        <p class="eyebrow">Receiving desk</p>
        <h2>Manifest Worklist</h2>
      </div>

      <span class="count">{{ manifests.length }}</span>
    </div>

    <div v-if="loading" class="state-message">
      Loading manifests...
    </div>

    <div v-else-if="manifests.length === 0" class="state-message">
      No manifests found for this lab.
    </div>

    <button
      v-for="manifest in manifests"
      v-else
      :key="manifest.id"
      class="manifest-card"
      :class="{ selected: manifest.id === selectedManifestId }"
      type="button"
      @click="emit('select', manifest.id)"
    >
      <div class="manifest-top-row">
        <strong>{{ manifest.code }}</strong>

        <span class="status-pill">
          {{ manifest.status }}
        </span>
      </div>

      <div class="manifest-meta">
        Sent {{ formatDate(manifest.sentAt) }}
      </div>

      <div class="manifest-progress">
        <span>
          Received {{ manifest.receivedCount }}/{{ manifest.totalSpecimens }}
        </span>

        <span v-if="manifest.flaggedCount > 0">
          Flagged {{ manifest.flaggedCount }}
        </span>
      </div>
    </button>
  </aside>
</template>

<style scoped>
.manifest-panel {
  width: 340px;
  min-width: 340px;
  height: 100vh;
  overflow-y: auto;
  background: #f7f9fc;
  border-right: 1px solid #dfe5ec;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24px 20px 16px;
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
  color: #172033;
  font-size: 20px;
}

.count {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 30px;
  height: 30px;
  border-radius: 999px;
  color: #1e40af;
  background: #dbeafe;
  font-weight: 700;
}

.state-message {
  padding: 20px;
  color: #64748b;
}

.manifest-card {
  width: calc(100% - 24px);
  margin: 6px 12px;
  padding: 16px;
  border: 1px solid #dfe5ec;
  border-radius: 12px;
  text-align: left;
  background: white;
  cursor: pointer;
  transition: 0.15s ease;
}

.manifest-card:hover {
  border-color: #93c5fd;
}

.manifest-card.selected {
  border-color: #2563eb;
  background: #eff6ff;
}

.manifest-top-row,
.manifest-progress {
  display: flex;
  justify-content: space-between;
  gap: 8px;
}

.manifest-meta,
.manifest-progress {
  margin-top: 8px;
  color: #64748b;
  font-size: 13px;
}

.status-pill {
  padding: 4px 8px;
  border-radius: 999px;
  color: #1e40af;
  background: #dbeafe;
  font-size: 12px;
  font-weight: 700;
}
</style>