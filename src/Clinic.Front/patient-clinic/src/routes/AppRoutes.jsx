// src/routes/AppRoutes.jsx
import { Routes, Route } from 'react-router-dom';
import PrivateRoute from '../hoc/PrivateRoute';
import MainLayout from '../components/layout/MainLayout';
import Login from '../pages/auth/Login';
import SignUp from '../pages/auth/SignUp';
import MyAppointmentList from '../pages/my-appointments/MyAppointmentList';
import NewAppointment from '../pages/new-appointment/NewAppointment';
import MyProfile from '../pages/profile/MyProfile';
import MyAppointmentDetails from '../pages/my-appointments/MyAppointmentDetails';
import Support from '../pages/support/Support'; // Импортируем новую страницу

const AppRoutes = () => (
  <Routes>
    <Route path="/login" element={<Login />} />
    <Route path="/signup" element={<SignUp />} />
    
    <Route element={<MainLayout />}> {/* <--- Общая обертка */}
      <Route path="/" element={<NewAppointment />} /> {/* Делаем его главной */}
      <Route path="/new-appointment" element={<NewAppointment />} />
      <Route path="/support" element={<Support />} /> {/* <-- НОВЫЙ РОУТ */}
      
      <Route element={<PrivateRoute />}>
        <Route path="/my-appointments" element={<MyAppointmentList />} />
        <Route path="/profile" element={<MyProfile />} />
        <Route path="/my-appointments/:id" element={<MyAppointmentDetails />} />
      </Route>
    </Route>

    <Route path="*" element={<h1>404</h1>} />
  </Routes>
);

export default AppRoutes;