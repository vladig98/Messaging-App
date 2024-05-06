import './chat.css'
import { useParams } from 'react-router-dom';
import { invokeSignalR, createConnection } from '../../invokeSignalR';
import { useState, useEffect } from 'react';
import { ROUTES } from '../../constants.js';
import Navbar from '../../components/navbar/navbar.jsx';

function Chat({ loggedIn }) {
    const { id } = useParams();
    const [message, setMessage] = useState('')
    const [chat, setChat] = useState(null)
    const [messages, setMessages] = useState([])
    const [chatId, setChatId] = useState('')
    const [connection, setConnection] = useState(null) //store the connection to have only one WS

    //get the chat info on page load and create a signalR connection for future updates
    useEffect(() => {
        const conn = createSignalRConnection()
        fetchChat(conn);
    }, []);

    //creates a signalR connection
    function createSignalRConnection() {
        //create a connection to signalR. It requres a URL path, a method to listen to and if it's authencticated (false by default)
        const conn = createConnection(ROUTES.GETCHATINFO, ['ReceiveChatInfo', 'MessageReceived'], true);

        setConnection(conn)
        return conn
    }

    //gets chat information
    async function fetchChat(c) {
        try {
            let cnn = c || connection
            //invokes signalR and gets a result from a Promise. 
            //It requires a connection, caller function to invoke, params to invoke(optional) and a function to rerender conetent(optional)
            const result = await invokeSignalR(cnn, 'GetChatInfo', { id }, fetchChat);

            //stores chat information
            setChat(result);
            setMessages(result.messages)
            setChatId(result.id)
        } catch (error) {
            console.log("error", error)
        }
    }

    //sends a message to the server
    async function sendMessage() {
        try {
            //invokes signalR and sends a message to the server
            //It requires a connection, caller function to invoke, params to invoke(optional) and a function to rerender conetent(optional)
            await invokeSignalR(connection, 'SendMessage', { "text": message, chatId }, fetchChat);

            //clears the input
            setMessage('')
        } catch (error) {
            console.log("error", error)
        }
    }

    return (
        <>
            <Navbar loggedIn={loggedIn}>
            </Navbar>
            <div className={"container-lg mt-3 w-50"}>
                <div className={"bg-primary text-white p-2 px-3 rounded-top"}>
                    {/*display the name and the profile picture of the person I'm cahtting with if the chat data exists*/}
                    {chat && (
                        <>
                            <img className={"chatHeaderImage"} src={chat.receiver.image} />You're chatting with {chat.receiver.username}
                        </>
                    )}
                </div>
                <div className={"border border-primary messageBoxHeight overflow-y-scroll pt-2 d-flex flex-column-reverse"}>
                    {/*displays the chat messages differently for each user. It reverses the messages so that we have the scroller pinned to the bottom
                        to create that real messaging feeling */}
                    {messages && chat && [...messages].reverse().map((m, index) => {
                        if (m.userId == chat.receiverId) {
                            return (
                                <div key={index} className={"d-flex justify-content-start mx-3 m-1"}>
                                    <span className={"w-50 d-flex flex justify-content-start"}>
                                        <span className={"bg-secondary text-white p-2 px-3 border messageBox text-start"}>
                                            {m.text}
                                        </span>
                                    </span>
                                </div>
                            )
                        } else {
                            return (
                                <div key={index} className={"d-flex justify-content-end mx-3 m-1"}>
                                    <span className={"w-50 d-flex justify-content-end"}>
                                        <span className={"bg-primary text-white p-2 px-3 border messageBox text-end"}>
                                            {m.text}
                                        </span>
                                    </span>
                                </div>
                            )
                        }
                    })}
                </div>
                <div className={"d-flex"}>
                    <textarea className={"w-100 overflow-y-scroll"} value={message} onChange={e => setMessage(e.target.value)}></textarea>
                </div>
                <div>
                    <button className={"rounded-bottom bg-primary text-white w-100 p-2"} onClick={sendMessage}>Send</button>
                </div>
                </div>
        </>
    )
}

export default Chat