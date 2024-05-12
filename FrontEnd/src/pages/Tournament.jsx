import { Button, Card, CardContent, CircularProgress, Container, Grid, Snackbar, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import LogoutButton from '../components/LogoutButton';
import MainMenu from '../components/MainMenu';
import MatchesDialog from '../components/MatchesDialog';
import '../style/pages/Tournament.css';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

export default function Tournament() {
    const [tournaments, setTournaments] = useState([]);
    const [loading, setLoading] = useState(false);
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [snackbarMessage, setSnackbarMessage] = useState('');
    const [matchesDialogOpen, setMatchesDialogOpen] = useState(false);
    const [selectedTournamentId, setSelectedTournamentId] = useState(null);

    useEffect(() => {
        const fetchTournaments = async () => {
            setLoading(true);
            try {
                const response = await fetch(ENDPOINTS.GET_TOURNAMENTS, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                        [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
                    },
                });
                if (response.ok) {
                    const data = await response.json();
                    setTournaments(data);
                } else {
                    throw new Error('Failed to fetch tournaments');
                }
            } catch (error) {
                console.error(error);
                setSnackbarMessage(error.message || "An error occurred while fetching tournaments.");
                setSnackbarOpen(true);
            } finally {
                setLoading(false);
            }
        };

        fetchTournaments();
    }, []);

    const handleViewMatches = (tournamentId) => {
        setSelectedTournamentId(tournamentId);
        setMatchesDialogOpen(true);
    };

    const handleEnrollment = async (tournamentId) => {
        try {
            const response = await fetch(ENDPOINTS.ENROLL_IN_TOURNAMENT(tournamentId), {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                    [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
                },
            });
            if (response.ok) {
                setTournaments(tournaments.map(tournament =>
                    tournament.id === tournamentId ? { ...tournament, status: 1 } : tournament
                ));
            } else {
                throw new Error('Failed to enroll in tournament');
            }
        } catch (error) {
            console.error(error);
            setSnackbarMessage(error.message || "An error occurred while enrolling in the tournament.");
            setSnackbarOpen(true);
        }
    };

    const handleCloseSnackbar = () => setSnackbarOpen(false);

    return (
        <>
            <MainMenu />
            <Container maxWidth="lg" style={{ marginTop: '100px' }}>
                <Grid container spacing={0} justifyContent="center">
                    {loading ? (
                        <CircularProgress />
                    ) : (
                        tournaments.map((tournament) => (
                            <Grid item key={tournament.id} style={{ marginBottom: 20 }}>
                                <Card className="tournament-card" raised>
                                    <CardContent className="tournament-card-content">
                                        <Typography variant="h3" component="h2" style={{ color: 'var(--color-purple)', textAlign: 'center' }}>{tournament.name}</Typography>
                                        <div style={{ display: 'container', width: 400, height: 50, margin: 'auto' }}>
                                            <Typography style={{ float: 'left' }} color="textSecondary">Start: {new Date(tournament.startDate).toLocaleDateString()}</Typography>
                                            <Typography style={{ float: 'right' }} color="textSecondary">End: {new Date(tournament.endDate).toLocaleDateString()}</Typography>
                                        </div>
                                        {tournament.status === 0 && (
                                            <Button
                                                variant="contained"
                                                style={{ backgroundColor: 'var(--color-purple)', color: 'white', marginRight: 10, float: 'right' }}
                                                onClick={() => handleEnrollment(tournament.id)}
                                            >
                                                Enroll
                                            </Button>
                                        )}
                                        <Button
                                            variant="outlined"
                                            style={{ color: 'var(--color-purple)', borderColor: 'var(--color-purple)', marginLeft: 10 }}
                                            onClick={() => handleViewMatches(tournament.id)}
                                        >
                                            View Matches
                                        </Button>
                                        {tournament.status === 1 && (
                                            <p style={{ color: 'var(--color-orange)', fontWeight: 700, float: 'right', marginRight: 20 }}>Pending</p>
                                        )}
                                        {tournament.status === 2 && (
                                            <p style={{ color: 'var(--color-green)', fontWeight: 700, float: 'right', marginRight: 20 }}>Enrolled</p>
                                        )}
                                    </CardContent>
                                </Card>
                            </Grid>
                        ))
                    )}
                </Grid>
                <LogoutButton />
            </Container>
            <Snackbar open={snackbarOpen} autoHideDuration={6000} onClose={handleCloseSnackbar} message={snackbarMessage} />
            <MatchesDialog
                open={matchesDialogOpen}
                onClose={() => setMatchesDialogOpen(false)}
                tournamentId={selectedTournamentId}
            />
        </>
    );
}
