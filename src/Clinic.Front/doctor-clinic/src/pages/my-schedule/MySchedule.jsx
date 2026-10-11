import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, Paper, Typography, CircularProgress, Alert, Chip } from '@mui/material';
import { Calendar, dateFnsLocalizer } from 'react-big-calendar';
import { format, parse, startOfWeek, getDay, parseISO, startOfMonth, endOfMonth, startOfToday } from 'date-fns';
import enUS from 'date-fns/locale/en-US';
import 'react-big-calendar/lib/css/react-big-calendar.css';

import appointmentService from '../../services/appointmentService';
import patientService from '../../services/patientService';
import { handleApiError } from '../../utils/handleApiError';
import { useAuth } from '../../hooks/useAuth';
import { APPOINTMENT_STATUSES, APPOINTMENT_STATUS_FROM_STRING, APPOINTMENT_STATUS_TEXT, APPOINTMENT_STATUS_COLORS } from '../../constants/appointmentConstants';
import { cleanObject } from '../../utils/cleanObject';

const locales = { 'en-US': enUS };
const localizer = dateFnsLocalizer({ format, parse, startOfWeek, getDay, locales });

const MySchedule = () => {
  const navigate = useNavigate();
  const { user } = useAuth();

  const [events, setEvents] = useState([]);
  const [patientsMap, setPatientsMap] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [timetableOptions, setTimetableOptions] = useState(null);
  const [currentRange, setCurrentRange] = useState(null);

  useEffect(() => {
    appointmentService.getTimetableOptions()
      .then(options => setTimetableOptions(options))
      .catch(err => setError(handleApiError(err).general));
  }, []);

  const fetchAppointments = useCallback(async (range) => {
    if (!user?.id || !range || !timetableOptions) return;

    setLoading(true);
    setError(null);
    try {
      const params = {
        DoctorIds: [user.id],
        DateTimeStart: range.start.toISOString(),
        DateTimeEnd: range.end.toISOString(),
        Statuses: [APPOINTMENT_STATUSES.WaitingClinicConfirm, APPOINTMENT_STATUSES.Confirmed],
        page: 1,
        pageSize: 500,
      };
      
      const cleanedParams = cleanObject(params);
      const response = await appointmentService.getPaged(cleanedParams);
      const appointmentsData = response.items || [];
      
      if (appointmentsData.length > 0) {
        const patientIds = [...new Set(appointmentsData.map(a => a.patientId).filter(Boolean))];
        if (patientIds.length > 0) {
          const patientsResponse = await patientService.getPaged({ Ids: patientIds, page: 1, pageSize: patientIds.length });
          const patientsDataMap = (patientsResponse.items || []).reduce((acc, p) => ({ ...acc, [p.id]: p }), {});
          setPatientsMap(prev => ({...prev, ...patientsDataMap}));
        }
      }

      const slotDuration = timetableOptions.timeSlotSizeMinutes;
      const calendarEvents = appointmentsData.map(apt => {
        const numericStatus = APPOINTMENT_STATUS_FROM_STRING[apt.status] ?? -1;
        return {
          id: apt.id,
          title: APPOINTMENT_STATUS_TEXT[numericStatus] || 'Appointment',
          start: parseISO(apt.dateTime),
          end: new Date(parseISO(apt.dateTime).getTime() + slotDuration * 60 * 1000), 
          allDay: false,
          resource: { ...apt, status: numericStatus },
        };
      });
      setEvents(calendarEvents);
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [user?.id, timetableOptions]);

  useEffect(() => {
    if (currentRange && timetableOptions) {
      fetchAppointments(currentRange);
    }
  }, [currentRange, timetableOptions, fetchAppointments]);
  
  useEffect(() => {
    const today = startOfToday();
    setCurrentRange({ start: startOfMonth(today), end: endOfMonth(today) });
  }, []);

  const handleRangeChange = useCallback((range) => {
    if (!timetableOptions) return;
    let start, end;
    if (Array.isArray(range)) {
      start = range[0];
      end = range[range.length - 1];
    } else if (range.start && range.end) {
      start = range.start;
      end = range.end;
    } else { return; }
    
    const [startHour, startMinute] = timetableOptions.workTimeStart.split(':').map(Number);
    const finalStart = new Date(start.setHours(startHour, startMinute, 0, 0));
    const [endHour, endMinute] = timetableOptions.workTimeEnd.split(':').map(Number);
    const finalEnd = new Date(end.setHours(endHour, endMinute, 59, 999));

    setCurrentRange({ start: finalStart, end: finalEnd });
  }, [timetableOptions]);

  const EventComponent = ({ event }) => {
    const patient = patientsMap[event.resource.patientId];
    return (
      <Box sx={{ lineHeight: 1.2, fontSize: '0.8em', p: '2px' }}>
        <strong>{format(event.start, 'HH:mm')}</strong>
        <Typography variant="body2" noWrap>
          {patient ? `${patient.lastName} ${patient.firstName}` : 'Loading...'}
        </Typography>
        <Chip 
          label={APPOINTMENT_STATUS_TEXT[event.resource.status] || 'Unknown'} 
          color={APPOINTMENT_STATUS_COLORS[event.resource.status] || 'default'}
          size="small" 
          sx={{ height: 'auto', '& .MuiChip-label': { p: '0 6px' } }}
        />
      </Box>
    );
  };
  
  const eventPropGetter = useCallback((event) => {
    const statusColor = APPOINTMENT_STATUS_COLORS[event.resource.status] || 'default';
    const colorMap = {
      primary: '#1976d2', warning: '#ed6c02', error: '#d32f2f',
      info: '#0288d1', success: '#2e7d32', secondary: '#9c27b0', default: '#bdbdbd',
    };
    const backgroundColor = colorMap[statusColor];
    return { style: { backgroundColor, opacity: 0.8, color: 'white', border: 0, borderRadius: '5px' } };
  }, []);

  const { min, max, dayPropGetter } = useMemo(() => {
    if (!timetableOptions) return { min: null, max: null, dayPropGetter: () => ({}) };
    const today = new Date();
    const minTime = parseISO(`${today.toISOString().split('T')[0]}T${timetableOptions.workTimeStart}Z`);
    const maxTime = parseISO(`${today.toISOString().split('T')[0]}T${timetableOptions.workTimeEnd}Z`);
    const dayPropGetterFunc = (date) => {
      const dayOfWeek = date.getDay(); // 0 = Sunday, 6 = Saturday (JS standard)
      if (dayOfWeek < timetableOptions.workWeekDayStart || dayOfWeek > timetableOptions.workWeekDayEnd) {
        return { style: { background: '#f5f5f5' } };
      }
      return {};
    };
    return { min: minTime, max: maxTime, dayPropGetter: dayPropGetterFunc };
  }, [timetableOptions]);

  return (
    <Paper sx={{ p: 2, height: '85vh', display: 'flex', flexDirection: 'column' }}>
      <Typography variant="h4" sx={{ mb: 2 }}>My Schedule</Typography>
      {error && <Alert severity="error">{error}</Alert>}
      <Box sx={{ flexGrow: 1, position: 'relative' }}>
        {(loading || !timetableOptions) && <CircularProgress sx={{ position: 'absolute', top: '50%', left: '50%', transform: 'translate(-50%, -50%)', zIndex: 1 }} />}
        {timetableOptions && (
          <Calendar
            localizer={localizer}
            events={events}
            startAccessor="start"
            endAccessor="end"
            style={{ height: '100%' }}
            onSelectEvent={event => navigate(`/appointments/${event.id}`)}
            onRangeChange={handleRangeChange}
            views={['month', 'week', 'day']}
            eventPropGetter={eventPropGetter}
            components={{ event: EventComponent }}
            min={min}
            max={max}
            step={timetableOptions.timeSlotSizeMinutes}
            timeslots={2}
            dayPropGetter={dayPropGetter}
            defaultView="week"
            defaultDate={new Date()}
          />
        )}
      </Box>
    </Paper>
  );
};

export default MySchedule;