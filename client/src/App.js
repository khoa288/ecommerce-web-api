import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import TotpActivate from "./pages/TotpActivate";
import TotpRequire from "./pages/TotpRequire";
import PrivateRoute from "./components/PrivateRoute";
import Loader from "./components/Loader";
import ServicesContext from "./services/ServicesContext";
import HttpService from "./services/HttpService";
import UserService from "./services/UserService";
import LocalStorageService from "./services/LocalStorageService.js";
import TotpService from "./services/TotpService";
import { variables } from "./Variables";

function App() {
    const httpService = new HttpService();
    const localStorageService = new LocalStorageService();
    const totpService = new TotpService(httpService);
    const userService = new UserService(
        httpService,
        totpService,
        localStorageService
    );

    return (
        <ServicesContext.Provider value={{ httpService, userService }}>
            <BrowserRouter>
                <Routes>
                    <Route path={variables.LOGIN_PAGE} element={<Login />} />
                    <Route
                        path={variables.REGISTER_PAGE}
                        element={<Register />}
                    />
                    <Route
                        path={variables.TOTP_REQUIRE}
                        element={
                            <PrivateRoute allowFirstFactor={true}>
                                <TotpRequire />
                            </PrivateRoute>
                        }
                    />
                    <Route
                        path={variables.DASHBOARD_PAGE}
                        element={
                            <PrivateRoute allowFirstFactor={false}>
                                <Dashboard />
                            </PrivateRoute>
                        }
                    />
                    <Route
                        path={variables.TOTP_ACTIVATE}
                        element={
                            <PrivateRoute allowFirstFactor={false}>
                                <TotpActivate />
                            </PrivateRoute>
                        }
                    />
                    <Route
                        path={variables.INVALID_PATH}
                        element={<Navigate to={variables.LOGIN_PAGE} replace />}
                    />
                </Routes>
            </BrowserRouter>
            <Loader />
        </ServicesContext.Provider>
    );
}

export default App;
