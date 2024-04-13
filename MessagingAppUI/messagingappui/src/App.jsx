import { useEffect, useState } from 'react';
import './App.css';

const JWTStorageName = "JWTAuthToken"

function Register({ setFirstName, setLastName, setPassword, setEmail, setUserName, visible, register, setVisible, clearErrors }) {
    return (
        <>
            {visible && (
                <div>
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
                        <button className={"btn btn-danger ms-3"} onClick={() => {
                            setVisible(!visible)
                            clearErrors()
                        }}>Back</button>
                    </form>
                </div>
            )}
        </>
    );
}

function Login({ setUserName, setPassword, visible, login }) {
    return (
        <div>
        {visible && (
                <div>
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
                </div>
            )}
        </div>
    );
}

function Error({ message }) {
    return (
        <div className={"mx-auto w-50 text-danger"}>
            <span>{message}</span>
        </div>
    )
}

function User({user, message}) {
    return (
        <div>
            <img />
            <h4></h4>
            <span></span>
        </div>
    )
}


function App() {
    const [userName, setUserName] = useState();
    const [password, setPassword] = useState();
    const [firstName, setFirstName] = useState();
    const [lastName, setLastName] = useState();
    const [email, setEmail] = useState();
    const [registerVisible, setRegisterVisible] = useState(false);
    const [loginVisible, setLoginVisible] = useState(true);
    const [jwt, setJwt] = useState(() => {
        const storedJwt = localStorage.getItem(JWTStorageName);
        return storedJwt ? storedJwt : "";
    });
    const [errors, setErrors] = useState([]);

    useEffect(() => {
        jwt ? setLoginVisible(false) : setLoginVisible(true);
        setRegisterVisible(false)
    }, [jwt]);

    function clearErrors() {
        setErrors([])
    }

    async function register(e) {
        e.preventDefault();
        const response = await fetch('https://localhost:7238/register', {
            method: "POST",
            mode: 'cors',
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(
                {
                    username: userName,
                    password: password,
                    firstName: firstName,
                    lastName: lastName,
                    email: email
                }
            )
        });
        const data = await response.text();
        let json;

        try {
            json = JSON.parse(data)
        } catch (e) {

        }

        if (!json) {
            data ? localStorage.setItem(JWTStorageName, data) : localStorage.removeItem(JWTStorageName);
            data ? setJwt(data) : setJwt();
        } else {
            let errors = json.errors
            let err = [];

            for (let e in errors) {
                err.push(errors[e][0])
            }

            setErrors(err)
        }
    }

    async function login(e) {
        e.preventDefault();
        const response = await fetch('https://localhost:7238/login', {
            method: "POST",
            mode: 'cors',
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(
                {
                    username: userName,
                    password: password,
                }
            )
        });
        const data = await response.text();
        let json;

        try {
            json = JSON.parse(data)
        } catch (e) {

        }

        if (!json) {
            data ? localStorage.setItem(JWTStorageName, data) : localStorage.removeItem(JWTStorageName);
            data ? setJwt(data) : setJwt();
        } else {
            let errors = json.errors
            let err = [];

            for (let e in errors) {
                err.push(errors[e][0])
            }

            setErrors(err)
        }
    }

    let heading;

    if (registerVisible) {
        heading = "Register to chat"
    } else {
        heading = "Messaging App"
    }

    return (
        <div>
            <h1 className={"mx-auto text-center mt-5"}>{heading}</h1>
            {errors.map((item, index) => (
                <Error key={index} message={item} />
            ))}
            {registerVisible === false && (
                <div>
                    <Login setPassword={setPassword} setUserName={setUserName} visible={loginVisible} login={login} />
                    {loginVisible && < div className={"mx-auto w-50 text-center pt-5"}>
                        <a className={"link-primary"} onClick={(e) => { e.preventDefault(); setRegisterVisible(true), setErrors([]) }}>
                            Click here to Register
                        </a>
                    </div>}
                </div>
            )}
            <Register
                setVisible={setRegisterVisible}
                register={register}
                setFirstName={setFirstName}
                setLastName={setLastName}
                setPassword={setPassword}
                setEmail={setEmail}
                setUserName={setUserName}
                visible={registerVisible}
                clearErrors={clearErrors}
            />
        </div>
    );
}

export default App;