import api from '../api/api'
import type {
  AddSpecimenRequest,
  ManifestDetail,
  ManifestListItem
} from '../types/manifest'

export async function getManifests(): Promise<ManifestListItem[]> {
  const response = await api.get<ManifestListItem[]>('/manifest')
  return response.data
}

export async function getManifest(
  manifestId: string
): Promise<ManifestDetail> {
  const response = await api.get<ManifestDetail>(
    `/manifest/${manifestId}`
  )

  return response.data
}

export async function receiveSpecimen(
  manifestId: string,
  specimenId: string
): Promise<void> {
  await api.post(
    `/manifest/${manifestId}/specimens/${specimenId}/receive`
  )
}

export async function flagSpecimen(
  manifestId: string,
  specimenId: string
): Promise<void> {
  await api.post(
    `/manifest/${manifestId}/specimens/${specimenId}/flag`
  )
}

export async function addOffManifestSpecimen(
  manifestId: string,
  request: AddSpecimenRequest
): Promise<void> {
  await api.post(
    `/manifest/${manifestId}/specimens`,
    request
  )
}

export async function closeManifest(
  manifestId: string
): Promise<void> {
  await api.post(`/manifest/${manifestId}/close`)
}