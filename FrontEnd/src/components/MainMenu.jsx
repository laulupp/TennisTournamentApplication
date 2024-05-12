import { Drawer, List, ListItemButton, ListItemText, Typography } from '@mui/material';
import React from 'react';
import '../style/components/MainMenu.css';
import { LOCAL_STORAGE_KEYS } from '../utils/LocalStorageKeys';
import { usePage } from './PageProvider';


function MainMenu(props) {
    const { setCurrentPage } = usePage();

    return (
        <>
            {props.empty == 1 && <div className="title" style={{ width: '100px', height: '100px' }}>
                <p></p>
            </div>}
            <Drawer variant="permanent" className="mainMenu">
                <div className="welcomeMessage">
                    <Typography variant="h6">Welcome,</Typography>
                    <Typography variant="h6" style={{ fontWeight: 'bold', color: 'var(--color-purple-background)' }}>{localStorage.getItem(LOCAL_STORAGE_KEYS.FIRST_NAME)} {localStorage.getItem(LOCAL_STORAGE_KEYS.LAST_NAME)}</Typography>
                </div>
                <List>
                    <ListItemButton component="a" onClick={() => setCurrentPage(2)}>
                        <ListItemText primary="Home" />
                    </ListItemButton>
                    {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) == 0 && <ListItemButton component="a" onClick={() => setCurrentPage(3)}>
                        <ListItemText primary="Tournaments" />
                    </ListItemButton>}
                    {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) == 2 && <ListItemButton component="a" onClick={() => setCurrentPage(4)}>
                        <ListItemText primary="Tournaments" />
                    </ListItemButton>}
                    {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) == 2 && <ListItemButton component="a" onClick={() => setCurrentPage(5)}>
                        <ListItemText primary="Matches" />
                    </ListItemButton>}
                    {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) == 2 && <ListItemButton component="a" onClick={() => setCurrentPage(6)}>
                        <ListItemText primary="UserManagement" />
                    </ListItemButton>}
                    {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) != 2 && <ListItemButton component="a" onClick={() => setCurrentPage(7)}>
                        <ListItemText primary="UserSettings" />
                    </ListItemButton>}
                    {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) != 2 && <ListItemButton component="a" onClick={() => setCurrentPage(8)}>
                        <ListItemText primary="MyMatches" />
                    </ListItemButton>}
                    {localStorage.getItem(LOCAL_STORAGE_KEYS.ROLE) == 2 && <ListItemButton component="a" onClick={() => setCurrentPage(9)}>
                        <ListItemText primary="TournamentRequests" />
                    </ListItemButton>}
                </List>
            </Drawer>
        </>
    );
}

export default MainMenu;
