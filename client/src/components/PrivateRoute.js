import { useEffect, memo } from "react";
import { useNavigate } from "react-router-dom";
import useAuth from "./AuthCheck";

const PrivateRoute = memo(function PrivateRoute({
    children,
    allowFirstFactor,
}) {
    const isAuthenticated = useAuth(allowFirstFactor);
    const navigate = useNavigate();

    useEffect(() => {
        if (isAuthenticated === false) {
            navigate(-1);
        }
    }, [isAuthenticated, navigate]);

    return isAuthenticated ? children : null;
});

export default PrivateRoute;
