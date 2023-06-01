import React, { useContext, useEffect, useState } from "react";
import { QRCodeSVG } from "qrcode.react";
import ServicesContext from "../services/ServicesContext";
import { useNavigate } from "react-router-dom";

const TotpActivate = () => {
    const navigate = useNavigate();
    const { userService } = useContext(ServicesContext);
    const [qrCode, setQrCode] = useState(null);

    useEffect(() => {
        async function getQrCode(navigate) {
            const qrcode = await userService.getQrCode(navigate);
            setQrCode(qrcode);
        }

        getQrCode(navigate);
    }, [userService, navigate]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        await userService.activateTwoFactorAuth(
            qrCode,
            e.target.totp.value,
            navigate
        );
    };

    return (
        <section className="vh-100">
            <div className="row d-flex align-items-center justify-content-center h-50">
                <div className="col" />
                <div className="col align-items-center">
                    <QRCodeSVG className="btn-center" value={qrCode} />
                    <div className="divider d-flex align-items-center my-4">
                        <p className="text-center mx-3 mb-0">Time-based OTP</p>
                    </div>
                    <form onSubmit={handleSubmit}>
                        <input
                            type="text"
                            className="form-control"
                            placeholder="Enter your TOTP"
                            id="totp"
                            name="totp"
                            required
                        />
                        <button
                            type="submit"
                            className="btn btn-block btn-center"
                            style={{ borderColor: "#000" }}
                        >
                            Submit
                        </button>
                    </form>
                </div>
                <div className="col" />
            </div>
        </section>
    );
};

export default TotpActivate;
