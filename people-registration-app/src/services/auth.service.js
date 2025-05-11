import api from './api';

class AuthService {
  async login(email, password) {
    try {
      const response = await api.post('/api/Auth/login', {
        email,
        password
      });
      
      if (response.data && response.data.success && response.data.data) {
        const tokenData = response.data.data;
        const tokenString = typeof tokenData === 'string' ? tokenData : JSON.stringify(tokenData);
        
        localStorage.setItem('token', tokenString);
        return {
          token: tokenString,
          message: response.data.message
        };
      }
      
      return null;
    } catch (error) {
      console.error('Login error:', error);
      if (error.response && error.response.data) {
        throw {
          message: error.response.data.message || 'Falha na autenticação',
          errors: error.response.data.errors || []
        };
      }
      throw error;
    }
  }

  async register(name, username, email, birthDate, cpf, password) {
    try {
      const response = await api.post('/api/Auth/register', {
        name,
        username,
        email,
        birthDate,
        cpf,
        password
      });
      
      if (response.data && response.data.success) {
        return {
          success: true,
          message: response.data.message
        };
      }
      
      return { success: false };
    } catch (error) {
      console.error('Register error:', error);
      if (error.response && error.response.data) {
        throw {
          message: error.response.data.message || 'Falha no registro',
          errors: error.response.data.errors || []
        };
      }
      throw error;
    }
  }

  logout() {
    localStorage.removeItem('token');
  }

  getCurrentUser() {
    return localStorage.getItem('token');
  }
}

export default new AuthService();
