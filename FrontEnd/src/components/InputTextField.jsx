import { TextField } from '@mui/material';
import React from 'react';
import '../style/components/InputTextField.css';

function InputTextField(props) {
    return (
        <TextField
            {...props}
            variant="outlined"
            margin="normal"
            required
            fullWidth
            autoFocus
            className="formInput"
        />
    )
}    

export default InputTextField