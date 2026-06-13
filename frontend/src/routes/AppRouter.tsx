import { Navigate, Route, Routes } from 'react-router-dom'
import { LoginPage } from '../views/LoginPage'
import { ProtectedRoute } from './ProtectedRoute'
import { DashboardLayout } from '../views/layout/DashboardLayout'
import { DashboardPage } from '../views/pages/DashboardPage'
import { ProjectsPage } from '../views/pages/ProjectsPage'
import { ProjectCreatePage } from '../views/pages/ProjectCreatePage'
import { ProjectDetailsPage } from '../views/pages/ProjectDetailsPage'
import { ProjectCredentialsPage } from '../views/pages/ProjectCredentialsPage'
import { UsersPage } from '../views/pages/UsersPage'
import { ActivityLogsPage } from '../views/pages/ActivityLogsPage'

export function AppRouter() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <DashboardLayout />
          </ProtectedRoute>
        }
      >
        <Route index element={<DashboardPage />} />
        <Route path="projects" element={<ProjectsPage />} />
        <Route path="projects/new" element={<ProjectCreatePage />} />
        <Route path="projects/:id/credentials" element={<ProjectCredentialsPage />} />
        <Route path="projects/:id" element={<ProjectDetailsPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="activity" element={<ActivityLogsPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

