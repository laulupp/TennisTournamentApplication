
import React from 'react';
import LogoutButton from '../components/LogoutButton';
import MainMenu from '../components/MainMenu';
import '../style/pages/Home.css';

function Home(props) {

    return (
        <>
            <MainMenu />
            <LogoutButton />
            <div className='text-box'>
                Tennis Tournaments Application - Home
            </div>
        </>
    );
}
export default Home;
