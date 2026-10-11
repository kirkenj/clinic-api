// src/routes/AppRoutes.jsx
import { Routes, Route } from 'react-router-dom';

import MainLayout from '../components/layout/MainLayout';
import PrivateRoute from '../hoc/PrivateRoute';

import Login from '../pages/auth/Login';
import Dashboard from '../pages/dashboard/Dashboard';
import MySchedule from '../pages/my-schedule/MySchedule';
import AppointmentDetails from '../pages/appointments/AppointmentDetails';
import AppointmentList from '../pages/appointments/AppointmentList';
import MyProfile from '../pages/profile/Profile';
import PatientList  from '../pages/patients/PatientList';
import DoctorList from '../pages/doctors/DoctorList';
import AppointmentForm from '../components/appointments/AppointmentForm';
import Support from '../pages/support/Support'; 
import { Box, Typography } from '@mui/material';

const AppRoutes = () => (
  <Routes>
    <Route path="/login" element={<Login />} />

    <Route element={<PrivateRoute />}>
      <Route element={<MainLayout />}>
        <Route path="/" element={<Dashboard />} />
        <Route path="/my-schedule" element={<MySchedule />} />
        <Route path="/profile" element={<MyProfile />} />
        <Route path="/appointments/:id" element={<AppointmentDetails/>}/>
        <Route path="/patients/" element={<PatientList/>}/>
        <Route path="/doctors/" element={<DoctorList/>}/>
        <Route path="/appointments/" element={<AppointmentList/>}/>
        <Route path="/appointments/new/:patientId" element={<AppointmentForm />} />
        <Route path="/support" element={<Support />} />
      </Route>
    </Route>
    
    <Route path="*" element={
      <Box sx={{ p: 4, textAlign: 'center' }}>
        <Typography variant="h3">404</Typography>
        <Typography variant="h5">Page Not Found</Typography>
      </Box>
    } />
  </Routes>
);

export default AppRoutes;