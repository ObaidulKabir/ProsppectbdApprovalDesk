import { useEffect, useState, type ReactNode } from 'react'
import toast from 'react-hot-toast'
import { Link, useParams } from 'react-router-dom'
import { api } from '../../api/client'
import type { ApiResponse, ProjectDto, ProjectStatus, UserDto } from '../../api/types'
import { useAppSelector } from '../../app/hooks'
import { ProjectStatusBadge, formatProjectStatus, projectStatusOptions } from '../components/ProjectStatusBadge'

export function ProjectDetailsPage() {
  const { id } = useParams()
  const [project, setProject] = useState<ProjectDto | null>(null)
  const role = useAppSelector((s) => s.auth.user?.role)
  const [users, setUsers] = useState<UserDto[]>([])
  const [assignedUserId, setAssignedUserId] = useState<string>('')
  const [status, setStatus] = useState<ProjectStatus>('Draft')
  const [submissionDate, setSubmissionDate] = useState<string>('')
  const [approvalDate, setApprovalDate] = useState<string>('')
  const [notes, setNotes] = useState<string>('')
  const [savingSection, setSavingSection] = useState<string | null>(null)
  const [editModes, setEditModes] = useState({
    projectInfo: false,
    workflow: false,
    assignment: false,
    notes: false,
  })
  const [projectInfo, setProjectInfo] = useState({
    projectName: '',
    ownerName: '',
    projectArea: '',
    projectLocation: '',
    driveLink: '',
    contactName: '',
    contactNumber: '',
  })

  const reload = () => {
    if (!id) return Promise.resolve()
    return api
      .get<ApiResponse<ProjectDto>>(`/api/projects/${id}`)
      .then((res) => {
        if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Failed to load project')
        const p = res.data.data
        setProject(p)
        setAssignedUserId(p.assignedUserId || '')
        setStatus(p.status)
        setSubmissionDate(p.submissionDate || '')
        setApprovalDate(p.approvalDate || '')
        setNotes(p.notes || '')
        setProjectInfo({
          projectName: p.projectName || '',
          ownerName: p.ownerName || '',
          projectArea: p.projectArea || '',
          projectLocation: p.projectLocation || '',
          driveLink: p.driveLink || '',
          contactName: p.contactName || '',
          contactNumber: p.contactNumber || '',
        })
      })
      .catch((err) => toast.error(err instanceof Error ? err.message : 'Failed to load project'))
  }

  useEffect(() => {
    reload()
  }, [id])

  useEffect(() => {
    if (role !== 'Admin') return
    api
      .get<ApiResponse<{ items: UserDto[] }>>('/api/users', { params: { page: 1, pageSize: 200 } })
      .then((res) => {
        if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Failed to load users')
        setUsers(res.data.data.items)
      })
      .catch(() => {})
  }, [role])

  const assigneeName = project?.assignedUserId ? users.find((user) => user.id === project.assignedUserId)?.fullName : null

  const resetProjectInfo = () => {
    if (!project) return
    setProjectInfo({
      projectName: project.projectName || '',
      ownerName: project.ownerName || '',
      projectArea: project.projectArea || '',
      projectLocation: project.projectLocation || '',
      driveLink: project.driveLink || '',
      contactName: project.contactName || '',
      contactNumber: project.contactNumber || '',
    })
  }

  const resetWorkflow = () => {
    if (!project) return
    setStatus(project.status)
    setSubmissionDate(project.submissionDate || '')
    setApprovalDate(project.approvalDate || '')
  }

  const resetAssignment = () => {
    if (!project) return
    setAssignedUserId(project.assignedUserId || '')
  }

  const resetNotes = () => {
    if (!project) return
    setNotes(project.notes || '')
  }

  const saveAssignment = async () => {
    if (!id) return
    setSavingSection('assignment')
    try {
      const payload = { assignedUserId: assignedUserId ? assignedUserId : null }
      const res = await api.post<ApiResponse<unknown>>(`/api/projects/${id}/assign`, payload)
      if (!res.data.success) throw new Error(res.data.message || 'Failed to assign project')
      toast.success('Assignment updated')
      await reload()
      setEditModes((current) => ({ ...current, assignment: false }))
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to assign project')
    } finally {
      setSavingSection(null)
    }
  }

  const saveStatus = async () => {
    if (!id) return
    setSavingSection('workflow')
    try {
      const payload = {
        status,
        submissionDate: submissionDate || null,
        approvalDate: approvalDate || null,
      }
      const res = await api.patch<ApiResponse<unknown>>(`/api/projects/${id}/status`, payload)
      if (!res.data.success) throw new Error(res.data.message || 'Failed to update status')
      toast.success('Status updated')
      await reload()
      setEditModes((current) => ({ ...current, workflow: false }))
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to update status')
    } finally {
      setSavingSection(null)
    }
  }

  const saveNotes = async () => {
    if (!id) return
    setSavingSection('notes')
    try {
      const payload = { notes: notes || null }
      const res = await api.patch<ApiResponse<unknown>>(`/api/projects/${id}/notes`, payload)
      if (!res.data.success) throw new Error(res.data.message || 'Failed to update notes')
      toast.success('Notes updated')
      await reload()
      setEditModes((current) => ({ ...current, notes: false }))
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to update notes')
    } finally {
      setSavingSection(null)
    }
  }

  const saveProjectInfo = async () => {
    if (!id || !project || role !== 'Admin') return

    setSavingSection('projectInfo')
    try {
      const payload = {
        projectName: projectInfo.projectName.trim(),
        ownerName: projectInfo.ownerName.trim(),
        projectArea: projectInfo.projectArea || null,
        projectLocation: projectInfo.projectLocation || null,
        driveLink: projectInfo.driveLink || null,
        contactName: projectInfo.contactName || null,
        contactNumber: projectInfo.contactNumber || null,
        email: project.email || null,
        ecpsAccountId: project.ecpsAccountId || null,
        ecpsApplicationId: project.ecpsApplicationId || null,
        notes: project.notes || null,
      }

      const res = await api.put<ApiResponse<unknown>>(`/api/projects/${id}`, payload)
      if (!res.data.success) throw new Error(res.data.message || 'Failed to update project info')
      toast.success('Project info updated')
      await reload()
      setEditModes((current) => ({ ...current, projectInfo: false }))
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to update project info')
    } finally {
      setSavingSection(null)
    }
  }

  if (!project) {
    return <div className="text-sm text-slate-600">Loading…</div>
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-4 rounded-xl border border-slate-200 bg-white p-6 lg:flex-row lg:items-start lg:justify-between">
        <div>
          <Link to="/projects" className="text-sm text-slate-500 hover:text-slate-700">
            ← Back to Projects
          </Link>
          <div className="mt-3 text-2xl font-semibold text-slate-900">{project.projectName}</div>
          <div className="mt-1 text-sm text-slate-600">
            {project.projectCode} · Owner: {project.ownerName}
          </div>
          <div className="mt-4 flex flex-wrap gap-2 text-sm">
            <MetaPill label="Assigned" value={assigneeName || 'Unassigned'} />
            <MetaPill label="Last Updated" value={formatDate(project.updatedAt || project.createdAt)} />
          </div>
        </div>
        <div className="flex flex-wrap items-center gap-2">
          <ProjectStatusBadge status={project.status} />
          <Link to={`/projects/${project.id}/credentials`} className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50">
            Manage Credentials
          </Link>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
        <section className="rounded-xl border border-slate-200 bg-white p-5">
          <SectionHeader
            title="Project Info"
            description="Core record details used to identify and contact the project."
            editable={role === 'Admin'}
            editing={editModes.projectInfo}
            onEdit={() => setEditModes((current) => ({ ...current, projectInfo: true }))}
            onCancel={() => {
              resetProjectInfo()
              setEditModes((current) => ({ ...current, projectInfo: false }))
            }}
          />

          {editModes.projectInfo ? (
            <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
              <EditableField label="Project Name" value={projectInfo.projectName} onChange={(value) => setProjectInfo((current) => ({ ...current, projectName: value }))} />
              <EditableField label="Owner Name" value={projectInfo.ownerName} onChange={(value) => setProjectInfo((current) => ({ ...current, ownerName: value }))} />
              <EditableField label="Project Area" value={projectInfo.projectArea} onChange={(value) => setProjectInfo((current) => ({ ...current, projectArea: value }))} />
              <EditableField label="Project Location" value={projectInfo.projectLocation} onChange={(value) => setProjectInfo((current) => ({ ...current, projectLocation: value }))} />
              <EditableField label="Google Drive Link" value={projectInfo.driveLink} onChange={(value) => setProjectInfo((current) => ({ ...current, driveLink: value }))} />
              <EditableField label="Contact Name" value={projectInfo.contactName} onChange={(value) => setProjectInfo((current) => ({ ...current, contactName: value }))} />
              <EditableField label="Contact Number" value={projectInfo.contactNumber} onChange={(value) => setProjectInfo((current) => ({ ...current, contactNumber: value }))} />
              <div className="md:col-span-2">
                <PrimaryButton onClick={saveProjectInfo} busy={savingSection === 'projectInfo'}>
                  Save Project Info
                </PrimaryButton>
              </div>
            </div>
          ) : (
            <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
              <ReadOnlyField label="Project Name" value={project.projectName} />
              <ReadOnlyField label="Owner Name" value={project.ownerName} />
              <ReadOnlyField label="Project Area" value={project.projectArea} />
              <ReadOnlyField label="Project Location" value={project.projectLocation} />
              <ReadOnlyLinkField label="Google Drive Link" value={project.driveLink} />
              <ReadOnlyField label="Contact Name" value={project.contactName} />
              <ReadOnlyField label="Contact Number" value={project.contactNumber} />
            </div>
          )}
        </section>

        <section className="rounded-xl border border-slate-200 bg-white p-5">
          <SectionHeader
            title="Workflow"
            description="Manage approval progress and milestone dates."
            editable={role === 'Admin'}
            editing={editModes.workflow}
            onEdit={() => setEditModes((current) => ({ ...current, workflow: true }))}
            onCancel={() => {
              resetWorkflow()
              setEditModes((current) => ({ ...current, workflow: false }))
            }}
          />

          {editModes.workflow ? (
            <div className="mt-4 grid grid-cols-1 gap-4">
              <div>
                <label className="text-sm font-medium text-slate-700">Status</label>
                <select
                  value={status}
                  onChange={(e) => setStatus(e.target.value as ProjectStatus)}
                  className="mt-1 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-slate-500"
                >
                  {projectStatusOptions.map((option) => (
                    <option key={option} value={option}>
                      {formatProjectStatus(option)}
                    </option>
                  ))}
                </select>
              </div>
              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <EditableDateField label="Submission Date" value={submissionDate} onChange={setSubmissionDate} />
                <EditableDateField label="Approval Date" value={approvalDate} onChange={setApprovalDate} />
              </div>
              <PrimaryButton onClick={saveStatus} busy={savingSection === 'workflow'}>
                Save Workflow
              </PrimaryButton>
            </div>
          ) : (
            <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
              <ReadOnlyField label="Current Status" value={formatProjectStatus(project.status)} />
              <ReadOnlyField label="Submission Date" value={project.submissionDate || null} />
              <ReadOnlyField label="Approval Date" value={project.approvalDate || null} />
            </div>
          )}
        </section>
      </div>

      <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
        <section className="rounded-xl border border-slate-200 bg-white p-5">
          <SectionHeader
            title="Assignment"
            description="Direct operational ownership to the right user."
            editable={role === 'Admin'}
            editing={editModes.assignment}
            onEdit={() => setEditModes((current) => ({ ...current, assignment: true }))}
            onCancel={() => {
              resetAssignment()
              setEditModes((current) => ({ ...current, assignment: false }))
            }}
          />

          {editModes.assignment ? (
            <div className="mt-4 space-y-4">
              <div>
                <label className="text-sm font-medium text-slate-700">Assigned User</label>
                <select
                  value={assignedUserId}
                  onChange={(e) => setAssignedUserId(e.target.value)}
                  className="mt-1 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-slate-500"
                >
                  <option value="">Unassigned</option>
                  {users.map((user) => (
                    <option key={user.id} value={user.id}>
                      {user.fullName} ({user.email})
                    </option>
                  ))}
                </select>
              </div>
              <PrimaryButton onClick={saveAssignment} busy={savingSection === 'assignment'}>
                Save Assignment
              </PrimaryButton>
            </div>
          ) : (
            <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
              <ReadOnlyField label="Assigned User" value={assigneeName || 'Unassigned'} />
              <ReadOnlyField label="Assignment Mode" value={project.assignedUserId ? 'Assigned' : 'Needs assignment'} />
            </div>
          )}
        </section>

        <section className="rounded-xl border border-slate-200 bg-white p-5">
          <SectionHeader
            title="Notes"
            description="Capture context, pending actions, and internal follow-up."
            editable={role === 'Admin'}
            editing={editModes.notes}
            onEdit={() => setEditModes((current) => ({ ...current, notes: true }))}
            onCancel={() => {
              resetNotes()
              setEditModes((current) => ({ ...current, notes: false }))
            }}
          />

          {editModes.notes ? (
            <div className="mt-4 space-y-4">
              <textarea
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
                rows={8}
              />
              <PrimaryButton onClick={saveNotes} busy={savingSection === 'notes'}>
                Save Notes
              </PrimaryButton>
            </div>
          ) : (
            <div className="mt-4 rounded-lg bg-slate-50 p-4 text-sm text-slate-700">{project.notes || 'No notes added yet.'}</div>
          )}
        </section>
      </div>

      <section className="rounded-xl border border-slate-200 bg-white p-5">
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div>
            <div className="text-base font-semibold text-slate-900">Credentials Access</div>
            <div className="text-sm text-slate-500">Sensitive credentials are separated into a dedicated protected flow to reduce accidental exposure on the main page.</div>
          </div>
          <Link to={`/projects/${project.id}/credentials`} className="rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800">
            Open Credentials
          </Link>
        </div>
        <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-3">
          <ReadOnlyField label="Email" value={project.email} />
          <ReadOnlyField label="ECPS Account Id" value={project.ecpsAccountId} />
          <ReadOnlyField label="ECPS Application Id" value={project.ecpsApplicationId} />
        </div>
      </section>
    </div>
  )
}

