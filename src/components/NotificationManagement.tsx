import React, { useState, useEffect } from 'react';
import { Patient, User, ApiResponse, NotificationResponse, BulkNotificationResponse } from '../types/patient';

interface NotificationManagementProps {
  // Add any props you need
}

const NotificationManagement: React.FC<NotificationManagementProps> = () => {
  // ✅ FIX: Properly type the state variables
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [selectedPatients, setSelectedPatients] = useState<Patient[]>([]);
  const [patients, setPatients] = useState<Patient[]>([]);
  const [emailSubject, setEmailSubject] = useState<string>('');
  const [emailMessage, setEmailMessage] = useState<string>('');
  const [bulkSubject, setBulkSubject] = useState<string>('');
  const [bulkMessage, setBulkMessage] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>('');

  // ✅ FIX: Load patients with proper API response handling
  useEffect(() => {
    const loadPatients = async () => {
      try {
        setLoading(true);
        setError('');
        
        const response = await fetch('/api/Admin/users');
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data: ApiResponse = await response.json();
        
        // ✅ FIX: Extract users array from API response
        const allUsers: User[] = data.users || [];
        
        // Filter for patients and ensure proper typing
        const patientData: Patient[] = allUsers
          .filter(user => user.role === 'patient')
          .map(user => ({
            id: user.id,
            firstName: user.firstName,
            lastName: user.lastName,
            email: user.email,
            phoneNumber: user.phoneNumber,
            role: user.role,
            isActive: user.isActive,
            createdAt: user.createdAt,
            updatedAt: user.updatedAt
          }));
        
        setPatients(patientData);
        console.log('🔔 [NotificationManagement] Loaded patients:', patientData);
      } catch (error) {
        console.error('🔔 [NotificationManagement] Error loading patients:', error);
        setError(`Failed to load patients: ${error}`);
      } finally {
        setLoading(false);
      }
    };

    loadPatients();
  }, []);

  // ✅ FIX: Analytics tracking with proper typing
  const trackEmailAnalytics = (emailSubject: string, emailMessage: string) => {
    console.log('🔔 [NotificationManagement] Email analytics:', {
      hasEmailSubject: !!emailSubject,
      hasEmailMessage: !!emailMessage,
      selectedPatientId: selectedPatient?.id,
      selectedPatientName: selectedPatient ? `${selectedPatient.firstName} ${selectedPatient.lastName}` : 'None',
      selectedPatientEmail: selectedPatient?.email,
      emailSubject: emailSubject,
      emailMessageLength: emailMessage?.length || 0,
      emailMessagePreview: emailMessage ? emailMessage.substring(0, 100) + (emailMessage.length > 100 ? '...' : '') : 'None'
    });
  };

  // ✅ FIX: Send custom notification with proper typing and error handling
  const sendCustomNotification = async () => {
    if (!selectedPatient || !emailSubject || !emailMessage) {
      console.log('🔔 [NotificationManagement] Missing required fields');
      setError('Please select a patient and fill in subject and message');
      return;
    }

    trackEmailAnalytics(emailSubject, emailMessage);

    console.log('🔔 [NotificationManagement] Preparing API call parameters:', {
      patientId: selectedPatient.id,
      patientData: selectedPatient,
      subject: emailSubject,
      message: emailMessage
    });

    try {
      setLoading(true);
      setError('');

      const token = localStorage.getItem('token') || sessionStorage.getItem('token');
      if (!token) {
        throw new Error('No authentication token found');
      }

      const response = await fetch('/api/Notifications/send-custom', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          patientId: selectedPatient.id,
          subject: emailSubject,
          message: emailMessage
        })
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result: NotificationResponse = await response.json();
      
      console.log('🔔 [NotificationManagement] 📧 EMAIL API RESPONSE RECEIVED:', result);
      
      // ✅ FIX: Handle both proper responses and static responses
      if (result.success === true) {
        console.log('🔔 [NotificationManagement] Email sent successfully:', {
          messageId: result.messageId,
          notificationId: result.notificationId,
          recipient: selectedPatient.email,
          subject: emailSubject
        });
        
        alert('Email sent successfully!');
        setEmailSubject('');
        setEmailMessage('');
      } else if (result.message && result.message.includes('Send custom notification endpoint is working')) {
        // Handle static response from backend
        console.log('🔔 [NotificationManagement] Backend returned static response - service not fully deployed');
        setError('Email service is temporarily unavailable. The backend is still deploying the latest changes. Please try again in a few minutes.');
      } else {
        console.log('🔔 [NotificationManagement] Failure details:', {
          error: result.error,
          recipient: selectedPatient.email,
          subject: emailSubject,
          fullResult: result
        });
        
        setError(`Failed to send email: ${result.error || result.message || 'Unknown error'}`);
      }
    } catch (error) {
      console.error('🔔 [NotificationManagement] Error sending email:', error);
      setError(`Error sending email: ${error}`);
    } finally {
      setLoading(false);
    }
  };

  // ✅ FIX: Bulk notification analytics
  const trackBulkAnalytics = (bulkSubject: string, bulkMessage: string) => {
    console.log('🔔 [NotificationManagement] Bulk email analytics:', {
      hasBulkSubject: !!bulkSubject,
      hasBulkMessage: !!bulkMessage,
      selectedPatientCount: selectedPatients.length,
      bulkSubjectLength: bulkSubject?.length || 0,
      bulkMessageLength: bulkMessage?.length || 0,
      bulkMessagePreview: bulkMessage ? bulkMessage.substring(0, 100) + (bulkMessage.length > 100 ? '...' : '') : 'None',
      selectedPatientEmails: selectedPatients.map(p => p.email),
      selectedPatientNames: selectedPatients.map(p => `${p.firstName} ${p.lastName}`)
    });
  };

  // ✅ FIX: Send bulk notifications with proper typing
  const sendBulkNotifications = async () => {
    if (selectedPatients.length === 0 || !bulkSubject || !bulkMessage) {
      console.log('🔔 [NotificationManagement] Missing required fields for bulk send');
      setError('Please select patients and fill in subject and message');
      return;
    }

    trackBulkAnalytics(bulkSubject, bulkMessage);

    console.log('🔔 [NotificationManagement] Preparing bulk API call parameters:', {
      patientCount: selectedPatients.length,
      patients: selectedPatients.map(p => ({
        id: p.id,
        name: `${p.firstName} ${p.lastName}`,
        email: p.email
      })),
      subject: bulkSubject,
      messageLength: bulkMessage.length
    });

    try {
      setLoading(true);
      setError('');

      const token = localStorage.getItem('token') || sessionStorage.getItem('token');
      if (!token) {
        throw new Error('No authentication token found');
      }

      const response = await fetch('/api/Notifications/send-bulk', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          patientIds: selectedPatients.map(p => p.id),
          subject: bulkSubject,
          message: bulkMessage
        })
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result: BulkNotificationResponse = await response.json();
      
      if (result.success) {
        console.log('🔔 [NotificationManagement] Bulk emails sent successfully:', {
          totalSent: result.totalSent,
          totalFailed: result.totalFailed,
          results: result.results
        });
        
        if (result.results && result.results.length > 0) {
          console.log('🔔 [NotificationManagement] Individual email results:');
          result.results.forEach((emailResult, index: number) => {
            console.log(`🔔 [NotificationManagement] Email ${index + 1}:`, {
              patientId: emailResult.patientId,
              success: emailResult.success,
              messageId: emailResult.messageId,
              error: emailResult.error
            });
          });
        }
        
        // Show success message
        alert(`Bulk emails sent! ${result.totalSent} successful, ${result.totalFailed} failed.`);
        
        // Clear form
        setBulkSubject('');
        setBulkMessage('');
        setSelectedPatients([]);
      } else {
        console.log('🔔 [NotificationManagement] Bulk send failed:', {
          error: result.message,
          totalAttempted: selectedPatients.length
        });
        
        // Show error message
        setError(`Failed to send bulk emails: ${result.message || 'Unknown error'}`);
      }
    } catch (error) {
      console.error('🔔 [NotificationManagement] Error sending bulk emails:', error);
      setError(`Error sending bulk emails: ${error}`);
    } finally {
      setLoading(false);
    }
  };

  // ✅ FIX: Patient selection handlers
  const handlePatientSelection = (patient: Patient) => {
    setSelectedPatient(patient);
  };

  const handleBulkPatientSelection = (patient: Patient, isSelected: boolean) => {
    if (isSelected) {
      setSelectedPatients(prev => [...prev, patient]);
    } else {
      setSelectedPatients(prev => prev.filter(p => p.id !== patient.id));
    }
  };

  return (
    <div className="notification-management" style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <h2>Notification Management</h2>
      
      {error && (
        <div style={{ color: 'red', marginBottom: '20px', padding: '10px', border: '1px solid red', borderRadius: '4px' }}>
          {error}
        </div>
      )}
      
      {loading && (
        <div style={{ color: 'blue', marginBottom: '20px' }}>
          Loading...
        </div>
      )}
      
      {/* Patient Selection */}
      <div className="patient-selection" style={{ marginBottom: '30px' }}>
        <h3>Select Patient for Custom Email</h3>
        <select 
          value={selectedPatient?.id || ''} 
          onChange={(e) => {
            const patient = patients.find(p => p.id === e.target.value);
            setSelectedPatient(patient || null);
          }}
          style={{ width: '100%', padding: '8px', marginBottom: '10px' }}
        >
          <option value="">Select a patient</option>
          {patients.map(patient => (
            <option key={patient.id} value={patient.id}>
              {patient.firstName} {patient.lastName} ({patient.email})
            </option>
          ))}
        </select>
        
        {selectedPatient && (
          <div style={{ padding: '10px', backgroundColor: '#f0f0f0', borderRadius: '4px' }}>
            <strong>Selected:</strong> {selectedPatient.firstName} {selectedPatient.lastName} - {selectedPatient.email}
          </div>
        )}
      </div>

      {/* Custom Email Form */}
      <div className="custom-email" style={{ marginBottom: '30px', padding: '20px', border: '1px solid #ccc', borderRadius: '4px' }}>
        <h3>Send Custom Email</h3>
        <input
          type="text"
          placeholder="Email Subject"
          value={emailSubject}
          onChange={(e) => setEmailSubject(e.target.value)}
          style={{ width: '100%', padding: '8px', marginBottom: '10px' }}
        />
        <textarea
          placeholder="Email Message"
          value={emailMessage}
          onChange={(e) => setEmailMessage(e.target.value)}
          rows={4}
          style={{ width: '100%', padding: '8px', marginBottom: '10px' }}
        />
        <button 
          onClick={sendCustomNotification}
          disabled={loading || !selectedPatient || !emailSubject || !emailMessage}
          style={{ 
            padding: '10px 20px', 
            backgroundColor: '#007bff', 
            color: 'white', 
            border: 'none', 
            borderRadius: '4px',
            cursor: loading ? 'not-allowed' : 'pointer',
            opacity: loading ? 0.6 : 1
          }}
        >
          {loading ? 'Sending...' : 'Send Custom Email'}
        </button>
      </div>

      {/* Bulk Email Form */}
      <div className="bulk-email" style={{ padding: '20px', border: '1px solid #ccc', borderRadius: '4px' }}>
        <h3>Send Bulk Email</h3>
        
        <div className="patient-checkboxes" style={{ marginBottom: '20px', maxHeight: '200px', overflowY: 'auto' }}>
          {patients.map(patient => (
            <label key={patient.id} style={{ display: 'block', marginBottom: '5px' }}>
              <input
                type="checkbox"
                checked={selectedPatients.some(p => p.id === patient.id)}
                onChange={(e) => handleBulkPatientSelection(patient, e.target.checked)}
                style={{ marginRight: '8px' }}
              />
              {patient.firstName} {patient.lastName} ({patient.email})
            </label>
          ))}
        </div>
        
        <div style={{ marginBottom: '10px' }}>
          <strong>Selected: {selectedPatients.length} patients</strong>
        </div>
        
        <input
          type="text"
          placeholder="Bulk Email Subject"
          value={bulkSubject}
          onChange={(e) => setBulkSubject(e.target.value)}
          style={{ width: '100%', padding: '8px', marginBottom: '10px' }}
        />
        <textarea
          placeholder="Bulk Email Message"
          value={bulkMessage}
          onChange={(e) => setBulkMessage(e.target.value)}
          rows={4}
          style={{ width: '100%', padding: '8px', marginBottom: '10px' }}
        />
        <button 
          onClick={sendBulkNotifications}
          disabled={loading || selectedPatients.length === 0 || !bulkSubject || !bulkMessage}
          style={{ 
            padding: '10px 20px', 
            backgroundColor: '#28a745', 
            color: 'white', 
            border: 'none', 
            borderRadius: '4px',
            cursor: loading ? 'not-allowed' : 'pointer',
            opacity: loading ? 0.6 : 1
          }}
        >
          {loading ? 'Sending...' : `Send Bulk Email (${selectedPatients.length} patients)`}
        </button>
      </div>
    </div>
  );
};

export default NotificationManagement;
