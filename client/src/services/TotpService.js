import { variables } from "../Variables";

class TotpService {
    constructor(httpService) {
        this.httpService = httpService;
    }

    async getQrCodeValue() {
        const getUserSecretCodeResponse = await this.httpService.get(variables.GET_SECRET_CODE);
        
        if (getUserSecretCodeResponse.ok) {
            return getUserSecretCodeResponse.text();
        } else {
            alert("Unauthorized");
        }
    }

    async validateTotp(totp) {
        const validateTotpResponse = await this.httpService.post(variables.VALIDATE_TOTP, {
            body: JSON.stringify({ totp }),
            credentials: "include",
        });

        if (validateTotpResponse.ok) {
            return true;
        } else {
            return false;
        }
    }

    async changeTwoFactorAuth() {
        const changeTwoFactorAuthResponse = await this.httpService.post(variables.CHANGE_TWO_FACTOR_AUTH, {
            credentials: "include"
        })

        if (changeTwoFactorAuthResponse.ok) {
            return true;
        } else {
            return false;
        }
    }

    async getTwoFactorAuth(){
        const getTwoFactorAuthResponse = await this.httpService.get(variables.GET_TWO_FACTOR_AUTH);
        if (getTwoFactorAuthResponse.ok) {
            const status = await getTwoFactorAuthResponse.text();
            if (status === "True") {
                return true;
            } else if (status === "False") {
                return false;
            }
        } else {
            return null;
        }
    }
}

export default TotpService;