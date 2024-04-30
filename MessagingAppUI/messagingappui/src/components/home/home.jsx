import Navbar from '../../components/navbar/navbar.jsx';
import User from '../user/user.jsx';

function Home({ loggedIn, users }) {
    return (
        <>
            <Navbar
                loggedIn={loggedIn}
            />
            <h1 className={"mx-auto text-center mt-5 mb-5"}>Messaging App</h1>
            {loggedIn && (
                <>
                    {users.map((user, index) => (
                        <User key={index} user={user.username} message={user.message} image={user.image} id={user.id} />
                    ))}
                </>
            )}
        </>
    );
}

export default Home;