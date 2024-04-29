import { JWT_STORAGE_NAME, UsersStorageName, domainName, METHODS, ROUTES } from '../constants.js';

function useAPI() {
    async function makeAnAPICall(url, method, authorize = false, body) {
        let options = {
            method: method,
            mode: 'cors',
            credentials: 'include',
        };

        options.headers = new Headers()

        if (authorize) {
            const jwt = localStorage.getItem('JWTAuthToken');
            options.headers = new Headers({ Authorization: `Bearer ${jwt}` });
        }

        if (body) {
            options.body = JSON.stringify(body);
            options.headers.set('Content-Type', 'application/json');
        }

        try {
            const response = await fetch(`${domainName}${url}`, options);
            return response;
        } catch (error) {
            console.error('API call failed:', error);
            throw new Error('API call failed');
        }
    }

    async function handleResponse(response) {
        if (!response.ok) {
            throw new Error(`API call failed with status ${response.status}`);
        }

        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            return await response.json();
        }

        return await response.text();
    }

    return { makeAnAPICall, handleResponse };
}

export default useAPI;
