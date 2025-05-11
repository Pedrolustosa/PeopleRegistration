import React, { useState, useEffect } from 'react';
import { Form, Button, Container, Card, Row, Col, Spinner, InputGroup } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { 
  faUser, 
  faSave, 
  faArrowLeft, 
  faEnvelope, 
  faCalendar, 
  faMapMarker,
  faPassport,
  faVenusMars,
  faIdCard,
  faHome,
  faInfoCircle,
  faPencilAlt,
  faUserPlus
} from '@fortawesome/free-solid-svg-icons';
import { useNavigate, useParams, Link } from 'react-router-dom';
import PersonService from '../../services/person.service';
import { GENDER_OPTIONS } from '../../constants';

const PersonForm = () => {
  const { id } = useParams();
  const isEditMode = window.location.pathname.includes('/edit/');
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    name: '',
    gender: '',
    email: '',
    birthDate: '',
    birthPlace: '',
    nationality: '',
    cpf: '',
    address: ''
  });
  
  const [loading, setLoading] = useState(false);
  const [fetchLoading, setFetchLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const [validated, setValidated] = useState(false);
  const [serverError, setServerError] = useState('');

  useEffect(() => {
    const fetchPerson = async () => {
      if (!id || !isEditMode) return;
      
      setFetchLoading(true);
      try {
        const person = await PersonService.getById(id);
        if (person) {
          const birthDate = person.birthDate ? new Date(person.birthDate).toISOString().split('T')[0] : '';
          
          const formattedCpf = person.cpf ? person.cpf.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})$/, '$1.$2.$3-$4') : '';
          setFormData({
            name: person.name || '',
            gender: person.gender !== null ? person.gender.toString() : '',
            email: person.email || '',
            birthDate,
            birthPlace: person.birthPlace || '',
            nationality: person.nationality || '',
            cpf: formattedCpf,
            address: person.address || ''
          });
        } else {
          throw new Error('Pessoa não encontrada');
        }
      } catch (error) {
        toast.error(error.message || 'Erro ao carregar os dados da pessoa');
        setServerError('Não foi possível carregar os dados. Por favor, tente novamente.');
        navigate('/people');
      } finally {
        setFetchLoading(false);
      }
    };

    fetchPerson();
  }, [id, isEditMode, navigate]);

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

    if (!formData.cpf?.trim()) {
      newErrors.cpf = 'CPF é obrigatório';
    } else if (!/^\d{3}\.\d{3}\.\d{3}-\d{2}$/.test(formData.cpf)) {
      newErrors.cpf = 'CPF inválido';
    }

    if (!formData.birthDate) {
      newErrors.birthDate = 'Data de nascimento é obrigatória';
    }

    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Email inválido';
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

    try {
      const personData = {
        name: formData.name,
        gender: formData.gender === '' ? null : Number(formData.gender),
        email: formData.email || null,
        birthDate: formData.birthDate,
        birthPlace: formData.birthPlace || null,
        nationality: formData.nationality || null,
        cpf: formData.cpf.replace(/\D/g, ''),
        address: formData.address || null
      };

      if (isEditMode) {
        await PersonService.update(id, personData);
        toast.success('Pessoa atualizada com sucesso!');
      } else {
        await PersonService.create(personData);
        toast.success('Pessoa cadastrada com sucesso!');
      }
      navigate('/people');
    } catch (error) {
      if (error.errors?.length > 0) {
        error.errors.forEach(err => toast.error(err));
      } else {
        toast.error(error.message || 'Erro ao salvar pessoa');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleBack = () => {
    navigate('/people');
  };

  if (fetchLoading) {
    return (
      <Container className="mt-5 text-center">
        <Spinner animation="border" variant="primary" />
        <p className="mt-2">Carregando dados...</p>
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <Card className="shadow-sm border-0">
        <Card.Header className="bg-white border-0 pb-0">
          <div className="d-flex justify-content-between align-items-center">
            <div>
              <h4 className="mb-2">
                <FontAwesomeIcon 
                  icon={isEditMode ? faPencilAlt : faUserPlus} 
                  className="text-success me-2" 
                />
                {isEditMode ? 'Editar Pessoa' : 'Nova Pessoa'}
              </h4>
              <p className="text-muted mb-0">Preencha os dados da pessoa</p>
            </div>
            <Button
              variant="light"
              size="sm"
              onClick={handleBack}
              className="px-3"
            >
              <FontAwesomeIcon icon={faArrowLeft} className="me-2" />
              Voltar
            </Button>
          </div>
        </Card.Header>
        <Card.Body className="pt-4">
          <Form noValidate validated={validated} onSubmit={handleSubmit}>
            <Row className="g-4">
              <Col md={6}>
                <Form.Group controlId="name">
                  <Form.Label>Nome<span className="text-danger">*</span></Form.Label>
                  <InputGroup hasValidation>
                    <InputGroup.Text className="bg-light border-end-0">
                      <FontAwesomeIcon icon={faUser} className="text-muted" />
                    </InputGroup.Text>
                    <Form.Control
                      type="text"
                      name="name"
                      value={formData.name}
                      onChange={handleChange}
                      placeholder="Digite o nome completo"
                      className="border-start-0"
                      required
                      isInvalid={!!errors.name}
                    />
                    <Form.Control.Feedback type="invalid">
                      {errors.name || 'Nome é obrigatório'}
                    </Form.Control.Feedback>
                  </InputGroup>
                </Form.Group>
              </Col>

              <Col md={6}>
                <Form.Group controlId="cpf">
                  <Form.Label>CPF<span className="text-danger">*</span></Form.Label>
                  <InputGroup hasValidation>
                    <InputGroup.Text className="bg-light border-end-0">
                      <FontAwesomeIcon icon={faIdCard} className="text-muted" />
                    </InputGroup.Text>
                    <Form.Control
                      type="text"
                      name="cpf"
                      value={formData.cpf}
                      onChange={handleCpfChange}
                      placeholder="000.000.000-00"
                      className={`border-start-0 ${isEditMode ? 'bg-light' : ''}`}
                      maxLength={14}
                      required
                      pattern="\d{3}\.\d{3}\.\d{3}-\d{2}"
                      isInvalid={!!errors.cpf}
                      disabled={isEditMode}
                    />
                    <Form.Control.Feedback type="invalid">
                      {errors.cpf || 'CPF é obrigatório e deve estar no formato correto'}
                    </Form.Control.Feedback>
                  </InputGroup>
                </Form.Group>
              </Col>
            </Row>

              <Col md={6}>
                <Form.Group controlId="email">
                  <Form.Label>
                    E-mail
                    {isEditMode && <span className="text-muted ms-2">(não editável)</span>}
                  </Form.Label>
                  <InputGroup>
                    <InputGroup.Text className="bg-light border-end-0">
                      <FontAwesomeIcon icon={faEnvelope} className="text-muted" />
                    </InputGroup.Text>
                    <Form.Control
                      type="email"
                      name="email"
                      value={formData.email}
                      onChange={handleChange}
                      placeholder="exemplo@email.com"
                      className={`border-start-0 ${isEditMode ? 'bg-light' : ''}`}
                      isInvalid={!!errors.email}
                      disabled={isEditMode}
                    />
                    <Form.Control.Feedback type="invalid">
                      {errors.email}
                    </Form.Control.Feedback>
                  </InputGroup>
                </Form.Group>
              </Col>

              <Col md={6}>
                <Form.Group controlId="gender">
                  <Form.Label>Gênero</Form.Label>
                  <InputGroup>
                    <InputGroup.Text className="bg-light border-end-0">
                      <FontAwesomeIcon icon={faVenusMars} className="text-muted" />
                    </InputGroup.Text>
                    <Form.Select
                      name="gender"
                      value={formData.gender}
                      onChange={handleChange}
                      className="border-start-0"
                    >
                      <option value="">Selecione...</option>
                      {GENDER_OPTIONS.map(option => (
                        <option key={option.value} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </Form.Select>
                  </InputGroup>
                </Form.Group>
              </Col>

              <Col md={6}>
                <Form.Group controlId="birthDate">
                  <Form.Label>Data de Nascimento<span className="text-danger">*</span></Form.Label>
                  <InputGroup hasValidation>
                    <InputGroup.Text className="bg-light border-end-0">
                      <FontAwesomeIcon icon={faCalendar} className="text-muted" />
                    </InputGroup.Text>
                    <Form.Control
                      type="date"
                      name="birthDate"
                      value={formData.birthDate}
                      onChange={handleChange}
                      className="border-start-0"
                      required
                      isInvalid={!!errors.birthDate}
                    />
                    <Form.Control.Feedback type="invalid">
                      {errors.birthDate || 'Data de nascimento é obrigatória'}
                    </Form.Control.Feedback>
                  </InputGroup>
                </Form.Group>
              </Col>

              <Col md={6}>
                <Form.Group controlId="birthPlace">
                  <Form.Label>Local de Nascimento</Form.Label>
                  <InputGroup>
                    <InputGroup.Text className="bg-light border-end-0">
                      <FontAwesomeIcon icon={faMapMarker} className="text-muted" />
                    </InputGroup.Text>
                    <Form.Control
                      type="text"
                      name="birthPlace"
                      value={formData.birthPlace}
                      onChange={handleChange}
                      placeholder="Cidade/Estado"
                      className="border-start-0"
                    />
                  </InputGroup>
                </Form.Group>
              </Col>

              <Col md={6}>
                <Form.Group controlId="nationality">
                  <Form.Label>Nacionalidade</Form.Label>
                  <InputGroup>
                    <InputGroup.Text className="bg-light border-end-0">
                      <FontAwesomeIcon icon={faPassport} className="text-muted" />
                    </InputGroup.Text>
                    <Form.Control
                      type="text"
                      name="nationality"
                      value={formData.nationality}
                      onChange={handleChange}
                      placeholder="Nacionalidade"
                      className="border-start-0"
                    />
                  </InputGroup>
                </Form.Group>
              </Col>

              <Col md={12}>
                <Form.Group controlId="address">
                  <Form.Label>Endereço Completo</Form.Label>
                  <InputGroup>
                    <InputGroup.Text className="bg-light border-end-0">
                      <FontAwesomeIcon icon={faHome} className="text-muted" />
                    </InputGroup.Text>
                    <Form.Control
                      as="textarea"
                      rows={3}
                      name="address"
                      value={formData.address}
                      onChange={handleChange}
                      placeholder="Digite o endereço completo"
                      className="border-start-0 resize-none"
                    />
                  </InputGroup>
                </Form.Group>
              </Col>

            <div className="d-flex justify-content-end mt-4 border-top pt-4">
              <Button 
                variant="light" 
                as={Link}
                to="/people"
                className="me-2 px-4"
              >
                <FontAwesomeIcon icon={faArrowLeft} className="me-2" />
                Cancelar
              </Button>
              <Button 
                variant="success" 
                type="submit" 
                disabled={loading}
                className="px-4"
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
                    Salvando...
                  </>
                ) : (
                  <>
                    <FontAwesomeIcon icon={faSave} className="me-2" />
                    {isEditMode ? 'Atualizar' : 'Cadastrar'}
                  </>
                )}
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default PersonForm;
