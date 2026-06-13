import { useState } from 'react'
import toast from 'react-hot-toast'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { api } from '../../api/client'
import type { ApiResponse, ProjectDto } from '../../api/types'
import { useAppSelector } from '../../app/hooks'

export function ProjectCreatePage() {
  const navigate = useNavigate()
  const role = useAppSelector((s) => s.auth.user?.role)
  const [submitting, setSubmitting] = useState(false)
  const [form, setForm] = useState({
    projectCode: '',
    projectName: '',
    ownerName: '',
    projectArea: '',
    projectLocation: '',
    driveLink: '',
    contactName: '',
    contactNumber: '',
    email: '',
    emailPassword: '',
    ecpsAccountId: '',
    ecpsPassword: '',
    ecpsApplicationId: '',
    notes: '',
  })

  if (role !== 'Admin') {
    return <Navigate to="/projects" replace />
  }

  const submitCreate = async () => {
    setSubmitting(true)

    try {
      const payload = {
        projectCode: form.projectCode.trim(),
        projectName: form.projectName.trim(),
        ownerName: form.ownerName.trim(),
        projectArea: form.projectArea || null,
        projectLocation: form.projectLocation || null,
        driveLink: form.driveLink || null,
        contactName: form.contactName || null,
        contactNumber: form.contactNumber || null,
        email: form.email || null,
        emailPassword: form.emailPassword || null,
        ecpsAccountId: form.ecpsAccountId || null,
        ecpsPassword: form.ecpsPassword || null,
        ecpsApplicationId: form.ecpsApplicationId || null,
        notes: form.notes || null,
      }

      const res = await api.post<ApiResponse<ProjectDto>>('/api/projects', payload)
      if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Failed to create project')

      toast.success('Project created')
      navigate(`/projects/${res.data.data.id}`)
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to create project')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <Link to="/projects" className="text-sm text-slate-500 hover:text-slate-700">
            ← Back to Projects
          </Link>
          <div className="mt-2 text-2xl font-semibold text-slate-900">Create Project</div>
          <div className="text-sm text-slate-500">Start with the essentials now. You can complete credentials and additional data later.</div>
        </div>
        <div className="rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800">
          Required first: Project Code, Project Name, Owner Name
        </div>
      </div>

      <div className="grid grid-cols-1 gap-4 xl:grid-cols-[1.5fr_1fr]">
        <div className="space-y-4">
          <section className="rounded-xl border border-slate-200 bg-white p-5">
            <div className="text-base font-semibold text-slate-900">Basic Info</div>
            <div className="mt-1 text-sm text-slate-500">Capture the minimum needed to create the project record.</div>
            <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label className="text-sm font-medium text-slate-700">Project Code</label>
                <input
                  value={form.projectCode}
                  onChange={(e) => setForm((current) => ({ ...current, projectCode: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  placeholder="e.g. RAJUK-2026-001"
                />
              </div>
              <div>
                <label className="text-sm font-medium text-slate-700">Project Name</label>
                <input
                  value={form.projectName}
                  onChange={(e) => setForm((current) => ({ ...current, projectName: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  placeholder="e.g. Green View Tower"
                />
              </div>
              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700">Owner Name</label>
                <input
                  value={form.ownerName}
                  onChange={(e) => setForm((current) => ({ ...current, ownerName: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                />
              </div>
            </div>
          </section>

          <section className="rounded-xl border border-slate-200 bg-white p-5">
            <div className="text-base font-semibold text-slate-900">Project Info</div>
            <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label className="text-sm font-medium text-slate-700">Project Area</label>
                <input
                  value={form.projectArea}
                  onChange={(e) => setForm((current) => ({ ...current, projectArea: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                />
              </div>
              <div>
                <label className="text-sm font-medium text-slate-700">Project Location</label>
                <input
                  value={form.projectLocation}
                  onChange={(e) => setForm((current) => ({ ...current, projectLocation: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                />
              </div>
              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700">Google Drive Link</label>
                <input
                  value={form.driveLink}
                  onChange={(e) => setForm((current) => ({ ...current, driveLink: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  placeholder="https://drive.google.com/..."
                />
              </div>
              <div>
                <label className="text-sm font-medium text-slate-700">Contact Name</label>
                <input
                  value={form.contactName}
                  onChange={(e) => setForm((current) => ({ ...current, contactName: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                />
              </div>
              <div>
                <label className="text-sm font-medium text-slate-700">Contact Number</label>
                <input
                  value={form.contactNumber}
                  onChange={(e) => setForm((current) => ({ ...current, contactNumber: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                />
              </div>
            </div>
          </section>

          <section className="rounded-xl border border-slate-200 bg-white p-5">
            <div className="flex items-center justify-between gap-3">
              <div>
                <div className="text-base font-semibold text-slate-900">Credentials</div>
                <div className="text-sm text-slate-500">Optional at creation time. You can manage these later from the dedicated credentials screen.</div>
              </div>
              <span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-600">Optional</span>
            </div>
            <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label className="text-sm font-medium text-slate-700">Email</label>
                <input
                  value={form.email}
                  onChange={(e) => setForm((current) => ({ ...current, email: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  type="email"
                />
              </div>
              <div>
                <label className="text-sm font-medium text-slate-700">Email Password</label>
                <input
                  value={form.emailPassword}
                  onChange={(e) => setForm((current) => ({ ...current, emailPassword: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  type="password"
                />
              </div>
              <div>
                <label className="text-sm font-medium text-slate-700">ECPS Account Id</label>
                <input
                  value={form.ecpsAccountId}
                  onChange={(e) => setForm((current) => ({ ...current, ecpsAccountId: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                />
              </div>
              <div>
                <label className="text-sm font-medium text-slate-700">ECPS Password</label>
                <input
                  value={form.ecpsPassword}
                  onChange={(e) => setForm((current) => ({ ...current, ecpsPassword: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                  type="password"
                />
              </div>
              <div className="md:col-span-2">
                <label className="text-sm font-medium text-slate-700">ECPS Application Id</label>
                <input
                  value={form.ecpsApplicationId}
                  onChange={(e) => setForm((current) => ({ ...current, ecpsApplicationId: e.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                />
              </div>
            </div>
          </section>
        </div>

        <div className="space-y-4">
          <section className="rounded-xl border border-slate-200 bg-white p-5">
            <div className="text-base font-semibold text-slate-900">Notes</div>
            <textarea
              value={form.notes}
              onChange={(e) => setForm((current) => ({ ...current, notes: e.target.value }))}
              className="mt-4 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
              rows={12}
              placeholder="Add any context, follow-up notes, or pending tasks..."
            />
          </section>

          <section className="rounded-xl border border-slate-200 bg-white p-5">
            <div className="text-base font-semibold text-slate-900">Next Step</div>
            <div className="mt-2 text-sm text-slate-500">
              After creating the record, continue with workflow updates or open the dedicated credentials screen when sensitive data is ready.
            </div>
            <div className="mt-5 flex flex-col gap-3">
              <button
                onClick={submitCreate}
                disabled={submitting}
                className="rounded-md bg-slate-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {submitting ? 'Creating...' : 'Create Project'}
              </button>
              <Link to="/projects" className="rounded-md border border-slate-300 px-4 py-2.5 text-center text-sm text-slate-700 hover:bg-slate-50">
                Cancel
              </Link>
            </div>
          </section>
        </div>
      </div>
    </div>
  )
}
