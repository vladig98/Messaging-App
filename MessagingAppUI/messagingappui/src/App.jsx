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
import Chat from './components/chat/chat.jsx'
import { JWT_STORAGE_NAME, UsersStorageName, domainName, METHODS, ROUTES } from './constants';

function App() {
    const location = useLocation()
    const [userName, setUserName] = useState();
    const [password, setPassword] = useState();
    const [firstName, setFirstName] = useState();
    const [lastName, setLastName] = useState();
    const [email, setEmail] = useState();
    const [users, setUsers] = useState([])
    const navigate = useNavigate()
    const { jwt, clearAuthstate, loggedIn, login, register, logout } = useAuth();
    const { makeAnAPICall, handleResponse } = useAPI();

    //clears all or some of the hooks on a page change
    //retrieves the users
    useEffect(() => {
        clearState()
        collectUsersInfo()
    }, [location]);

    //clears the hooks
    //clears all if true is passed
    //if nothing is passed, the token remains
    function clearState(clearAll = false) {
        setUserName()
        setPassword()
        setFirstName()
        setLastName()
        setEmail()
        setUsers([])

        if (clearAll) {
            clearAuthstate()
        }
    }

    //checks if the user is authenticated
    //uses the local storage to fix an issue with a race condition on the hook state update
    function isLoggedIn() {
        return loggedIn && localStorage.getItem(JWT_STORAGE_NAME)
    }

    //collects the users info with an API call
    function collectUsersInfo() {
        if (isLoggedIn()) {
            getUsers();
        }
    }

    //gets the users on page load
    useEffect(() => {
        collectUsersInfo()
    }, []);

    //gets the users
    async function getUsers() {
        //makes an api call to get the users and stores them in the hook
        const response = await makeAnAPICall(ROUTES.GETUSERS, METHODS.GET, true);

        let data = await handleResponse(response)

        await updateUsers(data)
    }

    //check if the response data is in JSON format
    function isDataInJSONFormat(data) {
        return typeof (data) == "object"
    }

    //sets the users in localStorage and in a hook
    async function updateUsers(data) {
        //the users data will be in JSON format
        if (isDataInJSONFormat(data)) {
            data ? localStorage.setItem(UsersStorageName, JSON.stringify(data)) : localStorage.removeItem(UsersStorageName);
            data ? setUsers(data) : setUsers([]);
        }
    }

    //handles the registration logic
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

    //handles the login logic
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
                    />
                } />
                <Route path={ROUTES.LOGOUT} element={
                    <Logout
                        clear={clearState}
                    />
                } />
                <Route exact path={ROUTES.CHAT_ID} element={<Chat loggedIn={loggedIn} />} />
            </Routes>
        </div>
    );
}

export default App;