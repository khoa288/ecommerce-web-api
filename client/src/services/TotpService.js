import { variables } from "../Variables";

class TotpService {
	constructor(httpService) {
		this.httpService = httpService;
	}

	async getQrCodeValue() {
		const getQRCodeValueRequest = await this.httpService.get(
			variables.GET_SECRET_CODE
		);

		if (getQRCodeValueRequest.ok) {
			return await getQRCodeValueRequest.text();
		} else {
			return null;
		}
	}

	async validateTotp(totp) {
		const validateTotpResponse = await this.httpService.post(
			variables.VALIDATE_TOTP,
			{
				body: JSON.stringify({ totp }),
			}
		);

		if (validateTotpResponse.ok) {
			return true;
		} else {
			return null;
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
