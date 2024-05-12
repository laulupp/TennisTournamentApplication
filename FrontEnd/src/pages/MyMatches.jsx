import { Button, Card, CardContent, CircularProgress, Container, Dialog, DialogActions, DialogContent, DialogTitle, Grid, Snackbar, TextField, Typography, FormControl, InputLabel, Select, MenuItem, Checkbox, FormControlLabel } from '@mui/material';
import React, { useEffect, useState } from 'react';
import LogoutButton from '../components/LogoutButton';
import MainMenu from '../components/MainMenu';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

export default function MyMatches() {
    const [matches, setMatches] = useState([]);
    const [loading, setLoading] = useState(false);
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [snackbarMessage, setSnackbarMessage] = useState('');
    const [scoreDialogOpen, setScoreDialogOpen] = useState(false);
    const [newScore, setNewScore] = useState('');
    const [selectedMatchId, setSelectedMatchId] = useState(null);
    const [filterDate, setFilterDate] = useState(null);
    const [isScored, setIsScored] = useState(false);
    const [playerOne, setPlayerOne] = useState('');
    const [playerTwo, setPlayerTwo] = useState('');

    useEffect(() => {
        fetchMatches();
    }, [filterDate, isScored, playerOne, playerTwo]);

    const fetchMatches = async () => {
        setLoading(true);
        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };

            let queryParams = new URLSearchParams();
            if (filterDate) queryParams.append('after', filterDate);
            if (isScored) queryParams.append('isScored', isScored);
            if (playerOne) queryParams.append('playerOne', playerOne);
            if (playerTwo) queryParams.append('playerTwo', playerTwo);

            const response = await fetch(`${ENDPOINTS.GET_MATCHES}?${queryParams}`, { method: 'GET', headers });
            if (!response.ok) throw new Error('Failed to fetch matches');

            const data = await response.json();
            setMatches(data);
        } catch (error) {
            console.error(error);
            setSnackbarMessage(error.message || "An error occurred while fetching matches.");
            setSnackbarOpen(true);
        } finally {
            setLoading(false);
        }
    };

    const handleCloseSnackbar = () => setSnackbarOpen(false);

    const handleChangeScore = async () => {
        if (!newScore.match(/^\d+-\d+$/)) {
            setSnackbarMessage("Score must be in the format 'number - number'.");
            setSnackbarOpen(true);
            return;
        }

        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };

            const response = await fetch(ENDPOINTS.PUT_SCORE(selectedMatchId), {
                method: 'PUT',
                headers,
                body: JSON.stringify(newScore),
            });

            if (!response.ok) {
                throw new Error('Failed to update score');
            }

            setMatches(matches.map(match =>
                match.id === selectedMatchId ? { ...match, score: newScore } : match
            ));
            setScoreDialogOpen(false);
            setNewScore('');
            setSnackbarMessage('Score updated successfully.');
            setSnackbarOpen(true);
        } catch (error) {
            console.error(error);
            setSnackbarMessage(error.message || "An error occurred while updating the score.");
            setSnackbarOpen(true);
        }
    };

    return (
        <>
            <MainMenu />
            <Container maxWidth="md" style={{ marginTop: '100px' }}>
                {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) == 1 && (
                    <Grid container spacing={2} alignItems="center" style={{ marginBottom: 20 }} justifyContent="center">
                        <Grid item xs={12} sm={3}>
                            <TextField
                                fullWidth
                                label="Match Date"
                                type="date"
                                name="date"
                                value={filterDate}
                                onChange={e => setFilterDate(e.target.value)}
                                InputLabelProps={{ shrink: true }}
                            />
                        </Grid>
                        <Grid item xs={12} sm={3}>
                            <TextField
                                fullWidth
                                label="Player One"
                                value={playerOne}
                                onChange={(e) => setPlayerOne(e.target.value)}
                            />
                        </Grid>
                        <Grid item xs={12} sm={3}>
                            <TextField
                                fullWidth
                                label="Player Two"
                                value={playerTwo}
                                onChange={(e) => setPlayerTwo(e.target.value)}
                            />
                        </Grid>
                        <Grid item xs={12} sm={3}>
                            <FormControlLabel
                                control={<Checkbox checked={isScored} onChange={(e) => setIsScored(e.target.checked)} />}
                                label="Is Scored"
                            />
                        </Grid>
                    </Grid>
                )}
                <Grid container spacing={2} justifyContent="center">
                    {loading ? (
                        <CircularProgress />
                    ) : (
                        matches.map((match) => (
                            <Grid item xs={12} key={match.id}>
                                <Card raised>
                                    <CardContent>
                                        <Typography variant="h5" component="h2">Match ID: {match.id}</Typography>
                                        <Typography color="textSecondary">Date: {new Date(match.date).toLocaleDateString()}</Typography>
                                        <Typography color="textSecondary">Player One: {localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME) == match.playerOne.username ? "Me" : match.playerOne.firstName + " " + match.playerOne.lastName}</Typography>
                                        <Typography color="textSecondary">Player Two: {localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME) == match.playerTwo.username ? "Me" : match.playerTwo.firstName + " " + match.playerTwo.lastName}</Typography>
                                        <Typography color="textSecondary">Score: {match.score}</Typography>
                                        {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) == 1 && <Button variant="contained" onClick={() => { setSelectedMatchId(match.id); setScoreDialogOpen(true); }}>Change Score</Button>}
                                    </CardContent>
                                </Card>
                            </Grid>
                        ))
                    )}
                </Grid>
                <Dialog open={scoreDialogOpen} onClose={() => setScoreDialogOpen(false)}>
                    <DialogTitle>Change Score</DialogTitle>
                    <DialogContent>
                        <TextField
                            autoFocus
                            margin="dense"
                            id="score"
                            label="Score"
                            type="text"
                            fullWidth
                            value={newScore}
                            onChange={(e) => setNewScore(e.target.value)}
                            placeholder="number - number"
                        />
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setScoreDialogOpen(false)}>Cancel</Button>
                        <Button onClick={handleChangeScore}>Update</Button>
                    </DialogActions>
                </Dialog>
                <Snackbar open={snackbarOpen} autoHideDuration={6000} onClose={handleCloseSnackbar} message={snackbarMessage} />
            </Container>
            <LogoutButton />
        </>
    );
}

