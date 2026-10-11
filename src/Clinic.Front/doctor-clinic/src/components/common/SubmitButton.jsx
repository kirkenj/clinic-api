// src/components/common/SubmitButton.jsx
import React from 'react';
import { Button, CircularProgress } from '@mui/material';

/**
 * A universal submit button for forms with a built-in loading indicator.
 *
 * @param {object} props - The component's props.
 * @param {React.ReactNode} props.children - The button's content (e.g., "Save", "Sign In").
 * @param {boolean} [props.isLoading=false] - A flag indicating if the form is currently submitting. Shows a spinner if true.
 * @param {boolean} [props.disabled=false] - An additional flag to disable the button (e.g., if the form is invalid).
 * @param {object} [props.other] - Any other props to be passed directly to the MUI Button (e.g., color, sx, onClick).
 */
const SubmitButton = ({ children, isLoading = false, disabled = false, ...other }) => {
  return (
    <Button
      type="submit"
      // The button is disabled if it's loading OR if it's disabled from the outside.
      disabled={isLoading || disabled}
      variant="contained"
      color="primary"
      fullWidth
      // Set a fixed height to prevent the button from "jumping" when the spinner appears.
      sx={{ mt: 3, mb: 2, height: '42px' }}
      {...other}
    >
      {isLoading ? (
        <CircularProgress
          size={24} // Make the spinner slightly smaller than the button height.
          color="inherit" // The spinner will inherit the button's text color.
        />
      ) : (
        children
      )}
    </Button>
  );
};

export default SubmitButton;