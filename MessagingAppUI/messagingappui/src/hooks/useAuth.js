import { useState, useEffect } from 'react';
import useAPI from './useAPI.js';
import { JWT_STORAGE_NAME, UsersStorageName, domainName, METHODS, ROUTES } from '../constants.js';

//utility function that handles app authentication
function useAuth() {
    const [jwt, setJwt] = useState(() => localStorage.getItem(JWT_STORAGE_NAME) || '');
    const [loggedIn, setLoggedIn] = useState(() => !!jwt);
    const { makeAnAPICall, handleResponse } = useAPI();

    //sets the loggedIn property whenever we update the JWT
    useEffect(() => {
        setLoggedIn(!!jwt);
    }, [jwt]);

    //checks if the data is in text format and not as JSON
    function isTheDataInTextFormat(data) {
        return typeof (data) == "string"
    }

    //updates the JWT token
    async function updateJWT(data) {
        //the JWT will be in text format
        if (isTheDataInTextFormat(data)) {
            //stores the token in the local storage and updates the hook for the JWT
            data ? localStorage.setItem(JWT_STORAGE_NAME, data) : localStorage.removeItem(JWT_STORAGE_NAME);
            data ? setJwt(data) : setJwt();
        }
    }

    //logs in a user
    const login = async (username, password) => {
        const body = { username, password };

        try {
            //makes an API call to log the user in and updates the JWT
            const response = await makeAnAPICall(ROUTES.LOGIN, METHODS.POST, false, body);
            const data = await handleResponse(response);
            updateJWT(data)
        } catch (error) {
            console.error('Login failed:', error);
            throw new Error('Login failed');
        }
    };

    //registers a user
    const register = async (username, password, firstName, lastName, email) => {
        const body = { username, password, firstName, lastName, email };

        try {
            //makes an API call to register the user and updates the JWT
            const response = await makeAnAPICall(ROUTES.REGISTER, METHODS.POST, false, body);
            const data = await handleResponse(response);
            updateJWT(data);
        } catch (error) {
            console.error('Registration failed:', error);
            throw new Error('Registration failed');
        }
    };

    //logsout the user
    const logout = () => {
        //clears the JWT and clears the local storage
        clearAuthstate()
    };

    //clears the states
    const clearAuthstate = () => {
        setJwt('')
        setLoggedIn(false)
        localStorage.clear();
    }

    return { jwt, loggedIn, login, register, logout, clearAuthstate };
}

export default useAuth;