function SectionHeader({
  title,
  description,
  editable,
  editing,
  onEdit,
  onCancel,
}: {
  title: string
  description: string
  editable: boolean
  editing: boolean
  onEdit: () => void
  onCancel: () => void
}) {
  return (
    <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
      <div>
        <div className="text-base font-semibold text-slate-900">{title}</div>
        <div className="text-sm text-slate-500">{description}</div>
      </div>
      {editable &&
        (editing ? (
          <button onClick={onCancel} className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50">
            Cancel
          </button>
        ) : (
          <button onClick={onEdit} className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50">
            Edit
          </button>
        ))}
    </div>
  )
}

function EditableField({ label, value, onChange }: { label: string; value: string; onChange: (value: string) => void }) {
  return (
    <div>
      <label className="text-sm font-medium text-slate-700">{label}</label>
      <input value={value} onChange={(e) => onChange(e.target.value)} className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500" />
    </div>
  )
}

function EditableDateField({ label, value, onChange }: { label: string; value: string; onChange: (value: string) => void }) {
  return (
    <div>
      <label className="text-sm font-medium text-slate-700">{label}</label>
      <input value={value} onChange={(e) => onChange(e.target.value)} className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500" type="date" />
    </div>
  )
}

function ReadOnlyField({ label, value }: { label: string; value?: string | null }) {
  return (
    <div>
      <div className="text-xs font-medium uppercase tracking-wide text-slate-500">{label}</div>
      <div className="mt-1 text-sm text-slate-800">{value || '—'}</div>
    </div>
  )
}

function ReadOnlyLinkField({ label, value }: { label: string; value?: string | null }) {
  return (
    <div>
      <div className="text-xs font-medium uppercase tracking-wide text-slate-500">{label}</div>
      <div className="mt-1 text-sm">
        {value ? (
          <a href={value} target="_blank" rel="noreferrer" className="text-slate-900 underline underline-offset-4">
            Open shared folder
          </a>
        ) : (
          <span className="text-slate-800">—</span>
        )}
      </div>
    </div>
  )
}

function PrimaryButton({ children, onClick, busy }: { children: ReactNode; onClick: () => void; busy: boolean }) {
  return (
    <button
      onClick={onClick}
      disabled={busy}
      className="rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
    >
      {busy ? 'Saving...' : children}
    </button>
  )
}

function MetaPill({ label, value }: { label: string; value: string }) {
  return (
    <span className="inline-flex items-center rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-700">
      {label}: {value}
    </span>
  )
}

function formatDate(value?: string | null) {
  if (!value) return '—'
  return new Date(value).toLocaleDateString()
}

