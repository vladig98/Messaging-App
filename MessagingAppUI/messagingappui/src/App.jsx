import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [forecasts, setForecasts] = useState();

    const [userName, setUserName] = useState();
    const [password, setPassword] = useState();
    const [firstName, setFirstName] = useState();
    const [lastName, setLastName] = useState();
    const [email, setEmail] = useState();

    useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tabelLabel">
            <thead>
                <tr>
                    <th>Chat Id</th>
                    <th>Id</th>
                    <th>User Id</th>
                    <th>Text</th>
                </tr>
            </thead>
            <tbody>
                <tr key={forecasts.id}>
                    <td>{forecasts.chatId}</td>
                    <td>{forecasts.id}</td>
                    <td>{forecasts.userId}</td>
                    <td>{forecasts.text}</td>
                </tr>
            </tbody>
        </table>;

    async function login(e) {
        e.preventDefault();
        const response = await fetch('https://localhost:7238/register', {
            method: "POST",
            mode: 'cors',
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(
                {
                    username: userName,
                    password: password,
                    firstName: firstName,
                    lastName: lastName,
                    email: email
                }
            )
        });
        const data = await response.text();
        console.log(data);
    }

    return (
        <div>
            <form onSubmit={login}>
                <div>
                    <span>First name:</span>
                    <input type="text" onChange={e => setFirstName(e.target.value)} />
                </div>
                <div>
                    <span>Last name:</span>
                    <input type="text" onChange={e => setLastName(e.target.value)} />
                </div>
                <div>
                    <span>Login name:</span>
                    <input type="text" onChange={e => setUserName(e.target.value)} />
                </div>
                <div>
                    <span>Password</span>
                    <input type="password" onChange={e => setPassword(e.target.value)} />
                </div>
                <div>
                    <span>Email</span>
                    <input type="email" onChange={e => setEmail(e.target.value)} />
                </div>
                <button type="submit"></button>
            </form>
        </div>
    );
    
    async function populateWeatherData() {
        let myHeaders = new Headers({
            "Content-Type": "application/json",
        });
        const response = await fetch('https://localhost:7238/Messaging/2', myHeaders);
        console.log(response)
        const data = await response.json();
        console.log(data)
        setForecasts(data);
    }
}

export default App;