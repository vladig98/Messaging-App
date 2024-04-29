import { useState, useEffect } from 'react';
import useAPI from './useAPI.js';
import { JWT_STORAGE_NAME, UsersStorageName, domainName, METHODS, ROUTES } from '../constants.js';

function useAuth() {
    const [jwt, setJwt] = useState(() => localStorage.getItem(JWT_STORAGE_NAME) || '');
    const [loggedIn, setLoggedIn] = useState(false);
    const { makeAnAPICall, handleResponse } = useAPI();

    useEffect(() => {
        setLoggedIn(!!jwt);
    }, [jwt]);

    async function updateJWT(data) {
        if (typeof (data) == "string") {
            data ? localStorage.setItem(JWT_STORAGE_NAME, data) : localStorage.removeItem(JWT_STORAGE_NAME);
            data ? setJwt(data) : setJwt();
        }
    }

    const login = async (username, password) => {
        const body = { username, password };

        try {
            const response = await makeAnAPICall(ROUTES.LOGIN, METHODS.POST, false, body);
            const data = await handleResponse(response);
            updateJWT(data)
        } catch (error) {
            console.error('Login failed:', error);
            throw new Error('Login failed');
        }
    };

    const register = async (username, password, firstName, lastName, email) => {
        const body = { username, password, firstName, lastName, email };

        try {
            const response = await makeAnAPICall(ROUTES.REGISTER, METHODS.POST, false, body);
            const data = await handleResponse(response);
            updateJWT(data);
        } catch (error) {
            console.error('Registration failed:', error);
            throw new Error('Registration failed');
        }
    };

    const logout = () => {
        setJwt('');
        localStorage.clear();
    };

    return { jwt, loggedIn, login, register, logout };
}

export default useAuth;
