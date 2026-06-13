import { useEffect, useMemo, useState } from 'react'
import toast from 'react-hot-toast'
import { api } from '../../api/client'
import type { ApiResponse, PagedResult, UserDto } from '../../api/types'
import { useAppSelector } from '../../app/hooks'

export function UsersPage() {
  const role = useAppSelector((s) => s.auth.user?.role)
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const [data, setData] = useState<PagedResult<UserDto> | null>(null)
  const pageSize = 20

  const query = useMemo(() => ({ page, pageSize, search: search || undefined }), [page, pageSize, search])

  useEffect(() => {
    if (role !== 'Admin') return
    api
      .get<ApiResponse<PagedResult<UserDto>>>('/api/users', { params: query })
      .then((res) => {
        if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Failed to load users')
        setData(res.data.data)
      })
      .catch((err) => toast.error(err instanceof Error ? err.message : 'Failed to load users'))
  }, [query, role])

  if (role !== 'Admin') return <div className="text-sm text-slate-600">Not authorized.</div>

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <div className="text-lg font-semibold text-slate-900">Users</div>
          <div className="text-sm text-slate-500">Manage employees and access control</div>
        </div>
        <input
          value={search}
          onChange={(e) => {
            setPage(1)
            setSearch(e.target.value)
          }}
          placeholder="Search by name or email…"
          className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-slate-500 md:w-72"
        />
      </div>

      <div className="overflow-hidden rounded-lg border border-slate-200 bg-white">
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-50 text-xs uppercase text-slate-500">
            <tr>
              <th className="px-4 py-3">Name</th>
              <th className="px-4 py-3">Email</th>
              <th className="px-4 py-3">Role</th>
              <th className="px-4 py-3">Active</th>
            </tr>
          </thead>
          <tbody>
            {data?.items?.map((u) => (
              <tr key={u.id} className="border-t border-slate-100">
                <td className="px-4 py-3 font-medium text-slate-900">{u.fullName}</td>
                <td className="px-4 py-3 text-slate-700">{u.email}</td>
                <td className="px-4 py-3 text-slate-700">{u.role}</td>
                <td className="px-4 py-3 text-slate-700">{u.isActive ? 'Yes' : 'No'}</td>
              </tr>
            ))}
            {!data?.items?.length && (
              <tr>
                <td className="px-4 py-8 text-center text-slate-500" colSpan={4}>
                  No users found
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

