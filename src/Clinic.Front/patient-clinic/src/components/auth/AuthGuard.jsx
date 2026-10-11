// src/components/auth/AuthGuard.jsx
import React, { useState } from 'react';
import { useAuth } from '../../hooks/useAuth';
import LoginDialog from './LoginDialog'; // Наш новый диалог

/**
 * Компонент-обертка, который проверяет авторизацию перед выполнением действия.
 * @param {object} props
 * @param {function} props.onSuccess - Коллбэк, который выполнится, если пользователь авторизован (или только что авторизовался).
 * @param {React.ReactNode} props.children - Элемент, который будет триггерить проверку (например, кнопка).
 */
const AuthGuard = ({ children, onSuccess }) => {
  const { isAuthenticated } = useAuth();
  const [isLoginDialogOpen, setIsLoginDialogOpen] = useState(false);

  const handleClick = () => {
    if (isAuthenticated) {
      onSuccess(); // Если пользователь уже залогинен, просто выполняем действие
    } else {
      setIsLoginDialogOpen(true); // Если нет - открываем диалог входа
    }
  };
  
  // Клонируем дочерний элемент (кнопку) и добавляем ему наш onClick
  const triggerElement = React.cloneElement(children, { onClick: handleClick });

  return (
    <>
      {triggerElement}
      <LoginDialog
        open={isLoginDialogOpen}
        onClose={() => setIsLoginDialogOpen(false)}
        onSuccess={() => {
          setIsLoginDialogOpen(false);
          onSuccess(); // После успешного входа в диалоге, выполняем исходное действие
        }}
      />
    </>
  );
};

export default AuthGuard;