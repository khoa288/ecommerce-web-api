import { useState, useEffect, useContext } from 'react';
import ServicesContext from '../services/ServicesContext';

const useAuth = () => {
    const [isAuthenticated, setIsAuthenticated] = useState(null);
    const { userService } = useContext(ServicesContext);

    useEffect(() => {
        async function checkAuth() {
            const authStatus = await userService.authenticated();
            setIsAuthenticated(authStatus);
        }

        checkAuth();
    }, [userService]);

    return isAuthenticated;
}

export default useAuth;