export interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ApiResponse {
  users: User[];
  pagination: {
    currentPage: number;
    totalPages: number;
    totalItems: number;
    itemsPerPage: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
  };
  filters: {
    search: string;
    role: string;
    status: string;
  };
  success: boolean;
  error: string;
}

export interface NotificationResponse {
  success: boolean;
  message: string;
  notificationId?: string;
  messageId?: string;
  error?: string;
}

export interface BulkNotificationResponse {
  success: boolean;
  message: string;
  totalSent: number;
  totalFailed: number;
  results: Array<{
    patientId: string;
    success: boolean;
    messageId?: string;
    error?: string;
  }>;
}










