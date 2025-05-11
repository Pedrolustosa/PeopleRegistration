import React, { useState } from 'react';
import { Form, Button, Container, Card, Row, Col, Spinner, Alert, InputGroup } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { 
  faUserPlus,
  faEnvelope,
  faLock,
  faEye,
  faEyeSlash,
  faUser,
  faCalendar,
  faIdCard
} from '@fortawesome/free-solid-svg-icons';
import { useAuth } from '../../context/AuthContext';

const Register = () => {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: '',
    username: '',
    email: '',
    birthDate: '',
    cpf: '',
    password: '',
    confirmPassword: ''
  });
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const [validated, setValidated] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [serverError, setServerError] = useState('');

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleCpfChange = (e) => {
    let value = e.target.value.replace(/\D/g, '');
    
    if (value.length <= 11) {
      // Formata o CPF (XXX.XXX.XXX-XX)
      value = value
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
        
      setFormData(prev => ({
        ...prev,
        cpf: value
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    
    if (!formData.name?.trim()) {
      newErrors.name = 'Nome é obrigatório';
    }

    if (!formData.username?.trim()) {
      newErrors.username = 'Nome de usuário é obrigatório';
    }

    if (!formData.email?.trim()) {
      newErrors.email = 'Email é obrigatório';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Email inválido';
    }

    if (!formData.birthDate) {
      newErrors.birthDate = 'Data de nascimento é obrigatória';
    }

    // Validação do CPF (formato básico)
    const cpfClean = formData.cpf.replace(/\D/g, '');
    if (!cpfClean || cpfClean.length !== 11) newErrors.cpf = 'CPF inválido';

    if (!formData.password?.trim()) {
      newErrors.password = 'Senha é obrigatória';
    } else if (formData.password.length < 6) {
      newErrors.password = 'A senha deve ter pelo menos 6 caracteres';
    }

    if (!formData.confirmPassword?.trim()) {
      newErrors.confirmPassword = 'Confirmação de senha é obrigatória';
    } else if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'As senhas não coincidem';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const form = e.currentTarget;

    if (!form.checkValidity() || !validateForm()) {
      e.stopPropagation();
      setValidated(true);
      return;
    }

    setLoading(true);
    setErrors({});

    try {
      const birthDateFormatted = new Date(formData.birthDate).toISOString();
      const response = await register(
        formData.name,
        formData.username,
        formData.email,
        birthDateFormatted,
        formData.cpf.replace(/\D/g, ''),
        formData.password
      );
      
      if (response.success) {
        toast.success(response.message || 'Registro realizado com sucesso! Você pode fazer login agora.');
        setTimeout(() => {
          navigate('/login');
        }, 2000);
      } else {
        if (response.errors && response.errors.length > 0) {
          // Tentar mapear erros para campos específicos
          const fieldErrors = {};
          let hasFieldErrors = false;
          
          response.errors.forEach(error => {
            // Exemplo de formato de erro: "O campo Email é obrigatório"
            const fieldMatch = error.match(/O campo ([\w]+) é/);
            if (fieldMatch && fieldMatch[1]) {
              const fieldName = fieldMatch[1].charAt(0).toLowerCase() + fieldMatch[1].slice(1);
              fieldErrors[fieldName] = error;
              hasFieldErrors = true;
            }
          });
          
          if (hasFieldErrors) {
            setErrors(fieldErrors);
          } else {
            setServerError(response.errors.join('\n'));
          }
        } else {
          setServerError(response.message || 'Ocorreu um erro ao tentar registrar. Por favor, tente novamente.');
        }
      }
    } catch (error) {
      console.error('Registration error:', error);
      if (error.errors && error.errors.length > 0) {
        setServerError(error.errors.join('\n'));
      } else {
        setServerError(error.message || 'Ocorreu um erro ao tentar registrar. Por favor, tente novamente.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container className="min-vh-100 d-flex align-items-center justify-content-center py-5">
      <Card className="shadow-lg border-0" style={{ maxWidth: '800px', width: '100%' }}>
        <Card.Body className="p-5">
          <div className="text-center mb-4">
            <div className="bg-success text-white rounded-circle d-inline-flex align-items-center justify-content-center mb-3" style={{ width: '64px', height: '64px' }}>
              <FontAwesomeIcon icon={faUserPlus} size="2x" />
            </div>
            <h2 className="fw-bold mb-0">Criar Conta</h2>
            <p className="text-muted">Preencha seus dados para se registrar</p>
          </div>
          <Card.Body>
            {serverError && <Alert variant="danger">{serverError}</Alert>}
            <Form noValidate validated={validated} onSubmit={handleSubmit}>
              <Row>
                <Col md={6}>
                  <Form.Group className="mb-3">
                    <Form.Label>Nome Completo</Form.Label>
                    <InputGroup>
                      <InputGroup.Text className="bg-light border-end-0">
                        <FontAwesomeIcon icon={faUser} className="text-muted" />
                      </InputGroup.Text>
                      <Form.Control
                        type="text"
                        name="name"
                        value={formData.name}
                        onChange={handleChange}
                        isInvalid={!!errors.name}
                        placeholder="Digite seu nome completo"
                        className="border-start-0"
                        required
                      />
                      <Form.Control.Feedback type="invalid">{errors.name || 'Nome completo é obrigatório'}</Form.Control.Feedback>
                    </InputGroup>
                  </Form.Group>
                </Col>
                <Col md={6}>
                  <Form.Group className="mb-3">
                    <Form.Label>Nome de Usuário</Form.Label>
                    <InputGroup>
                      <InputGroup.Text className="bg-light border-end-0">
                        <FontAwesomeIcon icon={faUser} className="text-muted" />
                      </InputGroup.Text>
                      <Form.Control
                        type="text"
                        name="username"
                        value={formData.username}
                        onChange={handleChange}
                        isInvalid={!!errors.username}
                        placeholder="Digite seu nome de usuário"
                        className="border-start-0"
                        required
                      />
                      <Form.Control.Feedback type="invalid">{errors.username || 'Nome de usuário é obrigatório'}</Form.Control.Feedback>
                    </InputGroup>
                  </Form.Group>
                </Col>
              </Row>

              <Row>
                <Col md={6}>
                  <Form.Group className="mb-3" controlId="email">
                    <Form.Label>Email</Form.Label>
                    <InputGroup hasValidation>
                      <InputGroup.Text className="bg-light border-end-0">
                        <FontAwesomeIcon icon={faEnvelope} className="text-muted" />
                      </InputGroup.Text>
                      <Form.Control
                        type="email"
                        name="email"
                        value={formData.email}
                        onChange={handleChange}
                        placeholder="Digite seu email"
                        className="border-start-0"
                        required
                        pattern="[^\s@]+@[^\s@]+\.[^\s@]+"
                        isInvalid={!!errors.email}
                      />
                      <Form.Control.Feedback type="invalid">
                        {errors.email || 'Email é obrigatório e deve estar no formato correto'}
                      </Form.Control.Feedback>
                    </InputGroup>
                  </Form.Group>
                </Col>
                <Col md={6}>
                  <Form.Group className="mb-3">
                    <Form.Label>Senha</Form.Label>
                    <InputGroup hasValidation>
                      <InputGroup.Text className="bg-light border-end-0">
                        <FontAwesomeIcon icon={faLock} className="text-muted" />
                      </InputGroup.Text>
                      <Form.Control
                        type={showPassword ? 'text' : 'password'}
                        name="password"
                        value={formData.password}
                        onChange={handleChange}
                        placeholder="Digite sua senha"
                        className="border-start-0 border-end-0"
                        required
                        minLength={6}
                        isInvalid={!!errors.password}
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
                        {errors.password || 'Senha é obrigatória e deve ter pelo menos 6 caracteres'}
                      </Form.Control.Feedback>
                    </InputGroup>
                  </Form.Group>
                </Col>
              </Row>

              <Row>
                <Col md={6}>
                  <Form.Group className="mb-3" controlId="confirmPassword">
                    <Form.Label>Confirmar Senha</Form.Label>
                    <InputGroup hasValidation>
                      <InputGroup.Text className="bg-light border-end-0">
                        <FontAwesomeIcon icon={faLock} className="text-muted" />
                      </InputGroup.Text>
                      <Form.Control
                        type={showConfirmPassword ? 'text' : 'password'}
                        name="confirmPassword"
                        value={formData.confirmPassword}
                        onChange={handleChange}
                        placeholder="Confirme sua senha"
                        className="border-start-0 border-end-0"
                        required
                        isInvalid={!!errors.confirmPassword}
                      />
                      <InputGroup.Text 
                        className="bg-light border-start-0 cursor-pointer"
                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                        style={{ cursor: 'pointer' }}
                      >
                        <FontAwesomeIcon 
                          icon={showConfirmPassword ? faEyeSlash : faEye} 
                          className="text-muted" 
                        />
                      </InputGroup.Text>
                      <Form.Control.Feedback type="invalid">
                        {errors.confirmPassword || 'Confirmação de senha é obrigatória'}
                      </Form.Control.Feedback>
                    </InputGroup>
                  </Form.Group>
                </Col>
                <Col md={6}>
                  <Form.Group className="mb-3">
                    <Form.Label>Data de Nascimento</Form.Label>
                    <InputGroup>
                      <InputGroup.Text className="bg-light border-end-0">
                        <FontAwesomeIcon icon={faCalendar} className="text-muted" />
                      </InputGroup.Text>
                      <Form.Control
                        type="date"
                        name="birthDate"
                        value={formData.birthDate}
                        onChange={handleChange}
                        isInvalid={!!errors.birthDate}
                        max={new Date().toISOString().split('T')[0]}
                        className="border-start-0"
                        required
                      />
                      <Form.Control.Feedback type="invalid">{errors.birthDate || 'Data de nascimento é obrigatória'}</Form.Control.Feedback>
                    </InputGroup>
                  </Form.Group>
                </Col>
              </Row>

              <Row>
                <Col md={6}>
                  <Form.Group className="mb-3">
                    <Form.Label>CPF</Form.Label>
                    <InputGroup>
                      <InputGroup.Text className="bg-light border-end-0">
                        <FontAwesomeIcon icon={faIdCard} className="text-muted" />
                      </InputGroup.Text>
                      <Form.Control
                        type="text"
                        name="cpf"
                        value={formData.cpf}
                        onChange={handleCpfChange}
                        isInvalid={!!errors.cpf}
                        placeholder="XXX.XXX.XXX-XX"
                        maxLength={14}
                        className="border-start-0"
                        required
                      />
                      <Form.Control.Feedback type="invalid">{errors.cpf || 'CPF é obrigatório'}</Form.Control.Feedback>
                    </InputGroup>
                  </Form.Group>
                </Col>
              </Row>

              <Button
                variant="success"
                type="submit"
                disabled={loading}
                className="w-100 mb-4 py-2 fw-bold mt-4"
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
                    Registrando...
                  </>
                ) : (
                  'Registrar'
                )}
              </Button>

              <div className="text-center">
                <p className="text-muted mb-0">
                  Já tem uma conta?{' '}
                  <Link to="/login" className="text-success text-decoration-none fw-bold">
                    Entre aqui
                  </Link>
                </p>
              </div>
            </Form>
          </Card.Body>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default Register;
