import React from 'react';
import ReactDOM from 'react-dom/client';
import { PageProvider } from './components/PageProvider';
import Router from './components/Router';
import './index.css';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <PageProvider>
        <Router />
    </PageProvider>
  </React.StrictMode>
);
