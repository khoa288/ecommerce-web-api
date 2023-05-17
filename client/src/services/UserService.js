import { variables } from "../Variables";

class UserService {
    constructor(httpService, totpService, localStorageService, ) {
        this.httpService = httpService;
        this.totpService = totpService;
        this.localStorageService = localStorageService;
    }

    async login(username, password, navigate) {
        const response = await this.httpService.post(variables.LOGIN_URL, {
            body: JSON.stringify({ username, password }),
            credentials: "include",
        });

        if (response.ok) {
            await this.localStorageService.setUsername(username);
            const status = await response.text();
            if (status === "Unauthorized") {
                navigate(variables.TOTP_REQUIRE);
            } else if (status === "Success") {
                navigate(variables.DASHBOARD_PAGE);
            }
        } else {
            alert("Login failed");
        }
    }

    async loginSecondFactor(totp, navigate) {
        const username = this.localStorageService.getUsername();

        if (username  === null) {
            await navigate(variables.LOGIN_PAGE);
        }

        const response = await this.httpService.post(variables.LOGIN_SECOND_FACTOR, {
            body: JSON.stringify({ username, totp }),
            credentials: "include",
        });

        if (response.ok) {
            await navigate(variables.DASHBOARD_PAGE);
        } else {
            alert("Login failed");
        }
    }

    async register(username, password, confirmPassword, navigate) {
        if (password !== confirmPassword) {
			alert("Passwords do not match");
			return;
		}

        const response = await this.httpService.post(variables.REGISTER_URL, {
            body: JSON.stringify({ username, password, confirmPassword }),
        });

        if (response.ok) {
            await navigate(variables.LOGIN_PAGE);
        } else {
            alert("Register failed");
        }
    }

    async logout(navigate) {
        let username = this.localStorageService.getUsername();
		if (!username) {
			const getUsernameResponse = await this.httpService.get(variables.GET_USERNAME);
            username = getUsernameResponse.text();
		}

		const revokeTokenResponse = await this.httpService.delete(variables.REVOKE_TOKEN + username);

		if (revokeTokenResponse.ok) {
			await this.localStorageService.removeUsername();
			navigate(variables.LOGIN_PAGE);
		} else {
			alert("Logout failed");
		}
    }

    async authenticated() {
        const getUsernameResponse = await this.httpService.get(variables.GET_USERNAME);
        const username = await getUsernameResponse.text();

        if (username) {
            return true;
        } else {
            const refreshTokenResponse = await this.httpService.get(variables.REFRESH_TOKEN);
            const refreshToken = await refreshTokenResponse.text();
            if (refreshToken === 'Success') {
                return true;
            }
        }
        return false;
    }

    async activateTotp(totp, navigate){
        const response = await this.totpService.validateTotp(totp);
        if (response) {
            await this.totpService.changeTwoFactorAuth();
            navigate(variables.DASHBOARD_PAGE);
        } else {
            alert("Invalid OTP");
        }
    }
}

export default UserService;