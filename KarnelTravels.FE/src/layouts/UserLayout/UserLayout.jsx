import { Outlet } from 'react-router-dom';
import Navbar from '../../components/common/Navbar';

const UserLayout = () => {
  return (
    <div className="min-h-screen flex flex-col">
      <Navbar />
      <main className="flex-1 pt-16 lg:pt-20">
        <Outlet />
      </main>
    </div>
  );
};

export default UserLayout;
