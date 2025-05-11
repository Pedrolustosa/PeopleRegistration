import axios from 'axios';
import { toast } from 'react-toastify';

// Configuração base do axios
const api = axios.create({
  baseURL: 'https://localhost:7069', // URL da sua API
});

// Interceptor para incluir o token em todas as requisições
api.interceptors.request.use((config) => {
  // Ler o token do localStorage
  let token = localStorage.getItem('token');
  
  if (token) {
    try {
      // Verificar se o token está em formato JSON - caso esteja, extrair o valor de JWT
      if (token.startsWith('{')) {
        const tokenObj = JSON.parse(token);
        // O token da API vem no formato {jwt: "valor-do-token", expiresAt: "..."}
        if (tokenObj.jwt) {
          token = tokenObj.jwt;
          console.log('JWT extraído do objeto:', token);
        }
      }

      // Adicionar prefixo Bearer se não existir
      if (!token.startsWith('Bearer ')) {
        token = `Bearer ${token}`;
      }

      // Adicionar o token ao cabeçalho Authorization
      config.headers.Authorization = token;
      console.log('Token enviado no formato correto:', config.headers.Authorization);
    } catch (err) {
      console.error('Erro ao processar o token:', err);
    }
  }
  
  return config;
}, (error) => {
  return Promise.reject(error);
});

// Interceptor para tratar respostas e erros
api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    // Somente logar os erros, sem causar redirecionamentos ou deslogar
    // Isso permitirá ver os logs para debug
    console.log('Erro na API:', error);
    console.log('Status do erro:', error.response?.status);
    console.log('URL da requisição:', error.config?.url);
    console.log('Cabeçalhos enviados:', error.config?.headers);
    
    // Sem redirecionamento para login!
    return Promise.reject(error);
  }
);

export default api;
