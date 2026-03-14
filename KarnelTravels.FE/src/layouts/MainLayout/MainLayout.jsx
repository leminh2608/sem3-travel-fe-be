import { Outlet, Link, useLocation } from 'react-router-dom';
import { useState, useEffect, useRef } from 'react';
import { useAuth } from '@/context/AuthContext/AuthContext';
import {
  Plane,
  Search,
  Menu,
  X,
  ChevronDown,
  User,
  LogOut,
  Heart,
  MapPin,
  Palmtree,
  Bus,
  Building2,
  Utensils,
  Home,
  Info,
  Phone,
  Calendar
} from 'lucide-react';

const MainLayout = () => {
  const location = useLocation();
  const { user, isAuthenticated, logout } = useAuth();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);
  const [activeDropdown, setActiveDropdown] = useState(null);

  useEffect(() => {
    const handleScroll = () => setIsScrolled(window.scrollY > 20);
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  useEffect(() => {
    setIsMobileMenuOpen(false);
    setActiveDropdown(null);
  }, [location.pathname]);

  const navItems = [
    { name: 'Trang chủ', path: '/', icon: Home },
    { name: 'Giới thiệu', path: '/about', icon: Info },
    { name: 'Tìm kiếm', path: '/search', icon: Search },
    {
      name: 'Thông tin',
      path: '/info',
      icon: Info,
      hasDropdown: true,
      dropdownItems: [
        { name: 'Điểm du lịch', path: '/info/destinations', icon: MapPin },
        { name: 'Tour Du lịch', path: '/info/tours', icon: Palmtree },
        { name: 'Phương tiện', path: '/info/transport', icon: Bus },
        { name: 'Khách sạn', path: '/info/hotels', icon: Building2 },
        { name: 'Nhà hàng', path: '/info/restaurants', icon: Utensils },
        { name: 'Resort', path: '/info/resorts', icon: Palmtree },
      ]
    },
    { name: 'Liên hệ', path: '/contact', icon: Phone },
  ];

  const handleDropdownToggle = (index) => {
    setActiveDropdown(activeDropdown === index ? null : index);
  };

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      {/* Header */}
      <header
        className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${
          isScrolled
            ? 'bg-white/95 backdrop-blur-md shadow-lg py-3'
            : 'bg-transparent py-5'
        }`}
      >
        <div className="container mx-auto px-4">
          <div className="flex items-center justify-between">
            {/* Logo */}
            <Link to="/" className="flex items-center gap-2 group">
              <div className="w-10 h-10 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-xl flex items-center justify-center shadow-lg group-hover:scale-110 transition-transform">
                <Plane className="w-5 h-5 text-white" />
              </div>
              <span className={`font-bold text-xl transition-colors ${
                isScrolled ? 'text-gray-800' : 'text-white'
              }`}>
                KarnelTravels
              </span>
            </Link>

            {/* Desktop Navigation */}
            <nav className="hidden lg:flex items-center gap-1">
              {navItems.map((item, index) => (
                <div key={item.path} className="relative">
                  {item.hasDropdown ? (
                    <button
                      onClick={() => handleDropdownToggle(index)}
                      className={`flex items-center gap-1.5 px-4 py-2.5 rounded-lg font-medium transition-all ${
                        isScrolled
                          ? 'text-gray-700 hover:bg-indigo-50 hover:text-indigo-600'
                          : 'text-white/90 hover:text-white hover:bg-white/10'
                      } ${location.pathname === item.path ? 'bg-indigo-500/20 text-indigo-600' : ''}`}
                    >
                      {item.name}
                      <ChevronDown className={`w-4 h-4 transition-transform ${
                        activeDropdown === index ? 'rotate-180' : ''
                      }`} />
                    </button>
                  ) : (
                    <Link
                      to={item.path}
                      className={`flex items-center gap-1.5 px-4 py-2.5 rounded-lg font-medium transition-all ${
                        isScrolled
                          ? 'text-gray-700 hover:bg-indigo-50 hover:text-indigo-600'
                          : 'text-white/90 hover:text-white hover:bg-white/10'
                      } ${location.pathname === item.path ? 'bg-indigo-500/20 text-indigo-600' : ''}`}
                    >
                      {item.name}
                    </Link>
                  )}

                  {/* Mega Menu Dropdown */}
                  {item.hasDropdown && activeDropdown === index && (
                    <div className="absolute top-full left-0 mt-2 w-[600px] bg-white rounded-2xl shadow-2xl border border-gray-100 overflow-hidden animate-in fade-in slide-in-from-top-2 duration-200">
                      <div className="grid grid-cols-2 gap-1 p-2">
                        {item.dropdownItems.map((dropdownItem) => (
                          <Link
                            key={dropdownItem.path}
                            to={dropdownItem.path}
                            className="flex items-center gap-3 p-4 rounded-xl hover:bg-indigo-50 transition-colors group"
                          >
                            <div className="w-10 h-10 bg-gradient-to-br from-indigo-100 to-purple-100 rounded-lg flex items-center justify-center group-hover:scale-110 transition-transform">
                              <dropdownItem.icon className="w-5 h-5 text-indigo-600" />
                            </div>
                            <div>
                              <p className="font-semibold text-gray-800">{dropdownItem.name}</p>
                              <p className="text-xs text-gray-500">Khám phá ngay</p>
                            </div>
                          </Link>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              ))}
            </nav>

            {/* Right Section */}
            <div className="flex items-center gap-3">
              {/* Search Button */}
              <Link
                to="/search"
                className={`p-2.5 rounded-full transition-all ${
                  isScrolled
                    ? 'bg-gray-100 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600'
                    : 'bg-white/10 text-white hover:bg-white/20'
                }`}
              >
                <Search className="w-5 h-5" />
              </Link>

              {isAuthenticated ? (
                <div className="flex items-center gap-3">
                  {/* Wishlist */}
                  <Link
                    to="/wishlist"
                    className={`p-2.5 rounded-full transition-all ${
                      isScrolled
                        ? 'bg-gray-100 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600'
                        : 'bg-white/10 text-white hover:bg-white/20'
                    }`}
                  >
                    <Heart className="w-5 h-5" />
                  </Link>

                  {/* Bookings */}
                  <Link
                    to="/bookings"
                    className={`p-2.5 rounded-full transition-all ${
                      isScrolled
                        ? 'bg-gray-100 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600'
                        : 'bg-white/10 text-white hover:bg-white/20'
                    }`}
                  >
                    <Calendar className="w-5 h-5" />
                  </Link>

                  {/* User Menu */}
                  <div className="relative group">
                    <button className="flex items-center gap-2 pl-2 pr-1 py-1.5 rounded-full bg-indigo-500 hover:bg-indigo-600 transition-colors">
                      <div className="w-8 h-8 bg-white/20 rounded-full flex items-center justify-center">
                        <User className="w-4 h-4 text-white" />
                      </div>
                      <span className="text-white font-medium text-sm pr-1">{user?.fullName?.split(' ')[0]}</span>
                    </button>

                    {/* User Dropdown */}
                    <div className="absolute right-0 top-full mt-2 w-56 bg-white rounded-2xl shadow-2xl border border-gray-100 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200">
                      <div className="p-3 border-b border-gray-100">
                        <p className="font-semibold text-gray-800">{user?.fullName}</p>
                        <p className="text-sm text-gray-500">{user?.email}</p>
                      </div>
                      <div className="p-2">
                        <Link
                          to="/profile"
                          className="flex items-center gap-3 px-3 py-2.5 rounded-xl hover:bg-gray-50 text-gray-700 transition-colors"
                        >
                          <User className="w-5 h-5" />
                          <span>Hồ sơ</span>
                        </Link>
                        <Link
                          to="/bookings"
                          className="flex items-center gap-3 px-3 py-2.5 rounded-xl hover:bg-gray-50 text-gray-700 transition-colors"
                        >
                          <Calendar className="w-5 h-5" />
                          <span>Đơn đặt</span>
                        </Link>
                        <button
                          onClick={logout}
                          className="w-full flex items-center gap-3 px-3 py-2.5 rounded-xl hover:bg-red-50 text-red-600 transition-colors"
                        >
                          <LogOut className="w-5 h-5" />
                          <span>Đăng xuất</span>
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="flex items-center gap-2">
                  <Link
                    to="/login"
                    className={`px-5 py-2.5 rounded-full font-medium transition-all ${
                      isScrolled
                        ? 'text-gray-700 hover:bg-gray-100'
                        : 'text-white hover:bg-white/10'
                    }`}
                  >
                    Đăng nhập
                  </Link>
                  <Link
                    to="/register"
                    className="px-5 py-2.5 bg-indigo-500 hover:bg-indigo-600 text-white font-medium rounded-full transition-all hover:shadow-lg hover:shadow-indigo-500/30"
                  >
                    Đăng ký
                  </Link>
                </div>
              )}

              {/* Mobile Menu Toggle */}
              <button
                onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                className={`lg:hidden p-2.5 rounded-lg transition-all ${
                  isScrolled
                    ? 'bg-gray-100 text-gray-700'
                    : 'bg-white/10 text-white'
                }`}
              >
                {isMobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
              </button>
            </div>
          </div>
        </div>

        {/* Mobile Menu */}
        <div
          className={`lg:hidden absolute top-full left-0 right-0 bg-white shadow-2xl transition-all duration-300 ${
            isMobileMenuOpen ? 'opacity-100 visible' : 'opacity-0 invisible'
          }`}
        >
          <nav className="container mx-auto px-4 py-4 space-y-2">
            {navItems.map((item) => (
              <div key={item.path}>
                {item.hasDropdown ? (
                  <>
                    <button
                      onClick={() => handleDropdownToggle(navItems.indexOf(item))}
                      className="w-full flex items-center justify-between px-4 py-3 rounded-xl text-gray-700 hover:bg-indigo-50"
                    >
                      <span className="flex items-center gap-3 font-medium">
                        <item.icon className="w-5 h-5" />
                        {item.name}
                      </span>
                      <ChevronDown className={`w-5 h-5 transition-transform ${
                        activeDropdown === navItems.indexOf(item) ? 'rotate-180' : ''
                      }`} />
                    </button>
                    {activeDropdown === navItems.indexOf(item) && (
                      <div className="pl-6 mt-2 space-y-1">
                        {item.dropdownItems.map((dropdownItem) => (
                          <Link
                            key={dropdownItem.path}
                            to={dropdownItem.path}
                            className="flex items-center gap-3 px-4 py-3 rounded-xl text-gray-600 hover:bg-indigo-50"
                          >
                            <dropdownItem.icon className="w-5 h-5" />
                            {dropdownItem.name}
                          </Link>
                        ))}
                      </div>
                    )}
                  </>
                ) : (
                  <Link
                    to={item.path}
                    className={`flex items-center gap-3 px-4 py-3 rounded-xl font-medium ${
                      location.pathname === item.path
                        ? 'bg-indigo-50 text-indigo-600'
                        : 'text-gray-700 hover:bg-indigo-50'
                    }`}
                  >
                    <item.icon className="w-5 h-5" />
                    {item.name}
                  </Link>
                )}
              </div>
            ))}
          </nav>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 pt-20">
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="bg-gray-900 text-white">
        <div className="container mx-auto px-4 py-16">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-10">
            {/* Brand */}
            <div>
              <div className="flex items-center gap-2 mb-6">
                <div className="w-10 h-10 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-xl flex items-center justify-center">
                  <Plane className="w-5 h-5 text-white" />
                </div>
                <span className="font-bold text-xl">KarnelTravels</span>
              </div>
              <p className="text-gray-400 mb-6">
                Đồng hành cùng bạn trên mọi hành trình khám phá Việt Nam và thế giới.
              </p>
              <div className="flex gap-4">
                {/* Social icons */}
              </div>
            </div>

            {/* Quick Links */}
            <div>
              <h4 className="font-semibold text-lg mb-6">Liên kết nhanh</h4>
              <ul className="space-y-3">
                {['Trang chủ', 'Giới thiệu', 'Tìm kiếm', 'Liên hệ'].map((item) => (
                  <li key={item}>
                    <Link to="/" className="text-gray-400 hover:text-white transition-colors">
                      {item}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>

            {/* Services */}
            <div>
              <h4 className="font-semibold text-lg mb-6">Dịch vụ</h4>
              <ul className="space-y-3">
                {['Tour du lịch', 'Khách sạn', 'Nhà hàng', 'Resort', 'Thuê xe'].map((item) => (
                  <li key={item}>
                    <Link to="/info" className="text-gray-400 hover:text-white transition-colors">
                      {item}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>

            {/* Contact */}
            <div>
              <h4 className="font-semibold text-lg mb-6">Liên hệ</h4>
              <ul className="space-y-4 text-gray-400">
                <li className="flex items-start gap-3">
                  <MapPin className="w-5 h-5 mt-0.5" />
                  <span>123 Đường ABC, Quận 1, TP.HCM</span>
                </li>
                <li className="flex items-center gap-3">
                  <Phone className="w-5 h-5" />
                  <span>0123 456 789</span>
                </li>
              </ul>
            </div>
          </div>

          <div className="border-t border-gray-800 mt-12 pt-8 text-center text-gray-500">
            <p>&copy; 2026 KarnelTravels. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default MainLayout;
