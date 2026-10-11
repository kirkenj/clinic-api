// src/hooks/useDebounce.js
import { useState, useEffect } from 'react';

/**
 * A custom hook that debounces a value.
 * Useful for search inputs to avoid sending a request on every keystroke.
 *
 * @param {*} value - The value to be debounced.
 * @param {number} delay - The delay in milliseconds.
 * @returns {*} The debounced value.
 */
function useDebounce(value, delay) {
  // State to store the debounced value
  const [debouncedValue, setDebouncedValue] = useState(value);

  useEffect(() => {
    // Set up a timer that will update the value after the `delay`
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    // Cleanup function: This will be called if `value` or `delay` changes
    // before the timer expires. This prevents updating the value if the user
    // continues typing.
    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]); // The effect is re-triggered only when value or delay changes

  return debouncedValue;
}

export default useDebounce;