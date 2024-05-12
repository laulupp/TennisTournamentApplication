import { Switch } from '@mui/material';
import React, { useState } from 'react';
import CenteredPanel from '../components/CenteredPanel';
import InputTextField from '../components/InputTextField';
import MainButton from '../components/MainButton';
import { usePage } from '../components/PageProvider';
import '../style/pages/Register.css';
import { ENDPOINTS } from '../utils/Endpoints';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

function Register(props) {
    const { setCurrentPage } = usePage();
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [phoneNumber, setPhoneNumber] = useState('');
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const [isReferee, setIsReferee] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');

    const handleRegister = async (event) => {
        event.preventDefault();
        if (password !== repeatPassword) {
            setErrorMessage("Passwords do not match");
            return;
        }

        try {
            setErrorMessage('');
            const response = await fetch(ENDPOINTS.REGISTER, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ username, email, firstName, lastName, phoneNumber, password, isReferee })
            });

            if (response.ok) {
                const data = await response.json();
                localStorage.setItem(LOCAL_STORAGE_KEYS.USERNAME, data.username);
                localStorage.setItem(LOCAL_STORAGE_KEYS.TOKEN, data.token);
                localStorage.setItem(LOCAL_STORAGE_KEYS.FIRST_NAME, data.firstName);
                localStorage.setItem(LOCAL_STORAGE_KEYS.LAST_NAME, data.lastName);

                setCurrentPage(2);
            } else {
                const errorData = await response.json();
                if (errorData.errors) {
                    let errors = Object.entries(errorData.errors).map(([key, messages]) => {
                        if (key === "PhoneNumber") return "The Phone Number has an invalid format.";
                        return messages.join('\n');
                    });
                    setErrorMessage(errors.join('\n'));
                } else {
                    setErrorMessage(errorData.error || 'Please complete all fields');
                }
            }
        } catch (error) {
            setErrorMessage('Network error');
        }
    };

    return (
        <CenteredPanel containerHeight="900">
            <h2 className="formTitle">
                Register
            </h2>
            <form noValidate onSubmit={handleRegister}>
                <InputTextField label="Username" value={username} onChange={(e) => setUsername(e.target.value)} />
                <InputTextField label="Email Address" value={email} onChange={(e) => setEmail(e.target.value)} />
                <InputTextField label="First Name" value={firstName} onChange={(e) => setFirstName(e.target.value)} />
                <InputTextField label="Last Name" value={lastName} onChange={(e) => setLastName(e.target.value)} />
                <InputTextField label="Phone Number" value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} />
                <InputTextField label="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                <InputTextField label="Repeat Password" type="password" value={repeatPassword} onChange={(e) => setRepeatPassword(e.target.value)} />
                <div className="centered-container">
                    <Switch checked={isReferee} onChange={(e) => setIsReferee(e.target.checked)} />
                    <span>{isReferee ? 'Referee' : 'Player'}</span>
                </div>
                <MainButton type="submit" text="Sign up" />
                {errorMessage && <div className="error-message" style={{ whiteSpace: "pre-line" }}>{errorMessage}</div>}
                <span onClick={() => setCurrentPage(0)} className="formLink">
                    Already have an account? Sign in
                </span>
            </form>
        </CenteredPanel>
    );
}

export default Register;
