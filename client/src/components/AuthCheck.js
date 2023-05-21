import { useState, useEffect, useContext } from "react";
import ServicesContext from "../services/ServicesContext";

const useAuth = (allowFirstFactor) => {
	const [isAuthenticated, setIsAuthenticated] = useState(null);
	const { userService } = useContext(ServicesContext);

	useEffect(() => {
		async function checkAuth(allowFirstFactor) {
			const authStatus = await userService.authenticated(
				allowFirstFactor
			);
			setIsAuthenticated(authStatus);
		}

		checkAuth(allowFirstFactor);
	}, [userService, allowFirstFactor]);

	return isAuthenticated;
};

export default useAuth;
