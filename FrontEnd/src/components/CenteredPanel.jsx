import { Box, Container, Paper } from '@mui/material';
import React from 'react';
import '../style/components/CenteredPanel.css';

function CenteredPanel(props) {
    return (
        <Container component="main" maxWidth="xs" className={props.className}>
            <Box className="box-style" style={{ marginTop: `calc((100vh - ${props.containerHeight ? props.containerHeight + 'px' : '400px'})/2)` }}>
                <Paper className="paper" elevation={3}>
                    {props.children}
                </Paper>
            </Box>
        </Container>
    );
}

export default CenteredPanel;
