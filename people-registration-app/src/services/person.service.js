import api from './api';

class PersonService {
  constructor() {
    this.baseUrlV1 = '/api/v1/Peoples';
    this.baseUrlV2 = '/api/v2/Peoples';
  }

  checkTokenExists() {
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('Token de autenticação não encontrado');
    }
    return token;
  }

  hasAddress(personData) {
    return personData && personData.address && personData.address.trim() !== '';
  }
  
  async getAll(pageNumber = 1, pageSize = 10) {
    try {
      this.checkTokenExists();
      
      const response = await api.get(`${this.baseUrlV1}?pageNumber=${pageNumber}&pageSize=${pageSize}`);
      return response.data;
    } catch (error) {
      if (error.response?.data) {
        throw error.response.data;
      }
      throw error;
    }
  }

  async getById(id) {
    try {
      this.checkTokenExists();
      const response = await api.get(`${this.baseUrlV1}/${id}`);
      
      if (response.data && response.data.success) {
        return response.data.data;
      }
      
      throw new Error(response.data.message || 'Pessoa não encontrada');
    } catch (error) {
      console.error(`Error getting person with ID ${id}:`, error);
      if (error.response?.data?.message) {
        throw new Error(error.response.data.message);
      }
      throw error;
    }
  }

  async create(personData) {
    try {
      const useV2 = this.hasAddress(personData);
      const url = useV2 ? this.baseUrlV2 : this.baseUrlV1;
      
      const response = await api.post(url, personData);
      
      return {
        success: response.data.success,
        data: response.data.data,
        message: response.data.message,
        errors: response.data.errors
      };
    } catch (error) {
      if (error.response?.data) {
        throw error.response.data;
      }
      throw error;
    }
  }

  async update(id, personData) {
    try {
      const useV2 = this.hasAddress(personData);
      const url = useV2 ? this.baseUrlV2 : this.baseUrlV1;
      console.log(`Usando API ${useV2 ? 'v2' : 'v1'} para atualizar pessoa`);
      
      const response = await api.put(`${url}/${id}`, personData);
      
      return {
        success: response.data.success,
        data: response.data.data,
        message: response.data.message,
        errors: response.data.errors
      };
    } catch (error) {
      console.error(`Error updating person with ID ${id}:`, error);
      if (error.response?.data) {
        throw error.response.data;
      }
      throw error;
    }
  }

  async delete(id) {
    try {
      const response = await api.delete(`${this.baseUrlV1}/${id}`);
      
      return {
        success: response.data.success,
        message: response.data.message,
        errors: response.data.errors
      };
    } catch (error) {
      console.error(`Error deleting person with ID ${id}:`, error);
      if (error.response?.data) {
        throw error.response.data;
      }
      throw error;
    }
  }
}

export default new PersonService();
