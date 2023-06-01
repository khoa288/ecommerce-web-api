import { variables } from "../Variables";

class TotpService {
    constructor(httpService) {
        this.httpService = httpService;
    }

    async getQrCode() {
        const getQRCodeRequest = await this.httpService.get(
            variables.GET_QR_CODE
        );

        if (getQRCodeRequest.ok) {
            return await getQRCodeRequest.text();
        } else {
            return null;
        }
    }

    async validateQrCode(username, qrCode, totp) {
        const validateQrCodeRequest = await this.httpService.post(
            variables.VALIDATE_QR_CODE,
            {
                body: JSON.stringify({ username, qrCode, totp }),
            }
        );

        if (validateQrCodeRequest.ok) {
            return true;
        } else {
            return await validateQrCodeRequest.text();
        }
    }

    async changeTwoFactorAuth() {
        const changeTwoFactorAuthResponse = await this.httpService.post(
            variables.CHANGE_TWO_FACTOR_AUTH
        );

        if (changeTwoFactorAuthResponse.ok) {
            return true;
        } else {
            return null;
        }
    }

    async getTwoFactorAuth() {
        const getTwoFactorAuthResponse = await this.httpService.get(
            variables.GET_TWO_FACTOR_AUTH
        );

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
