import { domainName, JWT_STORAGE_NAME } from './constants.js';
import { HubConnectionBuilder } from '@microsoft/signalr';

export function invokeSignalR(hubURL, invocationFunction, callbackFunction, authenticate = false, params = {}) {
    return new Promise((resolve, reject) => {
        const jwt = localStorage.getItem(JWT_STORAGE_NAME);
        const options = authenticate ? { accessTokenFactory: () => jwt } : {};

        const connection = new HubConnectionBuilder()
            .withUrl(`${domainName}${hubURL}`, options)
            .build();

        connection.on(callbackFunction, data => {
            resolve(data);
        });

        connection.start()
            .then(() => {
                return connection.invoke(invocationFunction, params);
            })
            .catch(error => {
                console.error('Error connecting to hub:', error);
                connection.stop();
                reject(error);
            });
    });
}
