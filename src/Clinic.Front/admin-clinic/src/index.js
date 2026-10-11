// src/index.js
import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';
import { store } from './store/store';
import App from './App';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  // <Provider> делает Redux store доступным для всех компонентов внутри <App />
  <Provider store={store}>
    <App />
  </Provider>
);