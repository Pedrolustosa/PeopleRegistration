import React, { useState } from 'react';
import { Form, Button, Container, Card, Spinner, InputGroup } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignInAlt, faEnvelope, faLock, faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
import { useAuth } from '../../context/AuthContext';
import { useNavigate, useLocation } from 'react-router-dom';
import { Link } from 'react-router-dom';

const Login = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [validated, setValidated] = useState(false);
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  
  const from = location.state?.from?.pathname || '/';

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    const form = e.currentTarget;
    
    if (!form.checkValidity()) {
      e.stopPropagation();
      setValidated(true);
      return;
    }

    setLoading(true);

    try {
      const response = await login(formData.email, formData.password);
      if (response.success) {
        toast.success(response.message);
        setTimeout(() => navigate(from), 1000);
      } else {
        if (response.errors?.length > 0) {
          response.errors.forEach(error => toast.error(error));
        } else {
          toast.error(response.message || 'Falha na autenticação');
        }
      }
    } catch (error) {
      toast.error('Erro ao tentar fazer login. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container className="min-vh-100 d-flex align-items-center justify-content-center py-5">
      <Card className="shadow-lg border-0" style={{ maxWidth: '450px', width: '100%' }}>
        <Card.Body className="p-5">
          <div className="text-center mb-4">
            <div className="bg-primary text-white rounded-circle d-inline-flex align-items-center justify-content-center mb-3" style={{ width: '64px', height: '64px' }}>
              <FontAwesomeIcon icon={faSignInAlt} size="2x" />
            </div>
            <h2 className="fw-bold mb-0">Bem-vindo</h2>
            <p className="text-muted">Faça login para continuar</p>
          </div>

          <Form noValidate validated={validated} onSubmit={handleLogin}>
            <Form.Group className="mb-4" controlId="email">
              <Form.Label>Email</Form.Label>
              <InputGroup>
                <InputGroup.Text className="bg-light border-end-0">
                  <FontAwesomeIcon icon={faEnvelope} className="text-muted" />
                </InputGroup.Text>
                <Form.Control
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  required
                  placeholder="Digite seu email"
                  className="border-start-0"
                  pattern="^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$"
                />
                <Form.Control.Feedback type="invalid">
                  Por favor, insira um email válido.
                </Form.Control.Feedback>
              </InputGroup>
            </Form.Group>

            <Form.Group className="mb-4" controlId="password">
              <Form.Label>Senha</Form.Label>
              <InputGroup>
                <InputGroup.Text className="bg-light border-end-0">
                  <FontAwesomeIcon icon={faLock} className="text-muted" />
                </InputGroup.Text>
                <Form.Control
                  type={showPassword ? 'text' : 'password'}
                  name="password"
                  value={formData.password}
                  onChange={handleInputChange}
                  required
                  placeholder="Digite sua senha"
                  className="border-start-0 border-end-0"
                  minLength="6"
                />
                <InputGroup.Text 
                  className="bg-light border-start-0 cursor-pointer"
                  onClick={() => setShowPassword(!showPassword)}
                  style={{ cursor: 'pointer' }}
                >
                  <FontAwesomeIcon 
                    icon={showPassword ? faEyeSlash : faEye} 
                    className="text-muted" 
                  />
                </InputGroup.Text>
                <Form.Control.Feedback type="invalid">
                  A senha deve ter pelo menos 6 caracteres.
                </Form.Control.Feedback>
              </InputGroup>
            </Form.Group>

            <Button
              variant="primary"
              type="submit"
              disabled={loading}
              className="w-100 mb-4 py-2 fw-bold"
            >
              {loading ? (
                <>
                  <Spinner
                    as="span"
                    animation="border"
                    size="sm"
                    role="status"
                    aria-hidden="true"
                    className="me-2"
                  />
                  Entrando...
                </>
              ) : (
                'Entrar'
              )}
            </Button>

            <div className="text-center">
              <p className="text-muted mb-0">
                Não tem uma conta?{' '}
                <Link to="/register" className="text-primary text-decoration-none fw-bold">
                  Registre-se aqui
                </Link>
              </p>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default Login;
