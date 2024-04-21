import { useEffect, useState } from 'react';
import './App.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import Error from './components/error/error.jsx';
import Login from './components/loginPage/loginPage.jsx';
import Register from './components/registerPage/registerPage.jsx';
import Home from './components/home/home.jsx';
import { useNavigate } from 'react-router-dom';

const JWTStorageName = "JWTAuthToken"
const UsersStorageName = "UsersInfo"

function App() {
    const [userName, setUserName] = useState();
    const [password, setPassword] = useState();
    const [firstName, setFirstName] = useState();
    const [lastName, setLastName] = useState();
    const [email, setEmail] = useState();
    const [jwt, setJwt] = useState(() => {
        const storedJwt = localStorage.getItem(JWTStorageName);
        return storedJwt ? storedJwt : "";
    });
    const [loggedIn, setLoggedIn] = useState(false)
    const [errors, setErrors] = useState([]);
    const [users, setUsers] = useState([])
    const navigate = useNavigate()

    useEffect(() => {
        if (jwt) {
            setLoggedIn(true)
        } else {
            setLoggedIn(false)
        }
    }, [jwt]);

    useEffect(() => {
        const storedUsers = localStorage.getItem(UsersStorageName);
        if (storedUsers !== null) {
            const parsedUsers = JSON.parse(storedUsers);
            setUsers(parsedUsers);
        }
    }, []);

    async function getUsers() {
        console.log(jwt)
        const response = await fetch('https://localhost:7238/users', {
            method: "GET",
            mode: 'cors',
            credentials: "include",
            headers: new Headers({
                'Authorization': 'Bearer ' + jwt,
            }), 
        });

        let data = await response.text();
        let json = await handleResponse(data)

        await updateUsers(json, data)
    }

    async function updateUsers(json, data) {
        if (json) {
            data ? localStorage.setItem(UsersStorageName, data) : localStorage.removeItem(UsersStorageName);
            data ? setUsers(data) : setUsers([]);
        } else {
            let errors = json.errors
            let err = [];

            for (let e in errors) {
                err.push(errors[e][0])
            }

            setErrors(err)
        }
    }

    async function updateJWT(json, data) {
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

    async function handleResponse(data) {
        let json;

        try {
            json = JSON.parse(data)
        } catch (e) {

        }

        return json;
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
        let json = await handleResponse(data)
        await updateJWT(json, data)
        await getUsers()

        navigate('/');
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
        let json = await handleResponse(data)
        await updateJWT(json, data)
        await getUsers()

        navigate('/');
    }

    return (
        <div>
            <Routes>
                <Route path="/" element={
                    <Home
                        loggedIn={loggedIn}
                        users={users}
                    />
                } />
                <Route path="/login" element={
                    <Login
                        setPassword={setPassword}
                        setUserName={setUserName}
                        login={login}
                        loggedIn={loggedIn}
                    />
                } />
                <Route path="/register" element={
                    <Register
                        register={register}
                        setFirstName={setFirstName}
                        setLastName={setLastName}
                        setPassword={setPassword}
                        setEmail={setEmail}
                        setUserName={setUserName}
                        loggedIn={loggedIn}
                    />
                } />
            </Routes>
            {/*{errors.map((item, index) => (*/}
            {/*    <Error key={index} message={item} />*/}
            {/*))}*/}
        </div>
    );
}

export default App;