import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return api(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = localStorage.getItem('refreshToken');

      if (!refreshToken) {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
        return Promise.reject(error);
      }

      try {
        const response = await axios.post(`${API_BASE_URL}/auth/refresh-token`, {
          refreshToken,
        });

        if (response.data.success) {
          const { token: newToken, refreshToken: newRefreshToken } = response.data.data;
          localStorage.setItem('token', newToken);
          localStorage.setItem('refreshToken', newRefreshToken);
          processQueue(null, newToken);
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return api(originalRequest);
        } else {
          processQueue(new Error(response.data.message));
          localStorage.removeItem('token');
          localStorage.removeItem('refreshToken');
          window.location.href = '/login';
        }
      } catch (refreshError) {
        processQueue(refreshError);
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

export const authService = {
  login: async (credentials) => {
    try {
      const response = await api.post('/auth/login', credentials);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Login failed' };
    }
  },

  register: async (userData) => {
    try {
      const response = await api.post('/auth/register', userData);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Registration failed' };
    }
  },

  logout: async () => {
    try {
      await api.post('/auth/logout');
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
    }
  },

  getCurrentUser: async () => {
    try {
      const response = await api.get('/auth/me');
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to get user' };
    }
  },

  changePassword: async (passwordData) => {
    try {
      const response = await api.put('/auth/change-password', passwordData);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Password change failed' };
    }
  },

  refreshToken: async (refreshToken) => {
    try {
      const response = await api.post('/auth/refresh-token', { refreshToken });
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Token refresh failed' };
    }
  },
};

export const tourService = {
  getAll: async (params) => {
    try {
      const response = await api.get('/tours', { params });
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to fetch tours' };
    }
  },

  getById: async (id) => {
    try {
      const response = await api.get(`/tours/${id}`);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to fetch tour' };
    }
  },

  create: async (data) => {
    try {
      const response = await api.post('/tours', data);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to create tour' };
    }
  },

  update: async (id, data) => {
    try {
      const response = await api.put(`/tours/${id}`, data);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to update tour' };
    }
  },

  delete: async (id) => {
    try {
      const response = await api.delete(`/tours/${id}`);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to delete tour' };
    }
  },
};

export const bookingService = {
  getAll: async (params) => {
    try {
      const response = await api.get('/bookings', { params });
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to fetch bookings' };
    }
  },

  create: async (data) => {
    try {
      const response = await api.post('/bookings', data);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to create booking' };
    }
  },

  cancel: async (id) => {
    try {
      const response = await api.put(`/bookings/${id}/cancel`);
      return response.data;
    } catch (error) {
      return error.response?.data || { success: false, message: 'Failed to cancel booking' };
    }
  },
};

export default api;
