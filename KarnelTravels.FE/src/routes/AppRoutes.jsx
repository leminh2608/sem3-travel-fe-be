import { Navigate, Routes, Route } from 'react-router-dom';

// Layouts
import MainLayout from '@/layouts/MainLayout/MainLayout';
import AdminLayout from '@/layouts/AdminLayout/AdminLayout';
import AuthLayout from '@/layouts/AuthLayout/AuthLayout';

// Auth Pages
import LoginPage from '@/pages/auth/LoginPage/LoginPage';
import ForbiddenPage from '@/pages/error/ForbiddenPage/ForbiddenPage';

// Protected Route Component
import ProtectedRoute from '@/components/common/ProtectedRoute/ProtectedRoute';

// Placeholder Pages
const HomePage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <div className="text-center">
      <h1 className="text-5xl font-bold text-gray-800 mb-4">Chào mừng đến với KarnelTravels</h1>
      <p className="text-xl text-gray-600">Hành trình của bạn bắt đầu tại đây</p>
    </div>
  </div>
);

const AboutPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Giới thiệu</h1>
  </div>
);

const SearchPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Tìm kiếm</h1>
  </div>
);

const ContactPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Liên hệ</h1>
  </div>
);

const ProfilePage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Hồ sơ</h1>
  </div>
);

const BookingsPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Đơn đặt tour</h1>
  </div>
);

const WishlistPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Danh sách yêu thích</h1>
  </div>
);

const DestinationsPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Điểm du lịch</h1>
  </div>
);

const ToursPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Tour du lịch</h1>
  </div>
);

const HotelsPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Khách sạn</h1>
  </div>
);

const RestaurantsPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Nhà hàng</h1>
  </div>
);

const ResortsPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Resort</h1>
  </div>
);

const TransportsPage = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Phương tiện</h1>
  </div>
);

// Admin Pages
const AdminDashboard = () => (
  <div>
    <h1 className="text-3xl font-bold text-gray-800 mb-6">Dashboard</h1>
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div className="bg-white rounded-2xl p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm text-gray-500">Tổng users</p>
            <p className="text-3xl font-bold text-gray-800">1,234</p>
          </div>
          <div className="w-12 h-12 bg-indigo-100 rounded-xl flex items-center justify-center">
            <span className="text-2xl">👥</span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-2xl p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm text-gray-500">Tổng bookings</p>
            <p className="text-3xl font-bold text-gray-800">567</p>
          </div>
          <div className="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center">
            <span className="text-2xl">📅</span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-2xl p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm text-gray-500">Doanh thu</p>
            <p className="text-3xl font-bold text-gray-800">$12,345</p>
          </div>
          <div className="w-12 h-12 bg-yellow-100 rounded-xl flex items-center justify-center">
            <span className="text-2xl">💰</span>
          </div>
        </div>
      </div>
      <div className="bg-white rounded-2xl p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm text-gray-500">Tours</p>
            <p className="text-3xl font-bold text-gray-800">89</p>
          </div>
          <div className="w-12 h-12 bg-purple-100 rounded-xl flex items-center justify-center">
            <span className="text-2xl">✈️</span>
          </div>
        </div>
      </div>
    </div>
  </div>
);

const AdminUsers = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Quản lý người dùng</h1>
  </div>
);

const AdminBookings = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Quản lý đặt tour</h1>
  </div>
);

const AdminDestinations = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Quản lý điểm du lịch</h1>
  </div>
);

const AdminTours = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Quản lý tour</h1>
  </div>
);

const AdminHotels = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Quản lý khách sạn</h1>
  </div>
);

const AdminRestaurants = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Quản lý nhà hàng</h1>
  </div>
);

const AdminResorts = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Quản lý resort</h1>
  </div>
);

const AdminTransports = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Quản lý phương tiện</h1>
  </div>
);

const AdminReports = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Báo cáo & Thống kê</h1>
  </div>
);

const AdminSettings = () => (
  <div className="min-h-[80vh] flex items-center justify-center">
    <h1 className="text-4xl font-bold text-gray-800">Cài đặt</h1>
  </div>
);

const AppRoutes = () => {
  return (
    <Routes>
      {/* ==================== AUTH LAYOUT ==================== */}
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<LoginPage />} />
      </Route>

      {/* ==================== MAIN LAYOUT (User) ==================== */}
      <Route element={<MainLayout />}>
        <Route element={<ProtectedRoute />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/about" element={<AboutPage />} />
          <Route path="/search" element={<SearchPage />} />
          <Route path="/contact" element={<ContactPage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/bookings" element={<BookingsPage />} />
          <Route path="/wishlist" element={<WishlistPage />} />

          {/* Info Pages */}
          <Route path="/info/destinations" element={<DestinationsPage />} />
          <Route path="/info/tours" element={<ToursPage />} />
          <Route path="/info/hotels" element={<HotelsPage />} />
          <Route path="/info/restaurants" element={<RestaurantsPage />} />
          <Route path="/info/resorts" element={<ResortsPage />} />
          <Route path="/info/transports" element={<TransportsPage />} />
        </Route>
      </Route>

      {/* ==================== ADMIN LAYOUT ==================== */}
      <Route element={<ProtectedRoute adminOnly />}>
        <Route element={<AdminLayout />}>
          <Route path="/admin" element={<AdminDashboard />} />
          <Route path="/admin/users" element={<AdminUsers />} />
          <Route path="/admin/bookings" element={<AdminBookings />} />
          <Route path="/admin/destinations" element={<AdminDestinations />} />
          <Route path="/admin/tours" element={<AdminTours />} />
          <Route path="/admin/hotels" element={<AdminHotels />} />
          <Route path="/admin/restaurants" element={<AdminRestaurants />} />
          <Route path="/admin/resorts" element={<AdminResorts />} />
          <Route path="/admin/transports" element={<AdminTransports />} />
          <Route path="/admin/reports" element={<AdminReports />} />
          <Route path="/admin/settings" element={<AdminSettings />} />
        </Route>
      </Route>

      {/* ==================== ERROR PAGES ==================== */}
      <Route path="/403" element={<ForbiddenPage />} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default AppRoutes;
