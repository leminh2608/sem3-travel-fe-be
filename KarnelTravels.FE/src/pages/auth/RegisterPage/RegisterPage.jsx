import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useAuth } from '@/context/AuthContext/AuthContext';
import { Plane, Mail, Lock, Eye, EyeOff, Loader2, ArrowRight, User, Phone } from 'lucide-react';

const schema = yup.object().shape({
  fullName: yup.string().required('Họ tên là bắt buộc').min(2, 'Họ tên phải có ít nhất 2 ký tự'),
  email: yup.string().required('Email là bắt buộc').email('Email không hợp lệ'),
  phoneNumber: yup.string().optional(),
  password: yup.string().required('Mật khẩu là bắt buộc').min(6, 'Mật khẩu phải có ít nhất 6 ký tự'),
  confirmPassword: yup.string().required('Xác nhận mật khẩu là bắt buộc').oneOf([yup.ref('password')], 'Mật khẩu không khớp'),
});

const RegisterPage = () => {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: yupResolver(schema),
  });

  const onSubmit = async (data) => {
    setIsLoading(true);
    setError('');

    const userData = {
      fullName: data.fullName,
      email: data.email,
      password: data.password,
      phoneNumber: data.phoneNumber || null,
    };

    const result = await registerUser(userData);

    if (result.success) {
      navigate('/', { replace: true });
    } else {
      setError(result.message || 'Đăng ký thất bại. Vui lòng thử lại.');
    }

    setIsLoading(false);
  };

  return (
    <div className="min-h-screen flex items-center justify-center relative overflow-hidden bg-gradient-to-br from-pink-500 via-purple-600 to-indigo-600">
      {/* Animated Background Shapes */}
      <div className="absolute inset-0 overflow-hidden">
        <div className="absolute -top-40 -left-40 w-80 h-80 bg-white/10 rounded-full blur-3xl animate-pulse"></div>
        <div className="absolute -bottom-40 -right-40 w-80 h-80 bg-white/10 rounded-full blur-3xl animate-pulse delay-700"></div>
        <div className="absolute top-1/3 left-1/4 w-64 h-64 bg-pink-300/20 rounded-full blur-3xl"></div>
      </div>

      {/* Floating Shapes */}
      <div className="absolute top-20 right-20 w-4 h-4 bg-white/30 rounded-full animate-bounce delay-200"></div>
      <div className="absolute bottom-32 left-20 w-6 h-6 bg-white/20 rounded-full animate-bounce delay-400"></div>
      <div className="absolute top-1/2 right-1/3 w-3 h-3 bg-white/40 rounded-full animate-bounce delay-600"></div>

      {/* Register Card */}
      <div className="relative w-full max-w-md mx-4">
        <div className="backdrop-blur-xl bg-white/20 border border-white/30 rounded-3xl shadow-2xl p-8">
          {/* Logo */}
          <div className="text-center mb-6">
            <div className="inline-flex items-center justify-center w-16 h-16 bg-gradient-to-br from-pink-500 to-purple-600 rounded-2xl shadow-lg mb-4">
              <Plane className="w-8 h-8 text-white" />
            </div>
            <h1 className="text-3xl font-bold text-white mb-2">Đăng ký</h1>
            <p className="text-white/80">Tạo tài khoản để bắt đầu hành trình</p>
          </div>

          {/* Error Message */}
          {error && (
            <div className="mb-5 p-4 bg-red-500/20 border border-red-500/30 rounded-xl text-white text-sm">
              {error}
            </div>
          )}

          {/* Form */}
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            {/* Full Name Field */}
            <div>
              <label className="block text-sm font-medium text-white/90 mb-2">Họ và tên</label>
              <div className="relative">
                <User className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-white/60" />
                <input
                  type="text"
                  {...register('fullName')}
                  placeholder="Nhập họ và tên"
                  className="w-full pl-12 pr-4 py-3 bg-white/20 border border-white/30 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-white/50 focus:border-white/50 transition-all backdrop-blur-sm"
                />
              </div>
              {errors.fullName && (
                <p className="mt-1.5 text-sm text-red-200">{errors.fullName.message}</p>
              )}
            </div>

            {/* Email Field */}
            <div>
              <label className="block text-sm font-medium text-white/90 mb-2">Email</label>
              <div className="relative">
                <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-white/60" />
                <input
                  type="email"
                  {...register('email')}
                  placeholder="Nhập email của bạn"
                  className="w-full pl-12 pr-4 py-3 bg-white/20 border border-white/30 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-white/50 focus:border-white/50 transition-all backdrop-blur-sm"
                />
              </div>
              {errors.email && (
                <p className="mt-1.5 text-sm text-red-200">{errors.email.message}</p>
              )}
            </div>

            {/* Phone Number Field */}
            <div>
              <label className="block text-sm font-medium text-white/90 mb-2">Số điện thoại (tùy chọn)</label>
              <div className="relative">
                <Phone className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-white/60" />
                <input
                  type="tel"
                  {...register('phoneNumber')}
                  placeholder="Nhập số điện thoại"
                  className="w-full pl-12 pr-4 py-3 bg-white/20 border border-white/30 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-white/50 focus:border-white/50 transition-all backdrop-blur-sm"
                />
              </div>
              {errors.phoneNumber && (
                <p className="mt-1.5 text-sm text-red-200">{errors.phoneNumber.message}</p>
              )}
            </div>

            {/* Password Field */}
            <div>
              <label className="block text-sm font-medium text-white/90 mb-2">Mật khẩu</label>
              <div className="relative">
                <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-white/60" />
                <input
                  type={showPassword ? 'text' : 'password'}
                  {...register('password')}
                  placeholder="Tạo mật khẩu"
                  className="w-full pl-12 pr-12 py-3 bg-white/20 border border-white/30 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-white/50 focus:border-white/50 transition-all backdrop-blur-sm"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-4 top-1/2 -translate-y-1/2 text-white/60 hover:text-white transition-colors"
                >
                  {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
              {errors.password && (
                <p className="mt-1.5 text-sm text-red-200">{errors.password.message}</p>
              )}
            </div>

            {/* Confirm Password Field */}
            <div>
              <label className="block text-sm font-medium text-white/90 mb-2">Xác nhận mật khẩu</label>
              <div className="relative">
                <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-white/60" />
                <input
                  type={showConfirmPassword ? 'text' : 'password'}
                  {...register('confirmPassword')}
                  placeholder="Nhập lại mật khẩu"
                  className="w-full pl-12 pr-12 py-3 bg-white/20 border border-white/30 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-white/50 focus:border-white/50 transition-all backdrop-blur-sm"
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="absolute right-4 top-1/2 -translate-y-1/2 text-white/60 hover:text-white transition-colors"
                >
                  {showConfirmPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
              {errors.confirmPassword && (
                <p className="mt-1.5 text-sm text-red-200">{errors.confirmPassword.message}</p>
              )}
            </div>

            {/* Terms */}
            <div className="flex items-start gap-2">
              <input
                type="checkbox"
                required
                className="mt-1 w-4 h-4 rounded border-white/40 bg-white/20 text-indigo-500 focus:ring-indigo-500 focus:ring-offset-0"
              />
              <span className="text-sm text-white/80">
                Tôi đồng ý với{' '}
                <Link to="/terms" className="text-white font-medium hover:underline">Điều khoản</Link>
                {' '}và{' '}
                <Link to="/privacy" className="text-white font-medium hover:underline">Chính sách bảo mật</Link>
              </span>
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isLoading}
              className="w-full py-3.5 bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:scale-[1.02] disabled:opacity-70 disabled:cursor-not-allowed disabled:transform-none flex items-center justify-center gap-2"
            >
              {isLoading ? (
                <>
                  <Loader2 className="w-5 h-5 animate-spin" />
                  Đang đăng ký...
                </>
              ) : (
                <>
                  Tạo tài khoản
                  <ArrowRight className="w-5 h-5" />
                </>
              )}
            </button>
          </form>

          {/* Divider */}
          <div className="relative my-5">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-white/30"></div>
            </div>
            <div className="relative flex justify-center text-sm">
              <span className="px-4 bg-transparent text-white/60">hoặc</span>
            </div>
          </div>

          {/* Login Link */}
          <p className="text-center text-white/80">
            Đã có tài khoản?{' '}
            <Link to="/login" className="text-white font-semibold hover:underline">
              Đăng nhập ngay
            </Link>
          </p>
        </div>

        {/* Back to Home */}
        <div className="text-center mt-6">
          <Link to="/" className="inline-flex items-center gap-2 text-white/80 hover:text-white transition-colors">
            <ArrowRight className="w-4 h-4 rotate-180" />
            Quay về trang chủ
          </Link>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
