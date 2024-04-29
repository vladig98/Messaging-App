import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function Logout({ clear }) {
    const navigate = useNavigate();

    useEffect(() => {
        clear(true)
        localStorage.clear();
        navigate("/", { replace: true });
    }, []);

    return (<div></div>);
}

export default Logout;
