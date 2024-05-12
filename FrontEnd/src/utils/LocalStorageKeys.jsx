export const LOCAL_STORAGE_KEYS = {
    USERNAME: 'username',
    TOKEN: 'token',
    FIRST_NAME: 'firstName',
    LAST_NAME: 'lastName',
    ROLE: 'role',
};
  
export const clearLocalStorage = () => {
    Object.values(LOCAL_STORAGE_KEYS).forEach(key => localStorage.removeItem(key));
};