import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import LogoutButton from '../components/LogoutButton';
import MainMenu from '../components/MainMenu';
import { usePage } from '../components/PageProvider';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS, clearLocalStorage } from '../utils/LocalStorageKeys';

function UserSettings() {
    const { setCurrentPage } = usePage();

    const [user, setUser] = useState({
        firstName: '',
        lastName: '',
        email: '',
        phoneNumber: '',
        password: ''
    });
    const [dialogContent, setDialogContent] = useState('');
    const [errorDialogOpen, setErrorDialogOpen] = useState(false);
    const [changePasswordDialogOpen, setChangePasswordDialogOpen] = useState(false);
    const [changePassword, setChangePassword] = useState({
        oldPassword: '',
        newPassword: ''
    });

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const headers = {
                    'Content-Type': 'application/json',
                    [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                    [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
                };
                const response = await fetch(ENDPOINTS.GET_PERSONAL_INFO, { method: 'GET', headers });
                if (!response.ok) {
                    const errorResponse = await response.json();
                    throw new Error(parseError(errorResponse));
                }
                const data = await response.json();
                setUser({ ...data, password: '' });
            } catch (error) {
                showErrorDialog(error.message);
            }
        };

        fetchUserData();
    }, []);

    const parseError = (errorResponse) => {
        if (errorResponse.error) {
            return errorResponse.error;
        } else if (errorResponse.title && errorResponse.errors) {
            let errorMessage = errorResponse.title;
            Object.keys(errorResponse.errors).forEach((field) => {
                errorMessage += ` ${field}: ${errorResponse.errors[field].join("\n")}`;
            });
            return errorMessage;
        }
        return "An unexpected error occurred.";
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setUser(prev => ({ ...prev, [name]: value }));
    };

    const handlePasswordChange = (e) => {
        const { name, value } = e.target;
        setChangePassword(prev => ({ ...prev, [name]: value }));
    };

    const showErrorDialog = (message) => {
        setDialogContent(message);
        setErrorDialogOpen(true);
    };

    const updateUser = async () => {
        if (!user.firstName || !user.lastName || !user.email || !user.phoneNumber) {
            showErrorDialog("Please complete all fields.");
            return;
        }

        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };

            const response = await fetch(ENDPOINTS.PUT_USER, {
                method: 'PUT',
                headers,
                body: JSON.stringify(user),
            });

            if (!response.ok) {
                const errorResponse = await response.json();
                throw new Error(parseError(errorResponse));
            }

            localStorage.setItem(LOCAL_STORAGE_KEYS.FIRST_NAME, user.firstName);
            localStorage.setItem(LOCAL_STORAGE_KEYS.LAST_NAME, user.lastName);

            setUser({ ...user, password: '' })
            showErrorDialog("User updated successfully.");
        } catch (error) {
            showErrorDialog(error.message);
        }
    };

    const changeUserPassword = async () => {
        if (!changePassword.oldPassword || !changePassword.newPassword) {
            showErrorDialog("Please complete all fields.");
            return;
        }

        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };

            const response = await fetch(ENDPOINTS.CHANGE_PASSWORD, {
                method: 'PUT',
                headers,
                body: JSON.stringify(changePassword),
            });

            setChangePassword({ oldPassword: '', newPassword: '' });

            if (!response.ok) {
                const errorResponse = await response.json();
                throw new Error(parseError(errorResponse));
            }

            showErrorDialog("Password changed successfully.");
        } catch (error) {
            showErrorDialog(error.message);
        } finally {
            setChangePasswordDialogOpen(false);
        }
    };

    const deleteUserAccount = async () => {
        const confirmDeletion = window.confirm("Are you sure you want to delete your account? This action cannot be undone.");
        if (!confirmDeletion) {
            return;
        }

        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };

            const response = await fetch(ENDPOINTS.DELETE_USER, {
                method: 'DELETE',
                headers,
            });

            if (!response.ok) {
                const errorResponse = await response.json();
                throw new Error(parseError(errorResponse));
            }

            clearLocalStorage();
            setCurrentPage(0);
        } catch (error) {
            showErrorDialog(error.message);
        }
    };

    return (
        <>
            <MainMenu />
            <Box sx={{ '& > :not(style)': { m: 1 }, maxWidth: '500px', margin: 'auto', marginTop: '200px' }}>
                <Typography variant="h6" style={{ fontWeight: 700, fontSize: 30, textAlign: 'center' }}>User Settings</Typography>
                <TextField fullWidth label="First Name" name="firstName" value={user.firstName} onChange={handleInputChange} />
                <TextField fullWidth label="Last Name" name="lastName" value={user.lastName} onChange={handleInputChange} />
                <TextField fullWidth label="Email" name="email" value={user.email} onChange={handleInputChange} />
                <TextField fullWidth label="Phone Number" name="phoneNumber" value={user.phoneNumber} onChange={handleInputChange} />
                <TextField fullWidth label="Confirm Password" name="password" type="password" value={user.password} onChange={handleInputChange} />

                <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 2 }}>
                    <Button variant="contained" style={{ marginLeft: '15px' }} onClick={updateUser}>Update Information</Button>
                    <Button variant="contained" style={{ marginLeft: '15px' }} onClick={() => setChangePasswordDialogOpen(true)}>Change Password</Button>
                    <Button variant="contained" style={{ marginLeft: '15px' }} color="error" onClick={deleteUserAccount}>Delete My Account</Button>
                </Box>

                <Dialog open={errorDialogOpen} onClose={() => setErrorDialogOpen(false)}>
                    <DialogTitle>{dialogContent.includes("successfully") ? "Success" : "Error"}</DialogTitle>
                    <DialogContent>
                        {dialogContent}
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setErrorDialogOpen(false)}>Close</Button>
                    </DialogActions>
                </Dialog>

                <Dialog open={changePasswordDialogOpen} onClose={() => setChangePasswordDialogOpen(false)}>
                    <DialogTitle>Change Password</DialogTitle>
                    <DialogContent>
                        <TextField
                            autoFocus
                            margin="dense"
                            name="oldPassword"
                            label="Old Password"
                            type="password"
                            fullWidth
                            value={changePassword.oldPassword}
                            onChange={handlePasswordChange}
                        />
                        <TextField
                            margin="dense"
                            name="newPassword"
                            label="New Password"
                            type="password"
                            fullWidth
                            value={changePassword.newPassword}
                            onChange={handlePasswordChange}
                        />
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setChangePasswordDialogOpen(false)}>Cancel</Button>
                        <Button onClick={changeUserPassword}>Change</Button>
                    </DialogActions>
                </Dialog>
            </Box>
            <LogoutButton />
        </>
    );
}

export default UserSettings;
