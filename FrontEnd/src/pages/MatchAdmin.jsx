import { Box, Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, FormControl, Grid, InputLabel, MenuItem, Select, TextField, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import LogoutButton from '../components/LogoutButton';
import MainMenu from '../components/MainMenu';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

export default function MatchAdmin() {
    const [matches, setMatches] = useState([]);
    const [newMatch, setNewMatch] = useState({ date: '', tournamentId: '', playerOneId: '', playerTwoId: '', refereeId: '' });
    const [players, setPlayers] = useState([]);
    const [tournaments, setTournaments] = useState([]);
    const [referees, setReferees] = useState([]);
    const [allPlayers, setAllPlayers] = useState([]);
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogContent, setDialogContent] = useState('');
    const [editDialogOpen, setEditDialogOpen] = useState(false);
    const [editMatchDetails, setEditMatchDetails] = useState({ date: '', tournamentId: '', playerOneId: '', playerTwoId: '', refereeId: '' });
    const [handleUpdateRefresh, setHandleUpdateRefresh] = useState(false);
    const [filterTournamentId, setFilterTournamentId] = useState('');
    const [filterPlayerId, setFilterPlayerId] = useState('');
    const [filterRefereeId, setFilterRefereeId] = useState('');

    useEffect(() => {
        const fetchInitialData = async () => {
            try {
                const headers = {
                    'Content-Type': 'application/json',
                    [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                    [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
                };

                const [tournamentsRes, refereesRes, allPlayers, matchesRes] = await Promise.all([
                    fetch(ENDPOINTS.GET_TOURNAMENTS, { method: 'GET', headers }),
                    fetch(ENDPOINTS.GET_REFEREES, { method: 'GET', headers }),
                    fetch(ENDPOINTS.GET_PLAYERS, { method: 'GET', headers }),
                    fetch(ENDPOINTS.GET_ALL_MATCHES, { method: 'GET', headers })
                ]);

                if (tournamentsRes.ok && refereesRes.ok && matchesRes.ok) {
                    const tournamentsData = await tournamentsRes.json();
                    const refereesData = await refereesRes.json();
                    const allPlayersData = await allPlayers.json();
                    const matchesData = await matchesRes.json();
                    setTournaments(tournamentsData);
                    setReferees(refereesData);
                    setAllPlayers(allPlayersData);
                    setMatches(matchesData);
                } else {
                    throw new Error('Failed to fetch initial data');
                }
            } catch (error) {
                setDialogContent(error.message);
                setDialogOpen(true);
            }
        };

        fetchInitialData();
    }, []);

    useEffect(() => {
        fetchMatches();
    }, [filterTournamentId, filterPlayerId, filterRefereeId, handleUpdateRefresh])

    const fetchMatches = async () => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };
            let query = `?`;
            if (filterTournamentId) query += `tournamentId=${filterTournamentId}&`;
            if (filterPlayerId) query += `playerId=${filterPlayerId}&`;
            if (filterRefereeId) query += `refereeId=${filterRefereeId}`;

            const response = await fetch(`${ENDPOINTS.GET_ALL_MATCHES}${query}`, { method: 'GET', headers });
            if (!response.ok) throw new Error('Failed to fetch matches');
            const data = await response.json();
            setMatches(data);
        } catch (error) {
            setDialogContent(error.message);
            setDialogOpen(true);
        }
    };

    const handleDownloadMatches = async (downloadAsCsv) => {
        const queryParams = new URLSearchParams({
            playerId: filterPlayerId || '',
            refereeId: filterRefereeId || '',
            tournamentId: filterTournamentId || '',
            downloadAsCsv
        }).toString();

        const url = `${ENDPOINTS.DOWNLOAD_MATCHES}?${queryParams}`;
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            }
        });

        if (!response.ok) {
            alert('Failed to download file');
            return;
        }

        const blob = await response.blob();
        const downloadUrl = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = downloadUrl;
        link.setAttribute('download', downloadAsCsv ? 'matches.csv' : 'matches.txt');
        document.body.appendChild(link);
        link.click();
        link.parentNode.removeChild(link);
    };

    const fetchPlayersForTournament = async (tournamentId) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };
            const response = await fetch(ENDPOINTS.PLAYERS_IN_TOURNAMENT(tournamentId), { method: 'GET', headers });
            if (!response.ok) throw new Error('Failed to fetch players');
            const data = await response.json();
            setPlayers(data);
        } catch (error) {
            setDialogContent(error.message);
            setDialogOpen(true);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setNewMatch(prev => ({ ...prev, [name]: value }));

        if (name === 'tournamentId') {
            fetchPlayersForTournament(value);
        }
    };

    const handleEditChange = (e) => {
        const { name, value } = e.target;
        setEditMatchDetails(prev => ({ ...prev, [name]: value }));
    };

    const handleAddMatch = async () => {
        const matchData = { ...newMatch };

        console.log(matchData);

        if (matchData.date === '' || matchData.tournamentId === '' || matchData.playerOneId === '' || matchData.playerTwoId === '' || matchData.refereeId === '') {
            setDialogContent("All fields are mandatory.");
            setDialogOpen(true);
            return;
        }

        if (matchData.playerOneId == matchData.playerTwoId) {
            setDialogContent("The players must be different.");
            setDialogOpen(true);
            return;
        }

        console.log(matchData);
        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };
            const response = await fetch(ENDPOINTS.POST_MATCH, {
                method: 'POST',
                headers,
                body: JSON.stringify(matchData),
            });
            if (!response.ok) throw new Error('Failed to add match');
            const addedMatch = await response.json();
            setMatches(prev => [...prev, { ...addedMatch, playerOne: players.find(p => p.id === newMatch.playerOneId), playerTwo: players.find(p => p.id === newMatch.playerTwoId) }]);
            setNewMatch({ date: '', tournamentId: '', playerOneId: '', playerTwoId: '', refereeId: '' });
        } catch (error) {
            setDialogContent(error.message);
            setDialogOpen(true);
        }
    };

    const handleEditClick = (match) => {
        setEditMatchDetails({ ...match, date: new Date(match.date).toISOString().slice(0, 10), refereeId: match.refereeId, id: match.id });
        setEditDialogOpen(true);
    };

    const handleEditMatchSubmit = async () => {
        try {
            if (editMatchDetails.date === '' || editMatchDetails.refereeId === '') {
                setDialogContent("All fields are mandatory.");
                setDialogOpen(true);
                return;
            }
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };

            const response = await fetch(ENDPOINTS.PUT_MATCH, {
                method: 'PUT',
                headers,
                body: JSON.stringify({
                    date: editMatchDetails.date,
                    refereeId: editMatchDetails.refereeId,
                    id: editMatchDetails.id
                }),
            });

            if (!response.ok) throw new Error('Failed to update match');
            setHandleUpdateRefresh(!handleUpdateRefresh);
            setNewMatch({ date: '', tournamentId: '', playerOneId: '', playerTwoId: '', refereeId: '' });
            setEditDialogOpen(false);
        } catch (error) {
            setDialogContent(error.message);
            setDialogOpen(true);
        }
    };

    const handleDeleteMatch = async (id) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
            };
            const response = await fetch(ENDPOINTS.DELETE_MATCH(id), {
                method: 'DELETE',
                headers,
            });
            if (!response.ok) throw new Error('Failed to delete match');
            setMatches(prev => prev.filter(match => match.id !== id));
        } catch (error) {
            setDialogContent(error.message);
            setDialogOpen(true);
        }
    };

    return (
        <>
            <MainMenu />
            <Box mx={40} mt={20}>
                <Typography variant="h4" sx={{ mb: 4 }}>Add New Match</Typography>
                <Grid container spacing={3} alignItems="center">
                    <Grid item xs={12} sm={3}>
                        <TextField
                            fullWidth
                            label="Match Date"
                            type="date"
                            name="date"
                            value={newMatch.date}
                            onChange={handleInputChange}
                            InputLabelProps={{ shrink: true }}
                        />
                    </Grid>
                    <Grid item xs={12} sm={3}>
                        <FormControl fullWidth>
                            <InputLabel>Tournament</InputLabel>
                            <Select
                                value={newMatch.tournamentId}
                                onChange={handleInputChange}
                                name="tournamentId"
                            >
                                {tournaments.map(tournament => (
                                    <MenuItem key={tournament.id} value={tournament.id}>{tournament.name}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={12} sm={2}>
                        <FormControl fullWidth disabled={!newMatch.tournamentId}>
                            <InputLabel>Player 1</InputLabel>
                            <Select
                                value={newMatch.playerOneId}
                                onChange={handleInputChange}
                                name="playerOneId"
                            >
                                {players.map(player => (
                                    <MenuItem key={player.id} value={player.id}>{`${player.firstName} ${player.lastName}`}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={12} sm={2}>
                        <FormControl fullWidth disabled={!newMatch.tournamentId}>
                            <InputLabel>Player 2</InputLabel>
                            <Select
                                value={newMatch.playerTwoId}
                                onChange={handleInputChange}
                                name="playerTwoId"
                            >
                                {players.map(player => (
                                    <MenuItem key={player.id} value={player.id}>{`${player.firstName} ${player.lastName}`}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={12} sm={2}>
                        <FormControl fullWidth>
                            <InputLabel>Referee</InputLabel>
                            <Select
                                value={newMatch.refereeId}
                                onChange={handleInputChange}
                                name="refereeId"
                            >
                                {referees.map(referee => (
                                    <MenuItem key={referee.id} value={referee.id}>{`${referee.firstName} ${referee.lastName}`}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={12}>
                        <Button variant="contained" color="primary" onClick={handleAddMatch}>Insert Match</Button>
                    </Grid>
                </Grid>

                <Typography variant="h4" sx={{ mt: 6, mb: 4 }}>Filter Matches</Typography>
                <Grid container spacing={2} alignItems="center" justifyContent="center">
                    <Grid item xs={10} sm={3}>
                        <FormControl fullWidth>
                            <InputLabel>Tournament Filter</InputLabel>
                            <Select value={filterTournamentId} onChange={e => setFilterTournamentId(e.target.value)} name="filterTournamentId">
                                <MenuItem value="">None</MenuItem>
                                {tournaments.map(tournament => <MenuItem key={tournament.id} value={tournament.id}>{tournament.name}</MenuItem>)}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={10} sm={3}>
                        <FormControl fullWidth>
                            <InputLabel>Player Filter</InputLabel>
                            <Select value={filterPlayerId} onChange={e => { setFilterPlayerId(e.target.value); fetchMatches(); }} name="filterPlayerId">
                                <MenuItem value="">None</MenuItem>
                                {allPlayers.map(player => <MenuItem key={player.id} value={player.id}>{`${player.firstName} ${player.lastName}`}</MenuItem>)}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={10} sm={3}>
                        <FormControl fullWidth>
                            <InputLabel>Referee Filter</InputLabel>
                            <Select value={filterRefereeId} onChange={e => { setFilterRefereeId(e.target.value); fetchMatches(); }} name="filterRefereeId">
                                <MenuItem value="">None</MenuItem>
                                {referees.map(referee => <MenuItem key={referee.id} value={referee.id}>{`${referee.firstName} ${referee.lastName}`}</MenuItem>)}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={10} sm={4}>
                        <Button variant="contained" color="primary" onClick={() => handleDownloadMatches(true)}>Download as CSV</Button>
                        <Button variant="contained" color="secondary" style={{ marginLeft: '20px'}} onClick={() => handleDownloadMatches(false)}>Download as TXT</Button>
                    </Grid>
                </Grid>

                <Typography variant="h4" sx={{ mt: 6, mb: 4 }}>Matches List</Typography>
                <Grid container spacing={2} alignItems="center">
                    <Grid item xs={1} style={{ fontWeight: 700, fontSize: 25 }}>Date</Grid>
                    <Grid item xs={1} style={{ fontWeight: 700, fontSize: 25 }}>Score</Grid>
                    <Grid item xs={2} style={{ fontWeight: 700, fontSize: 25 }}>Tournament</Grid>
                    <Grid item xs={2} style={{ fontWeight: 700, fontSize: 25 }}>Player 1</Grid>
                    <Grid item xs={2} style={{ fontWeight: 700, fontSize: 25 }}>Player 2</Grid>
                    <Grid item xs={1} style={{ fontWeight: 700, fontSize: 25 }}>Referee</Grid>
                </Grid>
                {matches.sort((a, b) => new Date(a.date) - new Date(b.date)).map(match => (
                    <Box key={match.id} sx={{ my: 2, p: 2, boxShadow: 3 }}>
                        <Grid container spacing={2} alignItems="center">
                            <Grid item xs={1}>{new Date(match.date).toLocaleDateString()}</Grid>
                            <Grid item xs={1}>{match.score}</Grid>
                            <Grid item xs={2}>{match.tournament.name}</Grid>
                            <Grid item xs={2}>{`${match.playerOne.firstName} ${match.playerOne.lastName}`}</Grid>
                            <Grid item xs={2}>{`${match.playerTwo.firstName} ${match.playerTwo.lastName}`}</Grid>
                            <Grid item xs={2}>{`${match.referee.firstName} ${match.referee.lastName}`}</Grid>
                            <Grid item xs={2}>
                                <Button variant="contained" color="primary" onClick={() => handleEditClick(match)} style={{ marginRight: 15 }}>Edit</Button>
                                <Button variant="contained" color="secondary" onClick={() => handleDeleteMatch(match.id)}>Delete</Button>
                            </Grid>
                        </Grid>
                    </Box>
                ))}

                <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)}>
                    <DialogTitle>Error</DialogTitle>
                    <DialogContent>
                        <DialogContentText>{dialogContent}</DialogContentText>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDialogOpen(false)}>Close</Button>
                    </DialogActions>
                </Dialog>
                <Dialog open={editDialogOpen} onClose={() => setEditDialogOpen(false)}>
                    <DialogTitle>Edit Match</DialogTitle>
                    <DialogContent>
                        <TextField
                            margin="dense"
                            label="Match Date"
                            type="date"
                            fullWidth
                            name="date"
                            value={editMatchDetails.date}
                            onChange={handleEditChange}
                            InputLabelProps={{ shrink: true }}
                        />
                        <FormControl fullWidth margin="dense">
                            <InputLabel>Referee</InputLabel>
                            <Select
                                value={editMatchDetails.refereeId}
                                onChange={handleEditChange}
                                name="refereeId"
                            >
                                {referees.map(referee => (
                                    <MenuItem key={referee.id} value={referee.id}>{`${referee.firstName} ${referee.lastName}`}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setEditDialogOpen(false)}>Cancel</Button>
                        <Button onClick={handleEditMatchSubmit}>Save</Button>
                    </DialogActions>
                </Dialog>
            </Box>
            <LogoutButton />
        </>
    );
}