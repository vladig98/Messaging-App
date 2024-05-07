import Navbar from '../../components/navbar/navbar.jsx';

function Register({ setFirstName, setLastName, setPassword, setEmail, setUserName, register, loggedIn }) {
    return (
        <>
            <Navbar
                loggedIn={loggedIn}
            />
            <h1 className="mx-auto w-50 text-center mt-3">Register</h1>
            <form onSubmit={register} className={"w-50 mx-auto mt-5 text-end"}>
                <div className={"form-floating mb-3"}>
                    <input placeholder={"First name"} id={"registerFirstName"} className={"form-control"} type="text" onChange={e => setFirstName(e.target.value)} />
                    <label htmlFor={"registerFirstName"} className={"form-label"}>First name</label>
                </div>
                <div className={"form-floating mb-3"}>
                    <input placeholder={"Last name"} id={"registerLastName"} className={"form-control"} type="text" onChange={e => setLastName(e.target.value)} />
                    <label htmlFor={"registerLastName"} className={"form-label"}>Last name</label>
                </div>
                <div className={"form-floating mb-3"}>
                    <input placeholder={"Login name"} id={"registerLoginName"} className={"form-control"} type="text" onChange={e => setUserName(e.target.value)} />
                    <label htmlFor={"registerLoginName"} className={"form-label"}>Login name</label>
                </div>
                <div className={"form-floating mb-3"}>
                    <input placeholder={"Password"} id={"registerPassword"} className={"form-control"} type="password" onChange={e => setPassword(e.target.value)} />
                    <label htmlFor={"registerPassword"} className={"form-label"}>Password</label>
                </div>
                <div className={"form-floating mb-3"}>
                    <input placeholder={"Email"} id={"registerEmail"} className={"form-control"} type="email" onChange={e => setEmail(e.target.value)} />
                    <label htmlFor={"registerEmail"} className={"form-label"}>Email</label>
                </div>
                <button className={"btn btn-primary"} type="submit">Register</button>
            </form>
        </>
    );
}

export default Register;