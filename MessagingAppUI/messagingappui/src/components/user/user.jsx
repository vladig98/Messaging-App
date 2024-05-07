import './user.css'
import { Link } from 'react-router-dom';

function User({ user, message, image, id }) {
    return (
        <div className={"card mb-3 mx-auto"} style={{ maxWidth: "540px" }}>
            <div className="row g-0">
                <div className="col-md-4 d-flex align-items-center justify-content-center">
                    <img src={image} className="img-fluid rounded-start" alt="profile pic" />
                </div>
                <div className="col-md-8">
                    <div className="card-body">
                        <h5 className="card-title">{user}</h5>
                        <p className="card-text">{message}</p>
                        <div className={"text-end"}>
                            <Link to={'chat/' + id}>
                                <button className={"btn btn-primary"}>Chat now</button>
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default User;