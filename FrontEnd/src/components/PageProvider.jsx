import React, { createContext, useContext, useEffect, useState } from 'react';
import { ENDPOINTS } from '../utils/Endpoints';
import { HEADER_NAMES } from '../utils/HeaderNames';
import { LOCAL_STORAGE_KEYS, clearLocalStorage } from '../utils/LocalStorageKeys';

const PageContext = createContext();

export const usePage = () => useContext(PageContext);

export const PageProvider = ({ children }) => {
    const [currentPage, setCurrentPage] = useState(0);

    useEffect(() => {
        const verifyToken = async () => {
            const token = localStorage.getItem(LOCAL_STORAGE_KEYS.TOKEN);
            const username = localStorage.getItem(LOCAL_STORAGE_KEYS.USERNAME);
            
            if (!token || !username) {
                setCurrentPage(0);
                return;
            }

            try {
                const response = await fetch(ENDPOINTS.VERIFY_TOKEN, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        [HEADER_NAMES.USER]: username,
                        [HEADER_NAMES.TOKEN]: token,
                    },
                });

                if (response.ok) {
                    setCurrentPage(2);
                    console.log("Previous token OK");
                } else {
                    clearLocalStorage();
                    setCurrentPage(0);
                    console.log("Previous token NOT OK");
                }
            } catch (error) {
                console.error(error);
                setCurrentPage(0);
            }
        };

        verifyToken();
    }, []);

    return (
        <PageContext.Provider value={{ currentPage, setCurrentPage }}>
            {children}
        </PageContext.Provider>
    );
};
