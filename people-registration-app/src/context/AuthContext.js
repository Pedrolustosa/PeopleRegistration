import React, { createContext, useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthService from '../services/auth.service';

const AuthContext = createContext(null);

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
  const [currentUser, setCurrentUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      setCurrentUser({ token });
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    try {
      const response = await AuthService.login(email, password);
      if (response && response.token) {
        setCurrentUser({ token: response.token });
        await new Promise(resolve => setTimeout(resolve, 100));
        return {
          success: true,
          message: response.message || 'Login realizado com sucesso!'
        };
      }
      return { success: false };
    } catch (error) {
      return {
        success: false,
        message: error.message || 'Falha na autenticação',
        errors: error.errors || []
      };
    }
  };

  const register = async (name, username, email, birthDate, cpf, password) => {
    try {
      const response = await AuthService.register(name, username, email, birthDate, cpf, password);
      return response;
    } catch (error) {
      return {
        success: false,
        message: error.message || 'Falha no registro',
        errors: error.errors || []
      };
    }
  };

  const logout = () => {
    AuthService.logout();
    setCurrentUser(null);
    navigate('/login');
  };

  const value = {
    currentUser,
    login,
    register,
    logout,
    isAuthenticated: !!currentUser && !!localStorage.getItem('token')
  };

  return (
    <AuthContext.Provider value={value}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

export default AuthContext;
