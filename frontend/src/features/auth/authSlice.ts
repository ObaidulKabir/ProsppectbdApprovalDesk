import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import { api } from '../../api/client'
import type { ApiResponse, LoginRequest, LoginResponse, UserRole } from '../../api/types'

type AuthUser = {
  userId: string
  fullName: string
  email: string
  role: UserRole
}

type AuthState = {
  token: string | null
  user: AuthUser | null
  status: 'idle' | 'loading' | 'failed'
}

const initialState: AuthState = {
  token: localStorage.getItem('pad_token'),
  user: null,
  status: 'idle',
}

export const login = createAsyncThunk<LoginResponse, LoginRequest>(
  'auth/login',
  async (payload) => {
    const res = await api.post<ApiResponse<LoginResponse>>('/api/auth/login', payload)
    if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Login failed')
    return res.data.data
  },
)

export const fetchMe = createAsyncThunk<{ userId: string; role: UserRole }, void>(
  'auth/me',
  async () => {
    const res = await api.get<ApiResponse<{ userId: string; role: UserRole }>>('/api/auth/me')
    if (!res.data.success || !res.data.data) throw new Error(res.data.message || 'Failed to load profile')
    return res.data.data
  },
)

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout: (state) => {
      state.token = null
      state.user = null
      localStorage.removeItem('pad_token')
    },
    hydrateUserFromToken: (state) => {
      state.token = localStorage.getItem('pad_token')
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.status = 'loading'
      })
      .addCase(login.fulfilled, (state, action) => {
        state.status = 'idle'
        state.token = action.payload.accessToken
        state.user = {
          userId: action.payload.userId,
          fullName: action.payload.fullName,
          email: action.payload.email,
          role: action.payload.role,
        }
        localStorage.setItem('pad_token', action.payload.accessToken)
      })
      .addCase(login.rejected, (state) => {
        state.status = 'failed'
      })
      .addCase(fetchMe.pending, (state) => {
        state.status = 'loading'
      })
      .addCase(fetchMe.fulfilled, (state, action) => {
        state.status = 'idle'
        if (!state.user) state.user = { userId: action.payload.userId, fullName: '', email: '', role: action.payload.role }
        state.user.userId = action.payload.userId
        state.user.role = action.payload.role
      })
      .addCase(fetchMe.rejected, (state) => {
        state.status = 'failed'
        state.token = null
        state.user = null
        localStorage.removeItem('pad_token')
      })
  },
})

export const { logout, hydrateUserFromToken } = authSlice.actions
export default authSlice.reducer

