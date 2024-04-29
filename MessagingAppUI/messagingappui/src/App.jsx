import { useEffect, useState } from 'react';
import './App.css';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import Login from './components/loginPage/loginPage.jsx';
import Register from './components/registerPage/registerPage.jsx';
import Home from './components/home/home.jsx';
import { useNavigate } from 'react-router-dom';
import Logout from './components/logout/logout';
import { useLocation } from 'react-router-dom';
import useAuth from './hooks/useAuth.js';
import useAPI from './hooks/useAPI.js';
import { JWT_STORAGE_NAME, UsersStorageName, domainName, METHODS, ROUTES } from './constants';

function App() {
    const location = useLocation()
    const [userName, setUserName] = useState();
    const [password, setPassword] = useState();
    const [firstName, setFirstName] = useState();
    const [lastName, setLastName] = useState();
    const [email, setEmail] = useState();
    const [errors, setErrors] = useState([]);
    const [users, setUsers] = useState([])
    const navigate = useNavigate()
    const { jwt, loggedIn, login, register, logout } = useAuth();
    const { makeAnAPICall, handleResponse } = useAPI();

    useEffect(() => {
    }, [errors])

    useEffect(clearState, [location]);

    function clearState(clearAll = false) {
        setUserName()
        setPassword()
        setFirstName()
        setLastName()
        setEmail()
        setErrors([])
        setUsers([])

        if (clearAll) {
            setJwt()
            setLoggedIn(false)
        }
    }

    useEffect(() => {
        const storedUsers = localStorage.getItem(UsersStorageName);
        if (storedUsers !== null) {
            const parsedUsers = JSON.parse(storedUsers);
            setUsers(parsedUsers);
        }
    }, []);

    async function getUsers() {
        const response = await makeAnAPICall(ROUTES.GETUSERS, METHODS.GET, true);

        let data = await handleResponse(response)

        await updateUsers(data)
    }

    async function updateUsers(data) {
        if (typeof (data) == "object") {
            data ? localStorage.setItem(UsersStorageName, JSON.stringify(data)) : localStorage.removeItem(UsersStorageName);
            data ? setUsers(data) : setUsers([]);
        }
    }

    function updateErrors(errs) {
        setErrors(prevErrors => [...prevErrors, errs]);
    }

    async function handleRegister(e) {
        e.preventDefault();

        try {
            await register(userName, password, firstName, lastName, email);
            await getUsers()
            navigate('/');
        } catch (error) {
            console.error('Login failed:', error.message);
        }
    }

    async function handleLogin(e) {
        e.preventDefault();

        try {
            await login(userName, password);
            await getUsers()
            navigate('/');
        } catch (error) {
            console.error('Login failed:', error.message);
        }
    }

    return (
        <div>
            <Routes>
                <Route path={ROUTES.HOME} element={
                    <Home
                        loggedIn={loggedIn}
                        users={users}
                    />
                } />
                <Route path={ROUTES.LOGIN} element={
                    <Login
                        setPassword={setPassword}
                        setUserName={setUserName}
                        login={handleLogin}
                        loggedIn={loggedIn}
                        errors={errors}
                    />
                } />
                <Route path={ROUTES.REGISTER} element={
                    <Register
                        register={handleRegister}
                        setFirstName={setFirstName}
                        setLastName={setLastName}
                        setPassword={setPassword}
                        setEmail={setEmail}
                        setUserName={setUserName}
                        loggedIn={loggedIn}
                        errors={errors}
                    />
                } />
                <Route path={ROUTES.LOGOUT} element={
                    <Logout
                        clear={clearState}
                    />
                } />
            </Routes>
        </div>
    );
}

export default App;