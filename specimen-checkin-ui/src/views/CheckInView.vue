<script setup lang="ts">
import { onMounted, ref } from 'vue'
import ManifestDetail from '../components/ManifestDetail.vue'
import ManifestList from '../components/ManifestList.vue'
import {
  closeManifest,
  flagSpecimen,
  getManifest,
  getManifests,
  receiveSpecimen
} from '../services/manifestService.ts'
import type {
  ManifestDetail as ManifestDetailType,
  ManifestListItem
} from '../types/manifest.ts'

const manifests = ref<ManifestListItem[]>([])
const selectedManifest = ref<ManifestDetailType | null>(null)
const selectedManifestId = ref<string | null>(null)

const loadingManifests = ref(false)
const loadingDetail = ref(false)
const errorMessage = ref<string | null>(null)

async function loadManifests() {
  loadingManifests.value = true
  errorMessage.value = null

  try {
    manifests.value = await getManifests()

    if (
      manifests.value.length > 0 &&
      selectedManifestId.value === null
    ) {
      await selectManifest(manifests.value[0].id)
    }
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Unable to load manifests.'
  } finally {
    loadingManifests.value = false
  }
}

async function selectManifest(manifestId: string) {
  selectedManifestId.value = manifestId
  loadingDetail.value = true
  errorMessage.value = null

  try {
    selectedManifest.value = await getManifest(manifestId)
  } catch (error) {
    console.error(error)
    selectedManifest.value = null
    errorMessage.value = 'Unable to load manifest details.'
  } finally {
    loadingDetail.value = false
  }
}

async function refreshSelectedManifest() {
  if (!selectedManifestId.value) {
    return
  }

  await selectManifest(selectedManifestId.value)
  await loadManifests()
}

async function handleReceive(specimenId: string) {
  if (!selectedManifestId.value) {
    return
  }

  try {
    await receiveSpecimen(
      selectedManifestId.value,
      specimenId
    )

    await refreshSelectedManifest()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Unable to receive specimen.'
  }
}

async function handleFlag(specimenId: string) {
  if (!selectedManifestId.value) {
    return
  }

  try {
    await flagSpecimen(
      selectedManifestId.value,
      specimenId
    )

    await refreshSelectedManifest()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Unable to flag specimen.'
  }
}

async function handleClose() {
  if (!selectedManifestId.value) {
    return
  }

  try {
    await closeManifest(selectedManifestId.value)
    await refreshSelectedManifest()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Unable to close manifest.'
  }
}

onMounted(loadManifests)
</script>

<template>
  <div class="page-shell">
    <ManifestList
      :manifests="manifests"
      :selected-manifest-id="selectedManifestId"
      :loading="loadingManifests"
      @select="selectManifest"
    />

    <ManifestDetail
      :manifest="selectedManifest"
      :loading="loadingDetail"
      @receive="handleReceive"
      @flag="handleFlag"
      @close="handleClose"
    />
  </div>

  <p v-if="errorMessage" class="global-error">
    {{ errorMessage }}
  </p>
</template>

<style scoped>
.page-shell {
  display: flex;
  min-height: 100vh;
}

.global-error {
  position: fixed;
  right: 20px;
  bottom: 20px;
  margin: 0;
  padding: 12px 16px;
  border: 1px solid #fecaca;
  border-radius: 10px;
  color: #991b1b;
  background: #fef2f2;
}
</style>