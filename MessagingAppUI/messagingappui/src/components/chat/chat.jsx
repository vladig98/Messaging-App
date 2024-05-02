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

    useEffect(() => {
        fetchChat();
    }, []);

    async function fetchChat() {
        try {
            const result = await invokeSignalR(ROUTES.GETCHATINFO, 'GetChatInfo', 'ReceiveChatInfo', true, { id });
            setChat(result);
            setMessages(result.messages)
            setChatId(result.id)
        } catch (error) {
            console.log("error", error)
        }
    }

    async function sendMessage() {
        try {
            const result = await invokeSignalR(ROUTES.GETCHATINFO, 'SendMessage', 'MessageReceived', true, { "text": message, chatId });
            setMessage('')
            fetchChat()
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