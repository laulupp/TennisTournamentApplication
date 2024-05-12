import React, { useState } from 'react';
import CenteredPanel from '../components/CenteredPanel';
import InputTextField from '../components/InputTextField';
import MainButton from '../components/MainButton';
import { usePage } from '../components/PageProvider';
import '../style/pages/Login.css';
import { ENDPOINTS } from '../utils/Endpoints';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

function Login(props) {
    const { setCurrentPage } = usePage();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');

    const handleLogin = async (event) => {
        event.preventDefault();

        try {
            setErrorMessage('');
            const response = await fetch(ENDPOINTS.LOGIN, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ username, password })
            });

            if (response.ok) {
                const data = await response.json();
                localStorage.setItem(LOCAL_STORAGE_KEYS.USERNAME, data.username);
                localStorage.setItem(LOCAL_STORAGE_KEYS.TOKEN, data.token);
                localStorage.setItem(LOCAL_STORAGE_KEYS.FIRST_NAME, data.firstName);
                localStorage.setItem(LOCAL_STORAGE_KEYS.LAST_NAME, data.lastName);
                localStorage.setItem(LOCAL_STORAGE_KEYS.ROLE, data.role);

                setCurrentPage(2);
            } else {
                const errorData = await response.json();
                console.log(errorData);
                setErrorMessage(errorData.error || 'Please complete all fields');
            }
        } catch (error) {
            setErrorMessage('Network error');
        }
    };

    return (
        <CenteredPanel>
            <h2 className="formTitle">
                Tennis Tournaments Application
            </h2>
            <form noValidate onSubmit={handleLogin}>
                <InputTextField label="Username" value={username} onChange={(e) => setUsername(e.target.value)} />
                <InputTextField label="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                <MainButton type="submit" text="Sign in" />
                {errorMessage && <div className="error-message">{errorMessage}</div>}
                <span onClick={() => setCurrentPage(1)} className="formLink">
                    Don't have an account? Sign Up
                </span>
            </form>
        </CenteredPanel>
    );
}

export default Login;
