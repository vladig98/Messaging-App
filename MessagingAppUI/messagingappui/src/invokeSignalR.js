import { domainName, JWT_STORAGE_NAME } from './constants.js';
import { HubConnectionBuilder } from '@microsoft/signalr';

export async function invokeSignalR(hubURL, invocationFunction, callbackFunction, authenticate = false, params = {}, existingConnection = null) {
    const jwt = localStorage.getItem(JWT_STORAGE_NAME);
    const options = authenticate ? { accessTokenFactory: () => jwt } : {};

    const conn = existingConnection ? existingConnection : new HubConnectionBuilder()
        .withUrl(`${domainName}${hubURL}`, options)
        .build();

    const promise = new Promise((resolve, reject) => {
        conn.on(callbackFunction, data => {
            resolve(data);
        });

        if (conn.state == 'Disconnected') {
            conn.start()
                .then(() => {
                    conn.invoke(invocationFunction, params)
                        .then(result => {
                            resolve(result);
                        })
                        .catch(error => {
                            console.error('Error invoking SignalR function:', error);
                            reject(error);
                        });
                })
                .catch(error => {
                    console.error('Error starting SignalR connection:', error);
                    reject(error);
                });
        } else {
            conn.invoke(invocationFunction, params)
                .then(result => {
                    resolve(result);
                })
                .catch(error => {
                    console.error('Error invoking SignalR function:', error);
                    reject(error);
                });
        }
    });

    return { conn, promise };
}
