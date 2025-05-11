import axios from 'axios';

const api = axios.create({
  baseURL: 'https://peopleregistrationweb.azurewebsites.net',
});

api.interceptors.request.use((config) => {
  let token = localStorage.getItem('token');
  
  if (token) {
    try {
      if (token.startsWith('{')) {
        const tokenObj = JSON.parse(token);
        if (tokenObj.jwt) {
          token = tokenObj.jwt;
        }
      }

      if (!token.startsWith('Bearer ')) {
        token = `Bearer ${token}`;
      }
      config.headers.Authorization = token;
    } catch (err) {
    }
  }
  
  return config;
}, (error) => {
  return Promise.reject(error);
});

api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;
