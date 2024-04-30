import { useParams } from 'react-router-dom';
import { useState, useEffect } from 'react';
import './chat.css'
import { domainName, ROUTES } from '../../constants.js';
import { HubConnectionBuilder } from '@microsoft/signalr';

function Chat() {
    let { id } = useParams();
    const [message, setMessage] = useState()
    const [username, setUsername] = useState('')

    const connection = new HubConnectionBuilder()
        .withUrl(`${domainName}${ROUTES.GETUSERINFO}`)
        .build();

    useEffect(() => {
        sendMessage()
    }, []);

    async function sendMessage() {
        await connection.start()
            .then(() => console.log('Connected to SignalR hub'))
            .catch(err => console.error('Error connecting to hub:', err));

        await connection.invoke("GetUserInfo", JSON.parse(JSON.stringify({ id })));
    }

    connection.on('ReceiveUserInfo', user => {
        setUsername(user.username)
    });

    return (
        <div className={"container-lg mt-3 w-50"}>
            <div className={"bg-primary text-white p-2 px-3 rounded-top"}>
                You're chatting with {username}
            </div>
            <div className={"border border-primary messageBoxHeight overflow-y-scroll pt-2"}>
                <div className={"d-flex justify-content-start mx-3 m-1"}>
                    <span className={"w-50 bg-secondary text-white p-2 px-3 border messageBox text-start"}>
                        Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.
                    </span>
                </div>
                <div className={"d-flex justify-content-start mx-3 m-1"}>
                    <span className={"w-50 bg-secondary text-white p-2 px-3 border messageBox text-start"}>
                        Message 2
                    </span>
                </div>
                <div className={"d-flex justify-content-end mx-3 m-1"}>
                    <span className={"w-50 bg-primary text-white p-2 px-3 border messageBox text-end"}>
                        Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.
                    </span>
                </div>
                <div className={"d-flex justify-content-end mx-3 m-1"}>
                    <span className={"w-50 bg-primary text-white p-2 px-3 border messageBox text-end"}>
                        Message 2
                    </span>
                </div>
            </div>
            <div className={"d-flex"}>
                <textarea className={"w-100 overflow-y-scroll"} onChange={e => setMessage(e.target.value)}></textarea>
            </div>
            <div>
                <button className={"rounded-bottom bg-primary text-white w-100 p-2"} onClick={sendMessage}>Send</button>
            </div>
        </div>
    )
}

export default Chat