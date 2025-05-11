import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Nav, Button } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { 
  faUserFriends, 
  faSignOutAlt,
  faBars,
  faTimes
} from '@fortawesome/free-solid-svg-icons';
import { useAuth } from '../../context/AuthContext';

const Sidebar = ({ expanded, toggleSidebar }) => {
  const location = useLocation();
  const { logout } = useAuth();
  
  const isActive = (path) => {
    return location.pathname === path || 
           (path !== '/' && location.pathname.startsWith(path));
  };

  const handleLogout = () => {
    logout();
  };

  return (
    <div className={`sidebar bg-dark text-white ${expanded ? 'expanded' : 'collapsed'}`}>
      <div className="sidebar-header d-flex justify-content-between align-items-center p-3">
        <h5 className={`m-0 ${!expanded && 'd-none'}`}>Menu</h5>
        <Button 
          variant="link" 
          className="text-white p-0" 
          onClick={toggleSidebar}
        >
          <FontAwesomeIcon icon={expanded ? faTimes : faBars} />
        </Button>
      </div>
      
      <Nav className="flex-column mt-3">
        <Nav.Item>
          <Nav.Link 
            as={Link} 
            to="/people" 
            className={`text-white d-flex align-items-center ${isActive('/people') ? 'active-link' : ''}`}
          >
            <FontAwesomeIcon icon={faUserFriends} className="me-3" />
            {expanded && <span>Pessoas</span>}
          </Nav.Link>
        </Nav.Item>
        
        <div className="mt-auto">
          <Nav.Item className="mt-5">
            <Nav.Link 
              onClick={handleLogout} 
              className="text-white d-flex align-items-center"
            >
              <FontAwesomeIcon icon={faSignOutAlt} className="me-3" />
              {expanded && <span>Sair</span>}
            </Nav.Link>
          </Nav.Item>
        </div>
      </Nav>
    </div>
  );
};

export default Sidebar;
