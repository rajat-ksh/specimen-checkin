export interface ManifestListItem {
  id: string
  code: string
  status: string
  sentAt: string
  totalSpecimens: number
  receivedCount: number
  pendingCount: number
  flaggedCount: number
}

export interface Specimen {
  id: string
  code: string
  patient: string
  site: string
  provider: string
  status: string
}

export interface Discrepancy {
  id: string
  specimenId?: string | null
  type: string
  status: string
  note: string
}

export interface ManifestDetail {
  id: string
  code: string
  status: string
  sentAt: string
  totalSpecimens: number
  receivedCount: number
  pendingCount: number
  flaggedCount: number
  addedCount: number
  readyToClose: boolean
  specimens: Specimen[]
  discrepancies: Discrepancy[]
}

export interface AddSpecimenRequest {
  code: string
  patient: string
  site: string
  provider: string
}