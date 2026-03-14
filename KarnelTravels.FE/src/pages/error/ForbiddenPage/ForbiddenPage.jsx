import { Link } from 'react-router-dom';
import { Home, ArrowLeft, Lock } from 'lucide-react';

const ForbiddenPage = () => {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-50 via-white to-purple-50 p-4">
      <div className="text-center">
        <div className="inline-flex items-center justify-center w-24 h-24 rounded-full bg-red-100 mb-6">
          <Lock className="w-12 h-12 text-red-500" />
        </div>
        <h1 className="text-6xl font-bold text-gray-800 mb-4">403</h1>
        <h2 className="text-2xl font-semibold text-gray-700 mb-4">Truy cập bị từ chối</h2>
        <p className="text-gray-600 mb-8 max-w-md mx-auto">
          Xin lỗi, bạn không có quyền truy cập vào trang này. Vui lòng liên hệ quản trị viên nếu bạn cho rằng đây là lỗi.
        </p>
        <div className="flex gap-4 justify-center">
          <Link
            to="/"
            className="inline-flex items-center gap-2 px-6 py-3 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700 transition-colors"
          >
            <Home className="w-5 h-5" />
            Trang chủ
          </Link>
          <button
            onClick={() => window.history.back()}
            className="inline-flex items-center gap-2 px-6 py-3 bg-gray-100 text-gray-700 font-medium rounded-lg hover:bg-gray-200 transition-colors"
          >
            <ArrowLeft className="w-5 h-5" />
            Quay lại
          </button>
        </div>
      </div>
    </div>
  );
};

export default ForbiddenPage;
