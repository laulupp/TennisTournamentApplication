import { Button } from '@mui/material';
import React from 'react';
import '../style/components/MainButton.css';

function MainButton(props) {
    return (
        <Button
            type="submit"
            fullWidth
            variant="contained"
            className="formButton"
            onClick={props.onClick}
        >
            {props.text ?? "please define text"}
        </Button>
    )
}

export default MainButton;