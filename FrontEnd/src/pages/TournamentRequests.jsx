import { Button, Card, CardContent, CircularProgress, Container, Grid, Snackbar, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import LogoutButton from '../components/LogoutButton';
import MainMenu from '../components/MainMenu';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

export default function TournamentRequests() {
    const [requests, setRequests] = useState([]);
    const [loading, setLoading] = useState(false);
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [snackbarMessage, setSnackbarMessage] = useState('');

    useEffect(() => {
        const fetchRequests = async () => {
            setLoading(true);
            try {
                const headers = {
                    'Content-Type': 'application/json',
                    [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                    [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
                };

                const response = await fetch(ENDPOINTS.GET_PENDING_REQUESTS, { method: 'GET', headers });
                if (response.ok) {
                    const data = await response.json();
                    setRequests(data);
                } else {
                    throw new Error('Failed to fetch requests');
                }
            } catch (error) {
                console.error(error);
                setSnackbarMessage(error.message || "An error occurred while fetching requests.");
                setSnackbarOpen(true);
            } finally {
                setLoading(false);
            }
        };

        fetchRequests();
    }, []);

    const handleRequest = async (userId, tournamentId, action) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };

            const endpoint = action === 'approve' ? ENDPOINTS.APPROVE_REQUEST : ENDPOINTS.DENY_REQUEST;
            const response = await fetch(`${endpoint}?userId=${userId}&tournamentId=${tournamentId}`, {
                method: 'POST',
                headers,
            });

            if (!response.ok) {
                throw new Error(`Failed to ${action} request`);
            }

            setRequests(requests.filter(request => request.userId !== userId || request.tournamentId !== tournamentId));
            setSnackbarMessage(`Request ${action}ed successfully.`);
            setSnackbarOpen(true);
        } catch (error) {
            console.error(error);
            setSnackbarMessage(error.message || `An error occurred while ${action}ing the request.`);
            setSnackbarOpen(true);
        }
    };

    const handleCloseSnackbar = () => setSnackbarOpen(false);

    return (
        <>
            <MainMenu />
            <Container maxWidth="md" style={{ marginTop: '100px' }}>
                <Grid container spacing={2} justifyContent="center">
                    {loading ? (
                        <CircularProgress />
                    ) : (
                        requests.map((request) => (
                            <Grid item xs={12} key={`${request.userId}-${request.tournamentId}`}>
                                <Card raised>
                                    <CardContent>
                                        <Typography variant="h6" component="h2">{request.tournament.name}</Typography>
                                        <Typography color="textSecondary">{`${request.user.firstName} ${request.user.lastName} - ${request.user.email}`}</Typography>
                                        <Button style={{ float: 'right', margin: 10, marginBottom: 20 }} variant="contained" color="primary" onClick={() => handleRequest(request.userId, request.tournamentId, 'approve')}>Approve</Button>
                                        <Button style={{ float: 'right', margin: 10, marginBottom: 20 }} variant="contained" color="secondary" onClick={() => handleRequest(request.userId, request.tournamentId, 'deny')}>Deny</Button>
                                    </CardContent>
                                </Card>
                            </Grid>
                        ))
                    )}
                </Grid>
                <Snackbar open={snackbarOpen} autoHideDuration={6000} onClose={handleCloseSnackbar} message={snackbarMessage} />
            </Container>
            <LogoutButton />
        </>
    );
}
