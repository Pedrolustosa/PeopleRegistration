import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import Login from '../components/auth/Login';
import Register from '../components/auth/Register';
import PeopleList from '../components/people/PeopleList';
import PersonForm from '../components/people/PersonForm';
import MainLayout from '../components/layout/MainLayout';
import { AuthProvider, useAuth } from '../context/AuthContext';

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
        const from = location.state?.from?.pathname || '/';
        return <Navigate to="/login" state={{ from }} replace />;
  }

  return children;
};

const PublicRoute = ({ children }) => {
  const { isAuthenticated } = useAuth();
  
  if (isAuthenticated) {
        return <Navigate to="/" replace />;
  }

  return children;
};

const AppRoutesContent = () => {
  return (
    <Routes>
      <Route path="/login" element={
        <PublicRoute>
          <Login />
        </PublicRoute>
      } />
      <Route path="/register" element={
        <PublicRoute>
          <Register />
        </PublicRoute>
      } />
      <Route element={
        <ProtectedRoute>
          <MainLayout />
        </ProtectedRoute>
      }>
        <Route path="/" element={<Navigate to="/people" replace />} />
        <Route path="/people" element={<PeopleList />} />
        <Route path="/people/create" element={<PersonForm />} />
        <Route path="/people/edit/:id" element={<PersonForm />} />
      </Route>
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
};

const AppRoutes = () => {
  return (
    <Router>
      <AuthProvider>
        <AppRoutesContent />
      </AuthProvider>
    </Router>
  );
};

export default AppRoutes;
