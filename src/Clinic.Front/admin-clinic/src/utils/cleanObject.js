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
    if (obj[key] !== null && obj[key] !== undefined && obj[key] !== '') {
      newObj[key] = obj[key];
    }
  }
  return newObj;
};