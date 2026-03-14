import { BrowserRouter, Routes, Route } from 'react-router-dom';
import UserLayout from './layouts/UserLayout/UserLayout';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<UserLayout />}>
          <Route index element={<div className="min-h-screen flex items-center justify-center">
            <h1 className="text-4xl font-bold text-gray-800">Welcome to KarnelTravels</h1>
          </div>} />
          <Route path="about" element={<div className="min-h-screen flex items-center justify-center">
            <h1 className="text-4xl font-bold text-gray-800">Giới thiệu</h1>
          </div>} />
          <Route path="search" element={<div className="min-h-screen flex items-center justify-center">
            <h1 className="text-4xl font-bold text-gray-800">Tìm kiếm</h1>
          </div>} />
          <Route path="info/*" element={<div className="min-h-screen flex items-center justify-center">
            <h1 className="text-4xl font-bold text-gray-800">Thông tin</h1>
          </div>} />
          <Route path="contact" element={<div className="min-h-screen flex items-center justify-center">
            <h1 className="text-4xl font-bold text-gray-800">Liên hệ</h1>
          </div>} />
          <Route path="login" element={<div className="min-h-screen flex items-center justify-center">
            <h1 className="text-4xl font-bold text-gray-800">Đăng nhập</h1>
          </div>} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
