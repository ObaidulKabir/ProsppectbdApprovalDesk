import { NavLink } from 'react-router-dom'
import { useAppSelector } from '../../app/hooks'
import clsx from 'clsx'

const navItem = (isActive: boolean) =>
  clsx(
    'flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium',
    isActive ? 'bg-slate-200 text-slate-900' : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900',
  )

export function Sidebar() {
  const role = useAppSelector((s) => s.auth.user?.role)

  return (
    <aside className="hidden w-64 flex-col border-r border-slate-200 bg-white p-4 md:flex">
      <div className="mb-6">
        <div className="text-base font-semibold text-slate-900">ProspectbdApprovalDesk</div>
        <div className="text-xs text-slate-500">RAJUK / ECPS Workflow</div>
      </div>

      <nav className="flex flex-col gap-1">
        <NavLink to="/" className={({ isActive }) => navItem(isActive)} end>
          Dashboard
        </NavLink>
        <NavLink to="/projects" className={({ isActive }) => navItem(isActive)}>
          Projects
        </NavLink>
        {role === 'Admin' && (
          <>
            <NavLink to="/users" className={({ isActive }) => navItem(isActive)}>
              Users
            </NavLink>
            <NavLink to="/activity" className={({ isActive }) => navItem(isActive)}>
              Activity Logs
            </NavLink>
          </>
        )}
      </nav>

      <div className="mt-auto pt-6 text-xs text-slate-500">© Prospectbd</div>
    </aside>
  )
}

