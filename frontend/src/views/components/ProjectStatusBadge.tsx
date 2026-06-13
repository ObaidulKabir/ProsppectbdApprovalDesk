import clsx from 'clsx'
import type { ProjectStatus } from '../../api/types'

const labelMap: Record<ProjectStatus, string> = {
  Draft: 'Draft',
  EcpsAccountCreated: 'ECPS Account Created',
  ApplicationSubmitted: 'Application Submitted',
  UnderReview: 'Under Review',
  CorrectionRequired: 'Correction Required',
  Resubmitted: 'Resubmitted',
  Approved: 'Approved',
  Rejected: 'Rejected',
}

const toneMap: Record<ProjectStatus, string> = {
  Draft: 'bg-slate-100 text-slate-700',
  EcpsAccountCreated: 'bg-cyan-100 text-cyan-800',
  ApplicationSubmitted: 'bg-blue-100 text-blue-800',
  UnderReview: 'bg-amber-100 text-amber-800',
  CorrectionRequired: 'bg-orange-100 text-orange-800',
  Resubmitted: 'bg-violet-100 text-violet-800',
  Approved: 'bg-emerald-100 text-emerald-800',
  Rejected: 'bg-rose-100 text-rose-800',
}

export const projectStatusOptions: ProjectStatus[] = [
  'Draft',
  'EcpsAccountCreated',
  'ApplicationSubmitted',
  'UnderReview',
  'CorrectionRequired',
  'Resubmitted',
  'Approved',
  'Rejected',
]

export function formatProjectStatus(status: ProjectStatus) {
  return labelMap[status] ?? status
}

export function ProjectStatusBadge({ status }: { status: ProjectStatus }) {
  return (
    <span className={clsx('inline-flex items-center rounded-full px-2.5 py-1 text-xs font-medium', toneMap[status])}>
      {formatProjectStatus(status)}
    </span>
  )
}
