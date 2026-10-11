// src/utils/cleanObject.js

/**
 * Removes properties with null, undefined, or empty string values from an object.
 * Useful for cleaning up filter objects before sending them as API query params.
 * @param {object} obj The object to clean.
 * @returns {object} A new object with "empty" properties removed.
 */
export const cleanObject = (obj) => {
  const newObj = {};
  for (const key in obj) {
    const value = obj[key];
    if (value !== null && value !== undefined && value !== '') {
      if (!Array.isArray(value) || value.length > 0) {
        newObj[key] = value;
      }
    }
  }
  return newObj;
};