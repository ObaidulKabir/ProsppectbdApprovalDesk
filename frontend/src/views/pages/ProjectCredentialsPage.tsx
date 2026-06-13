import { useEffect, useState, type ReactNode } from 'react'
import toast from 'react-hot-toast'
import { Link, useParams } from 'react-router-dom'
import { api } from '../../api/client'
import type { ApiResponse, ProjectDto } from '../../api/types'
import { useAppSelector } from '../../app/hooks'

type RevealState = {
  emailPassword: boolean
  ecpsPassword: boolean
}

export function ProjectCredentialsPage() {
  const { id } = useParams()
  const role = useAppSelector((s) => s.auth.user?.role)
  const [project, setProject] = useState<ProjectDto | null>(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [credentialInfo, setCredentialInfo] = useState({
    email: '',
    ecpsAccountId: '',
    ecpsApplicationId: '',
  })
  const [passwords, setPasswords] = useState({
    emailPassword: '',
    ecpsPassword: '',
  })
  const [revealed, setRevealed] = useState<RevealState>({
    emailPassword: false,
    ecpsPassword: false,
  })

  const reload = () => {
    if (!id) return Promise.resolve()

    setLoading(true)
    return api
      .get<ApiResponse<ProjectDto>>(`/api/projects/${id}`)
      .then((res) => {
        if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Failed to load credentials')
        const data = res.data.data
        setProject(data)
        setCredentialInfo({
          email: data.email || '',
          ecpsAccountId: data.ecpsAccountId || '',
          ecpsApplicationId: data.ecpsApplicationId || '',
        })
        setPasswords({
          emailPassword: data.emailPassword || '',
          ecpsPassword: data.ecpsPassword || '',
        })
      })
      .catch((err) => toast.error(err instanceof Error ? err.message : 'Failed to load credentials'))
      .finally(() => setLoading(false))
  }

  useEffect(() => {
    reload()
  }, [id])

  const copyValue = async (value: string, label: string) => {
    if (!value) return

    try {
      await navigator.clipboard.writeText(value)
      toast.success(`${label} copied`)
    } catch {
      toast.error(`Failed to copy ${label.toLowerCase()}`)
    }
  }

  const saveCredentials = async () => {
    if (!id || !project || role !== 'Admin') return

    setSaving(true)
    try {
      const updateProjectPayload = {
        projectName: project.projectName,
        ownerName: project.ownerName,
        projectArea: project.projectArea || null,
        projectLocation: project.projectLocation || null,
        contactName: project.contactName || null,
        contactNumber: project.contactNumber || null,
        email: credentialInfo.email || null,
        ecpsAccountId: credentialInfo.ecpsAccountId || null,
        ecpsApplicationId: credentialInfo.ecpsApplicationId || null,
        notes: project.notes || null,
      }

      const updateProjectRes = await api.put<ApiResponse<unknown>>(`/api/projects/${id}`, updateProjectPayload)
      if (!updateProjectRes.data.success) throw new Error(updateProjectRes.data.message || 'Failed to update credentials')

      const passwordPayload = {
        emailPassword: passwords.emailPassword || null,
        ecpsPassword: passwords.ecpsPassword || null,
      }

      const passwordRes = await api.patch<ApiResponse<unknown>>(`/api/projects/${id}/credentials`, passwordPayload)
      if (!passwordRes.data.success) throw new Error(passwordRes.data.message || 'Failed to update credentials')

      toast.success('Credentials updated')
      await reload()
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to update credentials')
    } finally {
      setSaving(false)
    }
  }

  if (loading && !project) {
    return <div className="text-sm text-slate-600">Loading credentials…</div>
  }

  if (!project) {
    return <div className="text-sm text-slate-600">Project not found.</div>
  }

  const canEdit = role === 'Admin'

  return (
    <div className="mx-auto max-w-4xl space-y-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <Link to={`/projects/${project.id}`} className="text-sm text-slate-500 hover:text-slate-700">
            ← Back to Project Overview
          </Link>
          <div className="mt-2 text-2xl font-semibold text-slate-900">Manage Credentials</div>
          <div className="mt-1 text-sm text-slate-500">
            {project.projectName} · {project.projectCode}
          </div>
        </div>
        <div className="rounded-lg border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800">
          Sensitive data. Reveal and copy only when needed.
        </div>
      </div>

      <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
        <section className="rounded-xl border border-slate-200 bg-white p-5">
          <div className="text-base font-semibold text-slate-900">Account Access</div>
          <div className="mt-4 space-y-4">
            <FieldShell label="Email">
              <div className="flex gap-2">
                <input
                  value={credentialInfo.email}
                  onChange={(e) => setCredentialInfo((current) => ({ ...current, email: e.target.value }))}
                  className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  type="email"
                  readOnly={!canEdit}
                />
                <button
                  onClick={() => copyValue(credentialInfo.email, 'Email')}
                  className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50"
                  type="button"
                >
                  Copy
                </button>
              </div>
            </FieldShell>

            <FieldShell label="Email Password">
              <div className="flex gap-2">
                <input
                  value={passwords.emailPassword}
                  onChange={(e) => setPasswords((current) => ({ ...current, emailPassword: e.target.value }))}
                  className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  type={revealed.emailPassword ? 'text' : 'password'}
                  readOnly={!canEdit}
                />
                <button
                  onClick={() => setRevealed((current) => ({ ...current, emailPassword: !current.emailPassword }))}
                  className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50"
                  type="button"
                >
                  {revealed.emailPassword ? 'Hide' : 'Reveal'}
                </button>
              </div>
            </FieldShell>
          </div>
        </section>

        <section className="rounded-xl border border-slate-200 bg-white p-5">
          <div className="text-base font-semibold text-slate-900">ECPS Access</div>
          <div className="mt-4 space-y-4">
            <FieldShell label="ECPS Account Id">
              <div className="flex gap-2">
                <input
                  value={credentialInfo.ecpsAccountId}
                  onChange={(e) => setCredentialInfo((current) => ({ ...current, ecpsAccountId: e.target.value }))}
                  className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  readOnly={!canEdit}
                />
                <button
                  onClick={() => copyValue(credentialInfo.ecpsAccountId, 'ECPS account id')}
                  className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50"
                  type="button"
                >
                  Copy
                </button>
              </div>
            </FieldShell>

            <FieldShell label="ECPS Password">
              <div className="flex gap-2">
                <input
                  value={passwords.ecpsPassword}
                  onChange={(e) => setPasswords((current) => ({ ...current, ecpsPassword: e.target.value }))}
                  className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  type={revealed.ecpsPassword ? 'text' : 'password'}
                  readOnly={!canEdit}
                />
                <button
                  onClick={() => setRevealed((current) => ({ ...current, ecpsPassword: !current.ecpsPassword }))}
                  className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50"
                  type="button"
                >
                  {revealed.ecpsPassword ? 'Hide' : 'Reveal'}
                </button>
              </div>
            </FieldShell>

            <FieldShell label="ECPS Application Id">
              <div className="flex gap-2">
                <input
                  value={credentialInfo.ecpsApplicationId}
                  onChange={(e) => setCredentialInfo((current) => ({ ...current, ecpsApplicationId: e.target.value }))}
                  className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  readOnly={!canEdit}
                />
                <button
                  onClick={() => copyValue(credentialInfo.ecpsApplicationId, 'ECPS application id')}
                  className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50"
                  type="button"
                >
                  Copy
                </button>
              </div>
            </FieldShell>
          </div>
        </section>
      </div>

      <section className="rounded-xl border border-slate-200 bg-white p-5">
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div>
            <div className="text-base font-semibold text-slate-900">Access Control</div>
            <div className="text-sm text-slate-500">
              {canEdit
                ? 'Admin can maintain all credential fields from this protected screen.'
                : 'You can view credentials here when permitted, but editing is limited to admin users.'}
            </div>
          </div>
          {canEdit && (
            <button
              onClick={saveCredentials}
              disabled={saving}
              className="rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {saving ? 'Saving...' : 'Save Credentials'}
            </button>
          )}
        </div>
      </section>
    </div>
  )
}

function FieldShell({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div>
      <label className="text-sm font-medium text-slate-700">{label}</label>
      <div className="mt-1">{children}</div>
    </div>
  )
}
