import { useEffect, memo } from 'react';
import { useNavigate } from 'react-router-dom';
import useAuth from './AuthCheck'
import { variables } from '../Variables';

const PrivateRoute = memo(function PrivateRoute({ children }) {
    const isAuthenticated = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        if (isAuthenticated === false) {
            navigate(variables.LOGIN_PAGE);
        }
    }, [isAuthenticated, navigate]);

    return isAuthenticated ? children : null;
});

export default PrivateRoute;