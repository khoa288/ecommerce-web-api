import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import Dashboard from './components/Dashboard';
import PrivateRoute from './components/PrivateRoute';
import TotpRequire from './components/TotpRequire';
import TotpActivate from './components/TotpActivate';
import ServicesContext from './services/ServicesContext';
import HttpService from './services/HttpService';
import UserService from './services/UserService';
import LocalStorageService from './services/LocalStorageService.js';
import TotpService from './services/TotpService';
import { variables } from './Variables';
import Loader from './components/Loader';

function App() {
	const httpService = new HttpService();
	const localStorageService = new LocalStorageService();
	const totpService = new TotpService(httpService);
	const userService = new UserService(httpService, totpService, localStorageService);

	return (
		<ServicesContext.Provider value={{ httpService, userService, totpService }}>
			<BrowserRouter>
				<Routes>
					<Route path={variables.LOGIN_PAGE} element={<Login />} />
					<Route path={variables.TOTP_REQUIRE} element={<TotpRequire />} />
					<Route path={variables.REGISTER_PAGE} element={<Register />} />
					<Route
						path={variables.DASHBOARD_PAGE}
						element={
						<PrivateRoute>
							<Dashboard />
						</PrivateRoute>} />
					<Route
						path={variables.TOTP_ACTIVATE}
						element={
						<PrivateRoute>
							<TotpActivate />
						</PrivateRoute>} />
				</Routes>
			</BrowserRouter>
			<Loader />
		</ServicesContext.Provider>
	);
}

export default App;