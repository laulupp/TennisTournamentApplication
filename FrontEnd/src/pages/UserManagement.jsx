import { Box, Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Grid, TextField, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import LogoutButton from '../components/LogoutButton';
import MainMenu from '../components/MainMenu';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

export default function UserManagement() {
    const roles = ["Player", "Referee", "Admin"];
    const [users, setUsers] = useState([]);
    const [editingUser, setEditingUser] = useState(null);
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogContent, setDialogContent] = useState('');
    const [changePasswordDialogOpen, setChangePasswordDialogOpen] = useState(false);
    const [newPassword, setNewPassword] = useState('');
    const [selectedUserForPasswordChange, setSelectedUserForPasswordChange] = useState(null);


    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const headers = {
                    'Content-Type': 'application/json',
                    [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                    [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
                };

                const response = await fetch(ENDPOINTS.GET_USERS, { method: 'GET', headers });
                if (!response.ok) throw new Error('Failed to fetch users');
                const data = await response.json();
                setUsers(data);
            } catch (error) {
                setDialogContent(error.message);
                setDialogOpen(true);
            }
        };

        fetchUsers();
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

    const handleUpdateUser = async () => {
        if (!editingUser.username || !editingUser.firstName || !editingUser.lastName || !editingUser.email || !editingUser.phoneNumber) {
            setDialogContent("Please complete all fields.");
            setDialogOpen(true);
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
                body: JSON.stringify(editingUser),
            });

            if (!response.ok) throw new Error(parseError(await response.json()));

            setUsers(prevUsers => prevUsers.map(user => user.id === editingUser.id ? editingUser : user));
            setEditingUser(null);
        } catch (error) {
            setDialogContent(error.message || "Failed to update user");
            setDialogOpen(true);
        }
    };

    const handleChangePassword = async () => {
        if (!selectedUserForPasswordChange) return;

        if (!newPassword) {
            setDialogContent("Please complete all fields.");
            setDialogOpen(true);
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
                body: JSON.stringify({ username: selectedUserForPasswordChange.username, newPassword }),
            });

            if (!response.ok) throw new Error('Failed to change password');

            setDialogContent('Password changed successfully.');
            setDialogOpen(true);
        } catch (error) {
            setDialogContent(error.message);
            setDialogOpen(true);
        } finally {
            setChangePasswordDialogOpen(false);
            setSelectedUserForPasswordChange(null);
            setNewPassword('');
        }
    };

    const handleDeleteUser = async (username) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };

            const response = await fetch(ENDPOINTS.DELETE_USER, {
                method: 'DELETE',
                headers,
                body: JSON.stringify(username)
            });

            if (!response.ok) throw new Error('Failed to delete user');
            setUsers(prev => prev.filter(user => user.username !== username));
        } catch (error) {
            setDialogContent(error.message);
            setDialogOpen(true);
        }
    };

    const openChangePasswordDialog = (user) => {
        setSelectedUserForPasswordChange(user);
        setChangePasswordDialogOpen(true);
    };

    const handleEditChange = (e) => {
        const { name, value } = e.target;
        setEditingUser(prev => ({ ...prev, [name]: value }));
    };

    const openEditDialog = (user) => {
        setEditingUser({ ...user });
    };

    return (
        <>
            <MainMenu />
            <Box mx={40} mt={20}>
                <Typography variant="h4" sx={{ mb: 4 }}>Users List</Typography>
                <Grid container spacing={2} alignItems="center">
                    <Grid item xs={1} style={{ fontWeight: 700, fontSize: 25 }}>{"User"}</Grid>
                    <Grid item xs={2} style={{ fontWeight: 700, fontSize: 25 }}>{"First Name"}</Grid>
                    <Grid item xs={2} style={{ fontWeight: 700, fontSize: 25 }}>{"Last Name"}</Grid>
                    <Grid item xs={2} style={{ fontWeight: 700, fontSize: 25 }}>{"Email"}</Grid>
                    <Grid item xs={1} style={{ fontWeight: 700, fontSize: 25 }}>{"Phone"}</Grid>
                    <Grid item xs={1} style={{ fontWeight: 700, fontSize: 25 }}>{"Role"}</Grid>
                </Grid>
                {users.map(user => (
                    <Box key={user.id} sx={{ my: 2, p: 2, boxShadow: 3 }}>
                        <Grid container spacing={2} alignItems="center">
                            <Grid item xs={1}>{user.username}</Grid>
                            <Grid item xs={2}>{user.firstName}</Grid>
                            <Grid item xs={2}>{user.lastName}</Grid>
                            <Grid item xs={2}>{user.email}</Grid>
                            <Grid item xs={1}>{user.phoneNumber}</Grid>
                            <Grid item xs={1}>{roles[user.role]}</Grid>
                            <Grid item xs={3}>
                                <Button variant="contained" color="primary" onClick={() => openEditDialog(user)} style={{ marginRight: 15 }}>Edit</Button>
                                <Button variant="contained" color="secondary" onClick={() => handleDeleteUser(user.username)}>Delete</Button>
                                <Button variant="contained" style={{ height: 50, width:100, marginLeft: 15 }} onClick={() => openChangePasswordDialog(user)}>Change Password</Button>
                            </Grid>
                        </Grid>
                    </Box>
                ))}

                <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)}>
                    <DialogTitle>{dialogContent.includes("success") ? "Success" : "Error"}</DialogTitle>
                    <DialogContent>
                        <DialogContentText>{dialogContent}</DialogContentText>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDialogOpen(false)}>Close</Button>
                    </DialogActions>
                </Dialog>

                <Dialog open={changePasswordDialogOpen} onClose={() => setChangePasswordDialogOpen(false)}>
                    <DialogTitle>Change Password</DialogTitle>
                    <DialogContent>
                        <TextField
                            autoFocus
                            margin="dense"
                            id="newPassword"
                            label="New Password"
                            type="password"
                            fullWidth
                            variant="standard"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                        />
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setChangePasswordDialogOpen(false)}>Cancel</Button>
                        <Button onClick={handleChangePassword}>Change</Button>
                    </DialogActions>
                </Dialog>

                {editingUser && (
                    <Dialog open={Boolean(editingUser)} onClose={() => setEditingUser(null)}>
                        <DialogTitle>Edit User</DialogTitle>
                        <DialogContent>
                            <TextField
                                margin="dense"
                                label="Username"
                                type="text"
                                fullWidth
                                name="username"
                                value={editingUser.username}
                                onChange={handleEditChange}
                                InputProps={{
                                    readOnly: true,
                                }}
                            />
                            <TextField
                                margin="dense"
                                label="First Name"
                                type="text"
                                fullWidth
                                name="firstName"
                                value={editingUser.firstName}
                                onChange={handleEditChange}
                            />
                            <TextField
                                margin="dense"
                                label="Last Name"
                                type="text"
                                fullWidth
                                name="lastName"
                                value={editingUser.lastName}
                                onChange={handleEditChange}
                            />
                            <TextField
                                margin="dense"
                                label="Email"
                                type="email"
                                fullWidth
                                name="email"
                                value={editingUser.email}
                                onChange={handleEditChange}
                            />
                            <TextField
                                margin="dense"
                                label="Phone Number"
                                type="text"
                                fullWidth
                                name="phoneNumber"
                                value={editingUser.phoneNumber}
                                onChange={handleEditChange}
                            />
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={() => setEditingUser(null)}>Cancel</Button>
                            <Button onClick={handleUpdateUser}>Save</Button>
                        </DialogActions>
                    </Dialog>
                )}
            </Box >
            <LogoutButton />
        </>
    );
}