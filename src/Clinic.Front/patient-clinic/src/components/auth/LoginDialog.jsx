// src/components/auth/LoginDialog.jsx
import React from 'react';
import { Dialog, DialogTitle, DialogContent, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import Login from '../../pages/auth/Login'; // Мы можем переиспользовать саму форму!

const LoginDialog = ({ open, onClose, onSuccess }) => {
  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>
        Sign In or Sign Up
        <IconButton aria-label="close" onClick={onClose} sx={{ position: 'absolute', right: 8, top: 8 }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        {/* 
          Чтобы не дублировать код, мы можем передать в Login проп, который изменит его поведение.
          Например, после успешного логина он не будет делать navigate, а вызовет onSuccess.
        */}
        <Login isDialogMode onSuccess={onSuccess} />
      </DialogContent>
    </Dialog>
  );
};

export default LoginDialog;