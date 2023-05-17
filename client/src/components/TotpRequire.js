import React, { useContext } from 'react'
import { useNavigate } from 'react-router-dom';
import ServicesContext from '../services/ServicesContext';

const TotpRequire = () => {
    const navigate = useNavigate();
    const { userService } = useContext(ServicesContext);

    const handleSubmit = async (e) => {
		e.preventDefault();
		await userService.loginSecondFactor(e.target.totp.value, navigate);
	};
    
    return (
        <div>
            <section className="vh-100">
                <div className="row d-flex align-items-center justify-content-center h-50">
                    <div className="col"></div>
                    <div className="col align-items-center">
                        <div className="divider d-flex align-items-center my-4">
                        	<p className="text-center mx-3 mb-0" >Time-based OTP</p>
                    	</div>
                        <form onSubmit={handleSubmit}>
                            <input type="text" className="form-control" placeholder="Enter your TOTP" id="totp" name="totp" required />
                            <button type="submit" className="btn btn-block btn-center" style={{borderColor: '#000',}}>Submit</button>
                        </form>
                    </div>
                    <div className="col"></div>
                </div>
            </section>
        </div>
    )
}

export default TotpRequire;