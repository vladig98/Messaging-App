function Error({ message }) {
    return (
        <div className={"mx-auto w-50 text-danger"}>
            <span>{message}</span>
        </div>
    );
}

export default Error;