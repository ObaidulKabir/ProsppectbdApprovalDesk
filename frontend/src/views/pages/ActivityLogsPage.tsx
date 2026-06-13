import { useEffect, useMemo, useState } from 'react'
import toast from 'react-hot-toast'
import { api } from '../../api/client'
import type { ApiResponse, PagedResult } from '../../api/types'
import { useAppSelector } from '../../app/hooks'

type ActivityLogDto = {
  id: string
  userId?: string | null
  action: string
  entityName?: string | null
  entityId?: string | null
  timestamp: string
  description?: string | null
}

export function ActivityLogsPage() {
  const role = useAppSelector((s) => s.auth.user?.role)
  const [page, setPage] = useState(1)
  const [data, setData] = useState<PagedResult<ActivityLogDto> | null>(null)
  const pageSize = 20

  const query = useMemo(() => ({ page, pageSize }), [page, pageSize])

  useEffect(() => {
    if (role !== 'Admin') return
    api
      .get<ApiResponse<PagedResult<ActivityLogDto>>>('/api/activity-logs', { params: query })
      .then((res) => {
        if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Failed to load logs')
        setData(res.data.data)
      })
      .catch((err) => toast.error(err instanceof Error ? err.message : 'Failed to load logs'))
  }, [query, role])

  if (role !== 'Admin') return <div className="text-sm text-slate-600">Not authorized.</div>

  return (
    <div className="space-y-4">
      <div>
        <div className="text-lg font-semibold text-slate-900">Activity Logs</div>
        <div className="text-sm text-slate-500">Audit trail of key actions</div>
      </div>

      <div className="overflow-hidden rounded-lg border border-slate-200 bg-white">
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-50 text-xs uppercase text-slate-500">
            <tr>
              <th className="px-4 py-3">Time</th>
              <th className="px-4 py-3">Action</th>
              <th className="px-4 py-3">Entity</th>
              <th className="px-4 py-3">Description</th>
            </tr>
          </thead>
          <tbody>
            {data?.items?.map((l) => (
              <tr key={l.id} className="border-t border-slate-100">
                <td className="px-4 py-3 text-slate-700">{new Date(l.timestamp).toLocaleString()}</td>
                <td className="px-4 py-3 font-medium text-slate-900">{l.action}</td>
                <td className="px-4 py-3 text-slate-700">
                  {l.entityName} {l.entityId ? `#${l.entityId.slice(0, 8)}` : ''}
                </td>
                <td className="px-4 py-3 text-slate-700">{l.description || '—'}</td>
              </tr>
            ))}
            {!data?.items?.length && (
              <tr>
                <td className="px-4 py-8 text-center text-slate-500" colSpan={4}>
                  No activity yet
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

