import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext/AuthContext';
import { Loader2 } from 'lucide-react';

const ProtectedRoute = ({ children, adminOnly = false }) => {
  const { isAuthenticated, isLoading, isAdmin } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-50 via-white to-purple-50">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="w-10 h-10 animate-spin text-indigo-600" />
          <p className="text-gray-600 font-medium">Đang tải...</p>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (adminOnly && !isAdmin) {
    return <Navigate to="/403" replace />;
  }

  return children;
};

export default ProtectedRoute;
