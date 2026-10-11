// src/context/ErrorBoundary.jsx
import React from 'react';
import ErrorFallback from '../components/ErrorFallback'; // Он рендерит этот компонент при ошибке

/**
 * An Error Boundary is a React component that catches JavaScript errors
 * anywhere in its child component tree, logs those errors, and displays
 * a fallback UI instead of the component tree that crashed.
 */
class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false };
  }

  /**
   * This lifecycle method is invoked after an error has been thrown by a
   * descendant component. It receives the error that was thrown and should
   * return a value to update state.
   */
  static getDerivedStateFromError(error) {
    // Update state so the next render will show the fallback UI.
    return { hasError: true };
  }

  /**
   * This lifecycle method is also invoked after an error has been thrown.
   * It's a good place for logging the error details.
   */
  componentDidCatch(error, errorInfo) {
    // You can also log the error to an error reporting service like Sentry, LogRocket, etc.
    console.error("Uncaught error:", error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      // If an error occurred, render our custom fallback UI.
      return <ErrorFallback />;
    }

    // Otherwise, render the children as normal.
    return this.props.children;
  }
}

export default ErrorBoundary;