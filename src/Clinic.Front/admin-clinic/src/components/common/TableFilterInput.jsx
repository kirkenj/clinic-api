// src/components/common/TableFilterInput.jsx
import React from 'react';
import { TextField, InputAdornment, IconButton } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';

/**
 * A reusable search input component for table filters.
 * Includes a search icon and a clear button.
 *
 * @param {object} props - The component's props.
 * @param {string} props.value - The current search term.
 * @param {function} props.onChange - Callback function to handle input changes.
 * @param {string} [props.label='Search...'] - The input field's label.
 * @param {object} [props.other] - Any other props to be passed to the MUI TextField.
 */
const TableFilterInput = ({ value, onChange, label = 'Search...', ...other }) => {
  // Handler for the clear button
  const handleClear = () => {
    // We simulate an event object to be compatible with a generic `handleFilterChange`
    const event = {
      target: {
        name: other.name || 'search', // Use the provided name or default to 'search'
        value: '',
      },
    };
    onChange(event);
  };

  return (
    <TextField
      fullWidth
      variant="outlined"
      label={label}
      value={value}
      onChange={onChange}
      InputProps={{
        startAdornment: (
          <InputAdornment position="start">
            <SearchIcon />
          </InputAdornment>
        ),
        // Add a clear button at the end if there is text in the input
        endAdornment: value && (
          <InputAdornment position="end">
            <IconButton
              aria-label="clear search"
              onClick={handleClear}
              edge="end"
            >
              <ClearIcon />
            </IconButton>
          </InputAdornment>
        ),
      }}
      {...other}
    />
  );
};

export default TableFilterInput;