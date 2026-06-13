export type ApiResponse<T> = {
  success: boolean
  message?: string | null
  traceId?: string | null
  data?: T | null
  errors?: Record<string, string[]> | null
}

export type PagedResult<T> = {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}

export type UserRole = 'Admin' | 'User'

export type LoginRequest = {
  email: string
  password: string
}

export type LoginResponse = {
  accessToken: string
  expiresAt: string
  userId: string
  fullName: string
  email: string
  role: UserRole
}

export type ProjectStatus =
  | 'Draft'
  | 'EcpsAccountCreated'
  | 'ApplicationSubmitted'
  | 'UnderReview'
  | 'CorrectionRequired'
  | 'Resubmitted'
  | 'Approved'
  | 'Rejected'

export type ProjectListItemDto = {
  id: string
  projectCode: string
  projectName: string
  ownerName: string
  assignedUserId?: string | null
  status: ProjectStatus
  submissionDate?: string | null
  approvalDate?: string | null
  createdAt: string
  updatedAt?: string | null
}

export type ProjectDto = ProjectListItemDto & {
  projectArea?: string | null
  projectLocation?: string | null
  driveLink?: string | null
  contactName?: string | null
  contactNumber?: string | null
  email?: string | null
  emailPassword?: string | null
  ecpsAccountId?: string | null
  ecpsPassword?: string | null
  ecpsApplicationId?: string | null
  notes?: string | null
}

export type UserDto = {
  id: string
  fullName: string
  email: string
  phone?: string | null
  role: UserRole
  isActive: boolean
  createdAt: string
}

