import { useParams } from 'react-router-dom';
import './chat.css'
import { invokeSignalR } from '../../invokeSignalR';
import { useState, useEffect } from 'react';
import { ROUTES } from '../../constants.js';

function Chat() {
    let { id } = useParams();
    const [message, setMessage] = useState('')
    const [chat, setChat] = useState(null)
    const [messages, setMessages] = useState([])
    const [chatId, setChatId] = useState('')
    const [connection, setConnection] = useState('')

    useEffect(() => {
        fetchChat();
    }, []);


    async function fetchChat() {
        try {
            let cnn = connection ? connection : null
            const { conn, promise } = await invokeSignalR(ROUTES.GETCHATINFO, 'GetChatInfo', 'ReceiveChatInfo', true, { id }, cnn);
            const data = await Promise.resolve(promise);
            setChat(data);
            setConnection(conn)
            setMessages(data.messages)
            setChatId(data.id)
        } catch (error) {
            console.log("error", error)
        }
    }

    useEffect(() => {
        if (connection) {
            connection.on("MessageReceived", data => {
                fetchChat()
            })
        }
    }, [connection])

    async function sendMessage() {
        try {
            let cnn = connection ? connection : null
            const { conn, promise } = await invokeSignalR(ROUTES.GETCHATINFO, 'SendMessage', 'MessageReceived', true, { "text": message, chatId }, cnn);
            const data = await Promise.resolve(promise);
            setConnection(conn)
            setMessage('')
            await fetchChat()
        } catch (error) {
            console.log("error", error)
        }
    }

    return (
        <div className={"container-lg mt-3 w-50"}>
            <div className={"bg-primary text-white p-2 px-3 rounded-top"}>
                {chat && (
                    <>
                        <img className={"chatHeaderImage"} src={chat.receiver.image} />You're chatting with {chat.receiver.username}
                    </>
                )}
            </div>
            <div className={"border border-primary messageBoxHeight overflow-y-scroll pt-2 d-flex flex-column-reverse"}>
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
    )
}

export default Chat