import { useEffect, useMemo, useState } from 'react'
import toast from 'react-hot-toast'
import { Link } from 'react-router-dom'
import { api } from '../../api/client'
import type { ApiResponse, PagedResult, ProjectListItemDto, ProjectStatus, UserDto } from '../../api/types'
import { useAppSelector } from '../../app/hooks'
import { ProjectStatusBadge, formatProjectStatus, projectStatusOptions } from '../components/ProjectStatusBadge'

export function ProjectsPage() {
  const role = useAppSelector((s) => s.auth.user?.role)
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const [data, setData] = useState<PagedResult<ProjectListItemDto> | null>(null)
  const [users, setUsers] = useState<UserDto[]>([])
  const [assignedUserId, setAssignedUserId] = useState('')
  const [status, setStatus] = useState<ProjectStatus | ''>('')
  const pageSize = 20

  const query = useMemo(
    () => ({
      page,
      pageSize,
      search: search || undefined,
      assignedUserId: role === 'Admin' && assignedUserId ? assignedUserId : undefined,
      status: status || undefined,
    }),
    [assignedUserId, page, pageSize, role, search, status],
  )

  const reload = () =>
    api
      .get<ApiResponse<PagedResult<ProjectListItemDto>>>('/api/projects', { params: query })
      .then((res) => {
        if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Failed to load projects')
        setData(res.data.data)
      })
      .catch((err) => toast.error(err instanceof Error ? err.message : 'Failed to load projects'))

  useEffect(() => {
    reload()
  }, [query])

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

  const assigneeMap = useMemo(() => new Map(users.map((user) => [user.id, user.fullName])), [users])

  return (
    <div className="space-y-5">
      <div className="flex flex-col gap-4 xl:flex-row xl:items-end xl:justify-between">
        <div>
          <div className="text-lg font-semibold text-slate-900">Projects Queue</div>
          <div className="text-sm text-slate-500">Track workload, identify the next project to act on, and jump quickly into admin tasks.</div>
        </div>
        <div className="flex flex-col gap-3 md:flex-row md:items-center">
          {role === 'Admin' && (
            <Link to="/projects/new" className="rounded-md bg-slate-900 px-3 py-2 text-center text-sm font-medium text-white hover:bg-slate-800">
              New Project
            </Link>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 gap-3 rounded-xl border border-slate-200 bg-white p-4 lg:grid-cols-[minmax(0,1.2fr)_220px_220px]">
        <div>
          <label className="text-sm font-medium text-slate-700">Search</label>
          <input
            value={search}
            onChange={(e) => {
              setPage(1)
              setSearch(e.target.value)
            }}
            placeholder="Search by code, name, or owner…"
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500"
          />
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700">Status</label>
          <select
            value={status}
            onChange={(e) => {
              setPage(1)
              setStatus(e.target.value as ProjectStatus | '')
            }}
            className="mt-1 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-slate-500"
          >
            <option value="">All statuses</option>
            {projectStatusOptions.map((option) => (
              <option key={option} value={option}>
                {formatProjectStatus(option)}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="text-sm font-medium text-slate-700">Assigned User</label>
          <select
            value={assignedUserId}
            onChange={(e) => {
              setPage(1)
              setAssignedUserId(e.target.value)
            }}
            disabled={role !== 'Admin'}
            className="mt-1 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-slate-500 disabled:bg-slate-100"
          >
            <option value="">All assignees</option>
            {users.map((user) => (
              <option key={user.id} value={user.id}>
                {user.fullName}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="overflow-hidden rounded-xl border border-slate-200 bg-white">
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
            <tr>
              <th className="px-4 py-3">Project</th>
              <th className="px-4 py-3">Owner</th>
              <th className="px-4 py-3">Status</th>
              <th className="px-4 py-3">Assignee</th>
              <th className="px-4 py-3">Updated</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {data?.items?.map((project) => {
              const assignee = project.assignedUserId ? assigneeMap.get(project.assignedUserId) : null

              return (
                <tr key={project.id} className="border-t border-slate-100 align-top">
                  <td className="px-4 py-4">
                    <div className="font-medium text-slate-900">{project.projectName}</div>
                    <div className="mt-1 text-xs text-slate-500">{project.projectCode}</div>
                  </td>
                  <td className="px-4 py-4 text-slate-700">{project.ownerName}</td>
                  <td className="px-4 py-4">
                    <ProjectStatusBadge status={project.status} />
                  </td>
                  <td className="px-4 py-4 text-slate-700">{assignee || 'Unassigned'}</td>
                  <td className="px-4 py-4 text-slate-600">{formatDate(project.updatedAt || project.createdAt)}</td>
                  <td className="px-4 py-4">
                    <div className="flex justify-end gap-2">
                      <Link className="rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-700 hover:bg-slate-50" to={`/projects/${project.id}`}>
                        Open
                      </Link>
                      {role === 'Admin' && (
                        <Link className="rounded-md bg-slate-900 px-3 py-2 text-sm font-medium text-white hover:bg-slate-800" to={`/projects/${project.id}/credentials`}>
                          Credentials
                        </Link>
                      )}
                    </div>
                  </td>
                </tr>
              )
            })}
            {!data?.items?.length && (
              <tr>
                <td className="px-4 py-12 text-center text-slate-500" colSpan={6}>
                  No projects match the current filters.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <div className="flex items-center justify-between text-sm text-slate-600">
        <div>
          Page {data?.page ?? page} / {data ? Math.max(1, Math.ceil(data.totalCount / data.pageSize)) : '—'}
        </div>
        <div className="flex gap-2">
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page <= 1}
            className="rounded-md border border-slate-300 bg-white px-3 py-2 hover:bg-slate-50"
          >
            Prev
          </button>
          <button
            onClick={() => setPage((p) => p + 1)}
            className="rounded-md border border-slate-300 bg-white px-3 py-2 hover:bg-slate-50"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  )
}

function formatDate(value?: string | null) {
  if (!value) return '—'
  return new Date(value).toLocaleDateString()
}

