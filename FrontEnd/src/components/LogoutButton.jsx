import LogoutIcon from '@mui/icons-material/Logout';
import { Button } from '@mui/material';
import React from 'react';
import { clearLocalStorage } from '../utils/LocalStorageKeys';
import { usePage } from './PageProvider';

function LogoutButton(props) {
    const { setCurrentPage } = usePage();
    return (
        <div className="logoutButton">
            <Button
            color="error"
            onClick={() => { clearLocalStorage(); setCurrentPage(0);}}
            startIcon={<LogoutIcon />}
            >
            Logout
            </Button>
        </div>
    )
}

export default LogoutButton;