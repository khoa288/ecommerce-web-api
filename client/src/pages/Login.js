import React, { useContext } from "react";
import { useNavigate, Link } from "react-router-dom";
import loginPicture from "../css/image/loginPic.png";
import ServicesContext from "../services/ServicesContext";

const Login = () => {
    const navigate = useNavigate();
    const { userService } = useContext(ServicesContext);

    const handleSubmit = async (e) => {
        e.preventDefault();

        await userService.login(
            e.target.username.value,
            e.target.password.value,
            navigate
        );
    };

    return (
        <section className="vh-100">
            <div className="container py-5 h-100">
                <div className="row d-flex align-items-center justify-content-center h-100">
                    <div className="col-md-8 col-lg-7 col-xl-6">
                        <img
                            src={loginPicture}
                            className="img-fluid"
                            alt="Phone"
                        />
                    </div>
                    <div className="col-md-7 col-lg-5 col-xl-5 offset-xl-1">
                        <Link to="/register">
                            <button
                                className="btn btn-lg btn-block btn-center"
                                style={{
                                    color: "#065FD4",
                                    borderColor: "#065FD4",
                                }}
                            >
                                Register new account
                            </button>
                        </Link>

                        <div className="divider d-flex align-items-center my-4">
                            <p
                                className="text-center mx-3 mb-0"
                                style={{ color: "#E74425" }}
                            >
                                LOGIN
                            </p>
                        </div>

                        <form name="login" onSubmit={handleSubmit}>
                            <div className="form-outline mb-4">
                                <label
                                    htmlFor="username"
                                    className="form-label"
                                >
                                    Username
                                </label>
                                <input
                                    type="text"
                                    className="form-control form-control-lg"
                                    placeholder="Enter your username"
                                    id="username"
                                    name="username"
                                    autoComplete="off"
                                    required
                                />
                            </div>

                            <div className="form-outline mb-4">
                                <label
                                    htmlFor="password"
                                    className="form-label"
                                >
                                    Password
                                </label>
                                <input
                                    type="password"
                                    className="form-control form-control-lg"
                                    placeholder="Enter your password"
                                    id="password"
                                    name="password"
                                    autoComplete="off"
                                    required
                                />
                            </div>

                            <button
                                type="submit"
                                className="btn btn-lg btn-block btn-center"
                                style={{
                                    color: "#ffffff",
                                    backgroundColor: "#E73125",
                                }}
                            >
                                Login
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default Login;
