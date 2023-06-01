import { variables } from "../Variables";

class UserService {
    constructor(httpService, totpService, localStorageService) {
        this.httpService = httpService;
        this.totpService = totpService;
        this.localStorageService = localStorageService;
    }

    async login(username, password, navigate) {
        const response = await this.httpService.post(variables.LOGIN_URL, {
            body: JSON.stringify({ username, password }),
        });
        const status = await response.text();

        if (response.ok) {
            await this.localStorageService.setUsername(username);

            if (status === "Unauthorized") {
                navigate(variables.TOTP_REQUIRE);
            } else {
                navigate(variables.DASHBOARD_PAGE);
            }
        } else {
            alert(status);
        }
    }

    async loginSecondFactor(totp, navigate) {
        const username = this.localStorageService.getUsername();

        const response = await this.httpService.post(
            variables.LOGIN_SECOND_FACTOR,
            {
                body: JSON.stringify({ username, totp }),
            }
        );

        if (response.ok) {
            await navigate(variables.DASHBOARD_PAGE);
        } else {
            alert(await response.text());
        }
    }

    async register(username, password, confirmPassword, navigate) {
        if (password !== confirmPassword) {
            alert("Passwords don't match");
            return;
        }

        const response = await this.httpService.post(variables.REGISTER_URL, {
            body: JSON.stringify({ username, password, confirmPassword }),
        });

        if (response.ok) {
            await navigate(variables.LOGIN_PAGE);
        } else {
            alert(await response.text());
        }
    }

    async logout(navigate) {
        const response = await this.httpService.delete(variables.REVOKE_TOKEN);

        if (response.ok) {
            await this.localStorageService.removeUsername();
            navigate(variables.LOGIN_PAGE);
        } else {
            alert(await response.text());
        }
    }

    async authenticated(allowFirstFactor) {
        const response = await this.httpService.get(variables.IS_AUTHENTICATED);
        const status = await response.text();

        if (allowFirstFactor && response.ok && status === "Unauthorized") {
            return true;
        } else if (
            !allowFirstFactor &&
            response.ok &&
            status === "Unauthorized"
        ) {
            return false;
        } else if (!allowFirstFactor && response.ok) {
            return true;
        } else {
            return false;
        }
    }

    async getQrCode(navigate) {
        const response = await this.totpService.getQrCode();

        if (response !== null) {
            return response;
        } else {
            navigate(variables.LOGIN_PAGE);
        }
    }

    async activateTwoFactorAuth(qrCode, totp, navigate) {
        const username = this.localStorageService.getUsername();
        const response = await this.totpService.validateQrCode(
            username,
            qrCode,
            totp
        );

        if (response === true) {
            await this.totpService.changeTwoFactorAuth();
            navigate(variables.DASHBOARD_PAGE);
        } else {
            alert(response);
        }
    }

    async deactivateTwoFactorAuth(setIsTwoFactorAuth, navigate) {
        const response = await this.totpService.changeTwoFactorAuth();

        if (response !== null) {
            setIsTwoFactorAuth(false);
        } else {
            navigate(variables.LOGIN_PAGE);
        }
    }

    async getTwoFactorAuth(setIsTwoFactorAuth, navigate) {
        const response = await this.totpService.getTwoFactorAuth();

        if (response !== null) {
            setIsTwoFactorAuth(response);
        } else {
            navigate(variables.LOGIN_PAGE);
        }
    }
}

export default UserService;
