import React, { useState, useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import Sidebar from './Sidebar';
import './layout.css';

const MainLayout = () => {
  const [sidebarExpanded, setSidebarExpanded] = useState(true);
  
  useEffect(() => {
    const handleResize = () => {
      setSidebarExpanded(window.innerWidth >= 992);
    };
    
    handleResize();
    
    window.addEventListener('resize', handleResize);
    
    return () => window.removeEventListener('resize', handleResize);
  }, []);
  
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
