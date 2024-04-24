import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function Logout({ setJwt, setUsers }) {
    const navigate = useNavigate();

    useEffect(() => {
        setJwt();
        setUsers([])
        localStorage.clear();
        navigate("/", { replace: true });
    }, []);

    return (<div></div>);
}

export default Logout;
