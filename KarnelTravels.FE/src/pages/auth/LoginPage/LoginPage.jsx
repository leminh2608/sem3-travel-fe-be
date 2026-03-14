import { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useAuth } from '@/context/AuthContext/AuthContext';
import { Mail, Lock, Eye, EyeOff, Loader2, ArrowRight, User } from 'lucide-react';

const schema = yup.object().shape({
  email: yup.string().required('Email là bắt buộc').email('Email không hợp lệ'),
  password: yup.string().required('Mật khẩu là bắt buộc').min(6, 'Mật khẩu phải có ít nhất 6 ký tự'),
});

const LoginPage = () => {
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const from = location.state?.from?.pathname || '/';

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

    const result = await login(data);

    if (result.success) {
      navigate(from, { replace: true });
    } else {
      setError(result.message || 'Đăng nhập thất bại. Vui lòng thử lại.');
    }

    setIsLoading(false);
  };

  return (
    <div className="backdrop-blur-xl bg-white/20 border border-white/30 rounded-3xl shadow-2xl p-8">
      {/* Header */}
      <div className="text-center mb-8">
        <h1 className="text-3xl font-bold text-white mb-2">Chào mừng trở lại!</h1>
        <p className="text-white/80">Đăng nhập để tiếp tục hành trình</p>
      </div>

      {/* Error Message */}
      {error && (
        <div className="mb-6 p-4 bg-red-500/20 border border-red-500/30 rounded-xl text-white text-sm">
          {error}
        </div>
      )}

      {/* Form */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        {/* Email Field */}
        <div>
          <label className="block text-sm font-medium text-white/90 mb-2">Email</label>
          <div className="relative">
            <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-white/60" />
            <input
              type="email"
              {...register('email')}
              placeholder="Nhập email của bạn"
              className="w-full pl-12 pr-4 py-3.5 bg-white/20 border border-white/30 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-white/50 focus:border-white/50 transition-all backdrop-blur-sm"
            />
          </div>
          {errors.email && (
            <p className="mt-2 text-sm text-red-200">{errors.email.message}</p>
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
              placeholder="Nhập mật khẩu"
              className="w-full pl-12 pr-12 py-3.5 bg-white/20 border border-white/30 rounded-xl text-white placeholder-white/50 focus:outline-none focus:ring-2 focus:ring-white/50 focus:border-white/50 transition-all backdrop-blur-sm"
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
            <p className="mt-2 text-sm text-red-200">{errors.password.message}</p>
          )}
        </div>

        {/* Remember Me & Forgot Password */}
        <div className="flex items-center justify-between text-sm">
          <label className="flex items-center gap-2 cursor-pointer">
            <input
              type="checkbox"
              className="w-4 h-4 rounded border-white/40 bg-white/20 text-indigo-500 focus:ring-indigo-500 focus:ring-offset-0"
            />
            <span className="text-white/80">Ghi nhớ đăng nhập</span>
          </label>
          <Link to="/forgot-password" className="text-white/90 hover:text-white font-medium transition-colors">
            Quên mật khẩu?
          </Link>
        </div>

        {/* Submit Button */}
        <button
          type="submit"
          disabled={isLoading}
          className="w-full py-3.5 bg-gradient-to-r from-indigo-500 to-purple-600 hover:from-indigo-600 hover:to-purple-700 text-white font-semibold rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:scale-[1.02] disabled:opacity-70 disabled:cursor-not-allowed disabled:transform-none flex items-center justify-center gap-2"
        >
          {isLoading ? (
            <>
              <Loader2 className="w-5 h-5 animate-spin" />
              Đang đăng nhập...
            </>
          ) : (
            <>
              Đăng nhập
              <ArrowRight className="w-5 h-5" />
            </>
          )}
        </button>
      </form>

      {/* Divider */}
      <div className="relative my-6">
        <div className="absolute inset-0 flex items-center">
          <div className="w-full border-t border-white/30"></div>
        </div>
        <div className="relative flex justify-center text-sm">
          <span className="px-4 bg-transparent text-white/60">hoặc</span>
        </div>
      </div>

      {/* Register Link */}
      <p className="text-center text-white/80">
        Chưa có tài khoản?{' '}
        <Link to="/register" className="text-white font-semibold hover:underline">
          Đăng ký ngay
        </Link>
      </p>
    </div>
  );
};

export default LoginPage;
