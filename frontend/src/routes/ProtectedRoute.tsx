import { ReactNode, useEffect } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../app/hooks'
import { fetchMe } from '../features/auth/authSlice'

export function ProtectedRoute({ children }: { children: ReactNode }) {
  const token = useAppSelector((s) => s.auth.token)
  const user = useAppSelector((s) => s.auth.user)
  const status = useAppSelector((s) => s.auth.status)
  const dispatch = useAppDispatch()
  const location = useLocation()

  useEffect(() => {
    if (token && !user && status !== 'loading') dispatch(fetchMe())
  }, [dispatch, status, token, user])

  if (!token) return <Navigate to="/login" replace state={{ from: location }} />
  if (!user) return <div className="p-6 text-sm text-slate-600">Checking session…</div>
  return <>{children}</>
}

