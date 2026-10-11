// src/components/common/FormInput.jsx
import React from 'react';
import { TextField } from '@mui/material';

/**
 * A universal input component based on Material-UI's TextField.
 * It encapsulates common styles and error handling logic.
 *
 * @param {object} props - The component's props.
 * @param {string} props.name - The unique name of the field, used for form state management.
 * @param {string} props.label - The label text visible to the user.
 * @param {string} props.value - The current value of the field.
 * @param {function} props.onChange - The callback function for value changes.
 * @param {string|null} [props.error=null] - The error text to display. If a string is passed, the field is highlighted as an error.
 * @param {object} [props.other] - Any other props to be passed directly to the MUI TextField (e.g., type, required, multiline).
 */
const FormInput = ({ name, label, value, onChange, error = null, ...other }) => {
  return (
    <TextField
      // Default styles for consistency across the app
      variant="outlined"
      margin="normal"
      fullWidth

      // Core props for field control
      name={name}
      label={label}
      value={value}
      onChange={onChange}

      // Error handling logic:
      // - The `error` prop of MUI TextField expects a boolean. We convert our error string to a boolean.
      // - `helperText` displays the error message below the field.
      error={!!error}
      helperText={error}

      // Pass all other props (like type="password", required, etc.) directly
      {...other}
    />
  );
};

export default FormInput;