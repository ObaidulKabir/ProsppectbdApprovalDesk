import { StatCard } from '../components/StatCard'
import { useAppSelector } from '../../app/hooks'

export function DashboardPage() {
  const role = useAppSelector((s) => s.auth.user?.role)

  return (
    <div className="space-y-6">
      <div>
        <div className="text-lg font-semibold text-slate-900">{role === 'Admin' ? 'Admin Dashboard' : 'My Dashboard'}</div>
        <div className="text-sm text-slate-500">Key metrics and recent updates</div>
      </div>

      <div className="grid grid-cols-1 gap-4 md:grid-cols-4">
        <StatCard label="Total Projects" value="—" />
        <StatCard label="Pending Approvals" value="—" />
        <StatCard label="Approved" value="—" />
        <StatCard label="Recent Activity" value="—" />
      </div>

      <div className="rounded-lg border border-slate-200 bg-white p-4 text-sm text-slate-600">
        Connect to the API and PostgreSQL to populate dashboard cards.
      </div>
    </div>
  )
}

