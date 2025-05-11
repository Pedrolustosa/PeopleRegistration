import React, { useState, useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import Sidebar from './Sidebar';
import './layout.css';

const MainLayout = () => {
  // Estado para controlar se a barra lateral está expandida ou não
  const [sidebarExpanded, setSidebarExpanded] = useState(true);
  
  // Detectar tamanho da tela para decidir se a barra lateral deve estar expandida
  useEffect(() => {
    const handleResize = () => {
      setSidebarExpanded(window.innerWidth >= 992);
    };
    
    // Configuração inicial
    handleResize();
    
    // Adicionar listener para redimensionamento da janela
    window.addEventListener('resize', handleResize);
    
    // Cleanup
    return () => window.removeEventListener('resize', handleResize);
  }, []);
  
  // Função para alternar a exibição da barra lateral
  const toggleSidebar = () => {
    setSidebarExpanded(!sidebarExpanded);
  };
  
  return (
    <div className="main-layout">
      <Sidebar expanded={sidebarExpanded} toggleSidebar={toggleSidebar} />
      <div className={`content ${sidebarExpanded ? 'content-shifted' : ''}`}>
        <Container fluid className="p-4">
          <Outlet />
        </Container>
      </div>
    </div>
  );
};

export default MainLayout;
