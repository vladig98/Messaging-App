import { domainName, JWT_STORAGE_NAME } from './constants.js';
import { HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

//checks if the connection hasn't been started already
function connectionIsDisconnected(conn) {
    return conn.state == HubConnectionState.Disconnected
}

//check if the event listener is listening for received messages
function isMessageReceived(callback) {
    return callback == "MessageReceived";
}

//invokes the connection and attaches an eventListener to all methods
//requires the connection, the method to invoke, parameters if any (empty object by default), a rerenderFunction if any (empty function by default)
export async function invokeSignalR(conn, invocationFunction, params = {}, rerenderFunction = () => { }) {
    //creates a promise to handle the signalR calls
    const promise = new Promise(async (resolve, reject) => {
        //loops through all listener methods and attaches an eventListener
        for (let callback of conn.callbackFunctions) {
            //attaches the event lisstener
            conn.on(callback, data => {
                //rerenders only if we received a new message
                if (isMessageReceived(callback)) {
                    rerenderFunction(conn)
                }

                //resolving the promise with the signalR data that we return
                resolve(data);
            });
        }

        try {
            //starting the connection only if it's not started already
            if (connectionIsDisconnected(conn)) {
                await conn.start();
            }

            //invoking the signalR method
            await conn.invoke(invocationFunction, params)
        } catch (e) {
            //handling errors and rejecting the promise if an error occurs
            reject(e)
        }
    });

    //awaiting the promise's resolution and returning the result
    const result = await promise;

    return result;
}

//creates the signalR connection
//it requres the URL path, the functions it needs to attach listeners to, and if the calls should be authenticated (false by default)
export function createConnection(hubURL, callbackFunctions, authenticate = false) {
    const jwt = localStorage.getItem(JWT_STORAGE_NAME); //gets the JWT token to authenticate
    const options = authenticate ? { accessTokenFactory: () => jwt } : {}; //creates the connection parameters based on the authenticate property

    //creates the connection
    const conn = new HubConnectionBuilder()
        .withUrl(`${domainName}${hubURL}`, options)
        .build();

    //sets a custom property for the listener methods
    conn.callbackFunctions = callbackFunctions;

    return conn;
}
