import { JWT_STORAGE_NAME, UsersStorageName, domainName, METHODS, ROUTES } from '../constants.js';

function useAPI() {
    //makes an API call
    //requres the URL, the method for the call (GET, POST, PUT, etc.), if it's authenticated, and the body if we're sending data
    async function makeAnAPICall(url, method, authorize = false, body) {
        //creates the API call options
        let options = {
            method: method,
            mode: 'cors',
            credentials: 'include',
        };

        options.headers = new Headers()

        //creates the authenticcation header if we are making an authenticated call
        if (authorize) {
            const jwt = localStorage.getItem('JWTAuthToken');
            options.headers = new Headers({ Authorization: `Bearer ${jwt}` });
        }

        //adds the body to the call if we're sending a body
        if (body) {
            options.body = JSON.stringify(body);
            options.headers.set('Content-Type', 'application/json');
        }

        //tries to make the call and catches errors
        try {
            const response = await fetch(`${domainName}${url}`, options);
            return response;
        } catch (error) {
            console.error('API call failed:', error);
            throw new Error('API call failed');
        }
    }

    //handles the API response
    async function handleResponse(response) {
        if (!response.ok) {
            throw new Error(`API call failed with status ${response.status}`);
        }

        //gets the content type header from the response
        //returns a text or a JSON value based on what we've received from the server
        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            return await response.json();
        }

        return await response.text();
    }

    return { makeAnAPICall, handleResponse };
}

export default useAPI;
