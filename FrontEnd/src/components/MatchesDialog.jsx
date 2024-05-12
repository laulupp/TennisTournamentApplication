import { Box, CircularProgress, Dialog, DialogContent, DialogTitle, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

function MatchesDialog({ open, onClose, tournamentId }) {
    const [matches, setMatches] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!open) return;

        const fetchMatches = async () => {
            setLoading(true);
            try {
                const response = await fetch(`${ENDPOINTS.GET_ALL_MATCHES}?tournamentId=${tournamentId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
                        [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
                    },
                });
                if (response.ok) {
                    const data = await response.json();
                    setMatches(data);
                } else {
                    throw new Error('Failed to fetch matches');
                }
            } catch (error) {
                console.error(error.message);
            } finally {
                setLoading(false);
            }
        };

        fetchMatches();
    }, [open, tournamentId]);

    return (
        <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
            <DialogTitle>Matches</DialogTitle>
            <DialogContent>
                {loading ? (
                    <CircularProgress />
                ) : (
                    matches.map(match => (
                        <Box key={match.id} my={2} p={2} boxShadow={3}>
                            <Typography variant="body1">
                                Date: {new Date(match.date).toLocaleString()}
                            </Typography>
                            <Typography variant="body1">
                                <strong>Player One:</strong> {match.playerOne.firstName} {match.playerOne.lastName}
                            </Typography>
                            <Typography variant="body1">
                                <strong>Player Two:</strong> {match.playerTwo.firstName} {match.playerTwo.lastName}
                            </Typography>
                            <Typography variant="body1">
                                <strong>Referee:</strong> {match.referee.firstName} {match.referee.lastName}
                            </Typography>     
                            <Typography style={{textAlign:'center'}}variant="h5">
                                Score: {match.score}
                            </Typography>                       
                        </Box>
                    ))
                )}
            </DialogContent>
        </Dialog>
    );
}

export default MatchesDialog;
