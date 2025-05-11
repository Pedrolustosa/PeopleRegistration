import React, { useState, useEffect } from 'react';
import {
  Container,
  Card,
  Row,
  Col,
  Button,
  Spinner,
  Modal,
  Pagination
} from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { 
  faUserPlus,
  faPencilAlt,
  faTrash,
  faEnvelope,
  faCalendar,
  faHome
} from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import PersonService from '../../services/person.service';
import { GENDER_OPTIONS } from '../../constants';

const PeopleList = () => {
  const [people, setPeople] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [pagination, setPagination] = useState({
    currentPage: 1,
    pageSize: 9,
    totalPages: 1,
    totalRecords: 0
  });
  const navigate = useNavigate();
  const { currentUser } = useAuth();

  const dataFetchedRef = React.useRef(false);
  
  const fetchPeople = async (page = pagination.currentPage) => {
    try {
      setLoading(true);
      const response = await PersonService.getAll(page, pagination.pageSize);
      if (response.success) {
        setPeople(response.data);
        setPagination(prev => ({
          ...prev,
          currentPage: page,
          totalPages: response.totalPages,
          totalRecords: response.totalRecords
        }));
        if (response.message) {
          toast.success(response.message);
        }
      } else {
        setPeople([]);
        if (response.errors?.length > 0) {
          response.errors.forEach(error => toast.error(error));
        } else if (response.message) {
          toast.error(response.message);
        }
        setError(response.message || response.errors?.[0]);
      }
    } catch (error) {
      setPeople([]);
      if (error.errors?.length > 0) {
        error.errors.forEach(err => toast.error(err));
        setError(error.errors[0]);
      } else {
        toast.error(error.message);
        setError(error.message);
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (currentUser) {
      console.log('Usuário autenticado, buscando pessoas...');
      fetchPeople(pagination.currentPage);
    }
    return () => {
      dataFetchedRef.current = false;
    };
  }, [currentUser, pagination.currentPage]);

  useEffect(() => {
    if (pagination.currentPage !== 1) {
      fetchPeople(1);
    }
  }, [pagination.currentPage]);

  const formatDate = (dateString) => {
    const options = { day: '2-digit', month: '2-digit', year: 'numeric' };
    return new Date(dateString).toLocaleDateString('pt-BR', options);
  };

  const formatCpf = (cpf) => {
    if (!cpf) return '';
    const cpfClean = cpf.replace(/\D/g, '');
    return cpfClean.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  };

  const getGenderText = (genderValue) => {
    if (genderValue === null || genderValue === undefined) return 'Não informado';
    const gender = GENDER_OPTIONS.find(g => g.value === genderValue);
    return gender ? gender.label : 'Não informado';
  };

  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [personToDelete, setPersonToDelete] = useState(null);

  const handleDeleteClick = (person) => {
    setPersonToDelete(person);
    setShowDeleteModal(true);
  };

  const handleDeleteConfirm = async () => {
    if (!personToDelete) return;

    try {
      const response = await PersonService.delete(personToDelete.id);
      if (response.success) {
        toast.success(response.message);
        fetchPeople(pagination.currentPage);
      } else {
        if (response.errors?.length > 0) {
          response.errors.forEach(error => toast.error(error));
        } else {
          toast.error(response.message);
        }
      }
    } catch (error) {
      if (error.errors?.length > 0) {
        error.errors.forEach(err => toast.error(err));
      } else {
        toast.error(error.message || 'Erro ao excluir pessoa');
      }
    } finally {
      setShowDeleteModal(false);
      setPersonToDelete(null);
    }
  };

  const handleDeleteCancel = () => {
    setShowDeleteModal(false);
    setPersonToDelete(null);
  };

  return (
    <Container className="py-4" style={{ maxWidth: '1400px' }}>
      <Card className="shadow-sm border-0">
        <Card.Header className="bg-white py-3 border-0">
          <Row className="align-items-center">
            <Col>
              <h4 className="mb-0 px-1 mx-1">
                <FontAwesomeIcon icon={faUserPlus} className="text-success me-2" />
                Lista de Pessoas
              </h4>
            </Col>
            <Col xs="auto">
              <Link 
                to="/people/create" 
                className="btn btn-success px-4"
              >
                <FontAwesomeIcon icon={faUserPlus} className="me-2" />
                Nova Pessoa
              </Link>
            </Col>
          </Row>
        </Card.Header>
        <Card.Body className="px-4 pb-4">



          {loading ? (
            <div className="text-center py-5">
              <Spinner animation="border" variant="primary" />
              <p className="text-muted mt-2">Carregando pessoas...</p>
            </div>
          ) : (
            <div className="container-fluid px-4">
              <div className="row g-4">
                {people.map((person) => (
                  <div key={person.id} className="col-12 col-md-6 col-lg-4">
                    <Card className="border-0 shadow-sm h-100" style={{ borderRadius: '10px' }}>
                      <Card.Body className="p-4">
                        <div className="d-flex justify-content-between align-items-start mb-4">
                          <h5 className="card-title text-success mb-0 fs-5">{person.name}</h5>
                          <div className="btn-group">
                            <Button
                              variant="success"
                              size="sm"
                              title="Editar"
                              onClick={() => navigate(`/people/edit/${person.id}`)}
                              className="me-2"
                            >
                              <FontAwesomeIcon icon={faPencilAlt} />
                            </Button>
                            <Button
                              variant="danger"
                              size="sm"
                              onClick={() => handleDeleteClick(person)}
                              title="Excluir"
                            >
                              <FontAwesomeIcon icon={faTrash} />
                            </Button>
                          </div>
                        </div>

                        <div className="mb-3">
                          <div className="text-muted mb-2 d-flex align-items-center">
                            <FontAwesomeIcon icon={faEnvelope} className="me-2" fixedWidth />
                            <span className="text-break">{person.email || 'Não informado'}</span>
                          </div>
                          <div className="text-muted mb-2 d-flex align-items-center">
                            <FontAwesomeIcon icon={faCalendar} className="me-2" fixedWidth />
                            <span>{new Date(person.birthDate).toLocaleDateString('pt-BR')}</span>
                          </div>
                          {person.address && (
                            <div className="text-muted d-flex align-items-start">
                              <FontAwesomeIcon icon={faHome} className="me-2 mt-1" fixedWidth />
                              <span className="text-break">{person.address}</span>
                            </div>
                          )}
                        </div>
                      </Card.Body>
                    </Card>
                  </div>
                ))}
              </div>
            </div>
          )}

          {!loading && people.length === 0 && (
            <div className="text-center py-5">
              <p className="text-muted mb-0">Nenhuma pessoa cadastrada</p>
            </div>
          )}

          {!loading && people.length > 0 && (
            <div className="d-flex justify-content-between align-items-center mt-4">
              <small className="text-muted">
                Mostrando {(pagination.currentPage - 1) * pagination.pageSize + 1} a{' '}
                {Math.min(pagination.currentPage * pagination.pageSize, pagination.totalRecords)} de{' '}
                {pagination.totalRecords} registros
              </small>
              
              <div className="d-flex gap-2">
                <Button
                  variant="outline-primary"
                  size="sm"
                  disabled={pagination.currentPage === 1}
                  onClick={() => fetchPeople(pagination.currentPage - 1)}
                >
                  Anterior
                </Button>
                
                {Array.from({ length: pagination.totalPages }, (_, i) => i + 1)
                  .filter(page => {
                    const current = pagination.currentPage;
                    return page === 1 || 
                           page === pagination.totalPages || 
                           (page >= current - 1 && page <= current + 1);
                  })
                  .map((page, index, array) => (
                    <React.Fragment key={page}>
                      {index > 0 && array[index - 1] !== page - 1 && (
                        <Button variant="outline-secondary" size="sm" disabled>...</Button>
                      )}
                      <Button
                        variant={pagination.currentPage === page ? 'primary' : 'outline-primary'}
                        size="sm"
                        onClick={() => fetchPeople(page)}
                      >
                        {page}
                      </Button>
                    </React.Fragment>
                  ))}
                
                <Button
                  variant="outline-primary"
                  size="sm"
                  disabled={pagination.currentPage === pagination.totalPages}
                  onClick={() => fetchPeople(pagination.currentPage + 1)}
                >
                  Próxima
                </Button>
              </div>
            </div>
          )}
        </Card.Body>
      </Card>

      {/* Modal de confirmação de exclusão */}
      <Modal show={showDeleteModal} onHide={handleDeleteCancel} centered>
        <Modal.Header closeButton className="border-0 pb-0">
          <Modal.Title className="text-danger">
            <FontAwesomeIcon icon={faTrash} className="me-2" />
            Confirmar Exclusão
          </Modal.Title>
        </Modal.Header>
        <Modal.Body className="pt-2">
          {personToDelete && (
            <p className="mb-0">Tem certeza que deseja excluir a pessoa <strong>{personToDelete.name}</strong>?</p>
          )}
        </Modal.Body>
        <Modal.Footer className="border-0">
          <Button 
            variant="light" 
            onClick={handleDeleteCancel}
            className="px-4"
          >
            Cancelar
          </Button>
          <Button 
            variant="danger" 
            onClick={handleDeleteConfirm}
            className="px-4"
          >
            Sim, Excluir
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};

export default PeopleList;
