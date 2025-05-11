# Sistema de Registro de Pessoas

Este é um sistema completo de registro de pessoas desenvolvido com React no frontend e .NET 8 no backend. O sistema permite gerenciar informações pessoais como nome, CPF, endereço e outros dados relevantes.

## 🚀 Tecnologias Utilizadas

### Frontend
- React 18
- React Bootstrap
- React Router DOM
- Axios
- Font Awesome
- React Toastify

### Backend
- .NET 8
- Entity Framework Core
- InMemory/SQLite
- JWT Authentication
- Swagger/OpenAPI

## 📋 Pré-requisitos

- Node.js 16+ 
- .NET SDK 8.0
- InMemory
- Git

## 🔧 Configuração e Instalação

### Backend (.NET 8)

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/PeopleRegistration.git
cd PeopleRegistration
```

2. Navegue até a pasta da API:
```bash
cd PeopleRegistrationAPI
```

3. Restaure os pacotes e execute as migrações:
```bash
dotnet restore
dotnet ef database update
```

4. Execute a API:
```bash
dotnet run
```

### Frontend (React)

1. Navegue até a pasta do frontend:
```bash
cd people-registration-app
```

2. Instale as dependências:
```bash
npm install
```

3. Execute o projeto:
```bash
npm start
```

O frontend estará disponível em `http://localhost:3000`

## 🌟 Funcionalidades

- ✅ Autenticação de usuários
- 📝 CRUD completo de pessoas
- 📱 Interface responsiva
- 🔒 Proteção de rotas
- ⚡ Validações em tempo real

## 📦 Estrutura do Projeto

## 🚀 Deploy

O frontend está deployado na Vercel e pode ser acessado em: https://people-registration-app.vercel.app/login

## ✨ Autor

Pedro Lustosa