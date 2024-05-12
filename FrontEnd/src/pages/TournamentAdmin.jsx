import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Grid, TextField } from '@mui/material';
import React, { useEffect, useState } from 'react';
import LogoutButton from '../components/LogoutButton';
import MainMenu from '../components/MainMenu';
import '../style/pages/TournamentAdmin.css';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';

const TournamentAdmin = () => {
  const [tournaments, setTournaments] = useState([]);
  const [newTournament, setNewTournament] = useState({ name: '', startDate: '', endDate: '' });
  const [editingTournament, setEditingTournament] = useState(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogContent, setDialogContent] = useState('');

  useEffect(() => {
    const fetchTournaments = async () => {
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
        setDialogContent(error.message);
        setDialogOpen(true);
      }
    };

    fetchTournaments();
  }, []);

  const handleInputChange = (e, setter) => {
    const { name, value } = e.target;
    setter(prev => ({ ...prev, [name]: value }));
  };

  const handleAddTournament = async () => {
    try {
      const response = await fetch(ENDPOINTS.POST_TOURNAMENT, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
          [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
        },
        body: JSON.stringify(newTournament),
      });

      if (response.ok) {
        const addedTournament = await response.json();
        setTournaments([...tournaments, addedTournament]);
        setNewTournament({ name: '', startDate: '', endDate: '' });
      } else {
        throw new Error('Failed to add tournament');
      }
    } catch (error) {
      setDialogContent(error.message);
      setDialogOpen(true);
    }
  };

  const handleUpdateTournament = async () => {
    try {
      const response = await fetch(ENDPOINTS.PUT_TOURNAMENT, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
          [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
        },
        body: JSON.stringify(editingTournament),
      });

      if (response.ok) {
        const updatedTournament = editingTournament;
        setTournaments(tournaments.map(t => t.id === updatedTournament.id ? updatedTournament : t));
        setEditingTournament(null);
      } else {
        throw new Error('Failed to update tournament');
      }
    } catch (error) {
      setDialogContent(error.message);
      setDialogOpen(true);
    }
  };

  const handleDeleteTournament = async (id) => {
    try {
      const response = await fetch(ENDPOINTS.DELETE_TOURNAMENT(id), {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
          [HEADER_NAMES.USER]: localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME),
          [HEADER_NAMES.TOKEN]: localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN),
        },
      });

      if (response.ok) {
        setTournaments(tournaments.filter(t => t.id !== id));
      } else {
        throw new Error('Failed to delete tournament');
      }
    } catch (error) {
      setDialogContent(error.message);
      setDialogOpen(true);
    }
  };

  return (
    <>
      <MainMenu />
      <div className="tournamentAdmin">
        <Grid container spacing={2} alignItems="center" style={{ marginBottom: '20px', marginTop: '20px' }}>
          <Grid item xs={3}>
            <TextField
              fullWidth
              label="Tournament Name"
              name="name"
              value={newTournament.name}
              onChange={(e) => handleInputChange(e, setNewTournament)}
            />
          </Grid>
          <Grid item xs={3}>
            <TextField
              fullWidth
              label="Start Date"
              name="startDate"
              value={newTournament.startDate}
              onChange={(e) => handleInputChange(e, setNewTournament)}
              type="date"
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={3}>
            <TextField
              fullWidth
              label="End Date"
              name="endDate"
              value={newTournament.endDate}
              onChange={(e) => handleInputChange(e, setNewTournament)}
              type="date"
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={3}>
            <Button variant="contained" color="primary" onClick={handleAddTournament}>Add Tournament</Button>
          </Grid>
        </Grid>

        {tournaments.map((tournament, index) => (
          <Grid container key={tournament.id} spacing={2} alignItems="center" style={{ marginBottom: '10px' }}>
            <Grid item xs={3}>
              <TextField
                fullWidth
                label="Tournament Name"
                name="name"
                value={editingTournament?.id === tournament.id ? editingTournament.name : tournament.name}
                onChange={(e) => handleInputChange(e, setEditingTournament)}
                disabled={editingTournament?.id !== tournament.id}
              />
            </Grid>
            <Grid item xs={3}>
              <TextField
                fullWidth
                label="Start Date"
                name="startDate"
                value={editingTournament?.id === tournament.id ? new Date(editingTournament.startDate).toISOString().slice(0, 10) : new Date(tournament.startDate).toISOString().slice(0, 10)}
                onChange={(e) => handleInputChange(e, setEditingTournament)}
                type="date"
                InputLabelProps={{ shrink: true }}
                disabled={editingTournament?.id !== tournament.id}
              />
            </Grid>
            <Grid item xs={3}>
              <TextField
                fullWidth
                label="End Date"
                name="endDate"
                value={editingTournament?.id === tournament.id ? new Date(editingTournament.endDate).toISOString().slice(0, 10) : new Date(tournament.endDate).toISOString().slice(0, 10)}
                onChange={(e) => handleInputChange(e, setEditingTournament)}
                type="date"
                InputLabelProps={{ shrink: true }}
                disabled={editingTournament?.id !== tournament.id}
              />
            </Grid>
            <Grid item xs={3}>
              {editingTournament?.id === tournament.id ? (
                <>
                  <Button variant="contained" color="secondary" onClick={handleUpdateTournament} style={{ marginRight: '0px' }}>Save</Button>
                  <Button variant="outlined" onClick={() => setEditingTournament(null)} style={{ marginRight: '0px' }}>Cancel</Button>
                </>
              ) : (
                <>
                  <Button variant="contained" onClick={() => setEditingTournament(tournament)} style={{ marginRight: '0px' }}>Edit</Button>
                  <Button variant="contained" color="error" onClick={() => handleDeleteTournament(tournament.id)} style={{ marginRight: '0px' }}>Delete</Button>
                </>
              )}
            </Grid>
          </Grid>
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
      </div>

      <LogoutButton />
    </>
  );
};

export default TournamentAdmin;
