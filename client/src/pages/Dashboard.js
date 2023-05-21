import React, { useContext, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import ServicesContext from "../services/ServicesContext";
import { variables } from "../Variables";

const Dashboard = () => {
	const navigate = useNavigate();
	const [isTwoFactorAuth, setIsTwoFactorAuth] = useState("");
	const { userService } = useContext(ServicesContext);

	useEffect(() => {
		async function getTwoFactorAuthStatus() {
			await userService.getTwoFactorAuth(setIsTwoFactorAuth, navigate);
		}

		getTwoFactorAuthStatus();
	}, [userService, navigate]);

	const handleSwitch = async (e) => {
		e.preventDefault();

		if (isTwoFactorAuth) {
			await userService.turnOffTwoFactorAuth(
				setIsTwoFactorAuth,
				navigate
			);
		} else {
			navigate(variables.TOTP_ACTIVATE);
		}
	};

	const handleLogout = async (e) => {
		e.preventDefault();
		await userService.logout(navigate);
	};

	return (
		<section className="vh-100">
			<div className="container py-5 h-100">
				<h1>Dashboard</h1>
				<p>Welcome to the dashboard!</p>
				<div className="form-check form-switch mb-3">
					<input
						className="form-check-input"
						type="checkbox"
						role="switch"
						onChange={handleSwitch}
						checked={isTwoFactorAuth}
					/>
					<label
						className="form-check-label"
						htmlFor="flexSwitchCheckDefault"
					>
						Two-factor authentication
					</label>
				</div>
				<button
					className="btn btn-block"
					style={{ borderColor: "#000" }}
					onClick={handleLogout}
				>
					Logout
				</button>
			</div>
		</section>
	);
};

export default Dashboard;
