import React from 'react';
import Home from '../pages/Home';
import Login from '../pages/Login';
import MatchAdmin from '../pages/MatchAdmin';
import MyMatches from '../pages/MyMatches';
import Register from '../pages/Register';
import Tournament from '../pages/Tournament';
import TournamentAdmin from '../pages/TournamentAdmin';
import UserManagement from '../pages/UserManagement';
import UserSettings from '../pages/UserSettings';
import { usePage } from './PageProvider';
import TournamentRequests from '../pages/TournamentRequests';

function Router(props) {
    const { currentPage } = usePage();

    const renderPage = () => {
        switch (currentPage) {
            case 0:
                return <Login />;
            case 1:
                return <Register />;
            case 3:
                return <Tournament />;
            case 4:
                return <TournamentAdmin />;
            case 5:
                return <MatchAdmin />;
            case 6:
                return <UserManagement />;
            case 7:
                return <UserSettings />;
            case 8:
                return <MyMatches />;
            case 9:
                return <TournamentRequests />;
            default:
                return <Home />;
        }
    }
    return (
        <>
            {renderPage()}
        </>
    );

}

export default Router;
