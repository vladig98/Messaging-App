import Navbar from '../../components/navbar/navbar.jsx';
import Error from '../../components/error/error.jsx'

function Login({ setUserName, setPassword, login, loggedIn, errors }) {
    return (
        <>
            <Navbar
                loggedIn={loggedIn}
            />
            <h1 className="mx-auto w-50 text-center mt-3">Login</h1>
            {errors.map((item, index) => (
                <Error key={index} message={item} />
            ))}
            <form onSubmit={login} className={"w-50 mx-auto mt-5 text-end"}>
                <div className={"form-floating mb-3"}>
                    <input placeholder={"Login name"} id={"loginLoginname"} className={"form-control"} type="text" onChange={e => setUserName(e.target.value)} />
                    <label htmlFor={"loginLoginname"} className={"form-label"}>Login name:</label>
                </div>
                <div className={"form-floating mb-3"}>
                    <input placeholder={"Password"} id={"loginPassword"} className={"form-control"} type="password" onChange={e => setPassword(e.target.value)} />
                    <label htmlFor={"loginPassword"} className={"form-label"}>Password</label>
                </div>
                <button className={"btn btn-primary"} type="submit">Login</button>
             </form>
        </>
    );
}

export default Login;