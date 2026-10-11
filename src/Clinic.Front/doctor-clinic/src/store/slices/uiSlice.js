// src/store/slices/uiSlice.js
import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  isLoading: false,   // For the global preloader managed by axios interceptors
  isLoggingIn: false, // Flag to prevent auth race conditions
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    setLoading: (state, action) => {
      state.isLoading = action.payload;
    },
    setIsLoggingIn: (state, action) => {
      state.isLoggingIn = action.payload;
    },
  },
});

export const { setLoading, setIsLoggingIn } = uiSlice.actions;
export default uiSlice.reducer;