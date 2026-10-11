// src/routes/AppRoutes.jsx
import { Routes, Route } from 'react-router-dom';
import { Box, Typography } from '@mui/material';

// Core Layout & Public Pages
import PrivateRoute from '../hoc/PrivateRoute';
import MainLayout from '../components/layout/MainLayout';
import Login from '../pages/auth/Login';
import Dashboard from '../pages/dashboard/Dashboard';

// Admin Pages
import AdminList from '../pages/admins/AdminList';
import AdminForm from '../pages/admins/AdminForm';

// Doctor Pages
import DoctorList from '../pages/doctors/DoctorList';
import DoctorForm from '../pages/doctors/DoctorForm';

// Office Pages
import OfficeList from '../pages/offices/OfficeList';
import OfficeForm from '../pages/offices/OfficeForm';

// Specialization Pages
import SpecializationList from '../pages/specializations/SpecializationList';
import SpecializationForm from '../pages/specializations/SpecializationForm';

// TODO: Import patient pages when they are created
import PatientList from '../pages/patients/PatientList';
import PatientForm from '../pages/patients/PatientForm';

import ServiceList from '../pages/services/ServiceList';
import ServiceForm from '../pages/services/ServiceForm';

import Profile from '../pages/profile/Profile'; 

import AppointmentList from '../pages/appointments/AppointmentList';
import AppointmentForm from '../pages/appointments/AppointmentForm';
import AppointmentResultForm from '../pages/appointments/AppointmentResultForm';
import AppointmentDetails from '../pages/appointments/AppointmentDetails';

const AppRoutes = () => (
  <Routes>
    {/* Public Route */}
    <Route path="/login" element={<Login />} />

    {/* Private Routes */}
    <Route element={<PrivateRoute />}>
      <Route element={<MainLayout />}>
        <Route path="/" element={<Dashboard />} />
        
        {/* Administrators (Receptionists) */}
        <Route path="/admins" element={<AdminList />} />
        <Route path="/admins/edit/:id" element={<AdminForm />} />
        
        {/* Doctors */}
        <Route path="/doctors" element={<DoctorList />} />
        <Route path="/doctors/new" element={<DoctorForm />} />
        <Route path="/doctors/edit/:id" element={<DoctorForm />} />

        {/* Offices */}
        <Route path="/offices" element={<OfficeList />} />
        <Route path="/offices/new" element={<OfficeForm />} />
        <Route path="/offices/edit/:id" element={<OfficeForm />} />
        
        {/* Specializations */}
        <Route path="/specializations" element={<SpecializationList />} />
        <Route path="/specializations/new" element={<SpecializationForm />} />
        <Route path="/specializations/edit/:id" element={<SpecializationForm />} />

        {/* TODO: Patient routes */}
        <Route path="/patients" element={<PatientList />} />
        <Route path="/patients/edit/:id" element={<PatientForm />} />
        
        <Route path="/services" element={<ServiceList />} />
        <Route path="/services/new" element={<ServiceForm />} />
        <Route path="/services/edit/:id" element={<ServiceForm />} />

        
        <Route path="/appointments" element={<AppointmentList />} />
        <Route path="/appointments/:id" element={<AppointmentDetails />} />
        <Route path="/appointments/new/:patientId" element={<AppointmentForm />} />
        <Route path="/appointments/:appointmentId/result" element={<AppointmentResultForm />} />
        <Route path="/profile" element={<Profile />} /> 
      </Route>
    </Route>

    {/* Not Found Route */}
    <Route path="*" element={
      <Box sx={{ p: 4, textAlign: 'center', height: '100vh', display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center' }}>
          <Typography variant="h1" component="div" fontWeight="bold">404</Typography>
          <Typography variant="h5">Page Not Found</Typography>
          <Typography variant="body1" sx={{ mt: 2 }}>The page you are looking for does not exist.</Typography>
      </Box>
    } />
  </Routes>
);

export default AppRoutes;