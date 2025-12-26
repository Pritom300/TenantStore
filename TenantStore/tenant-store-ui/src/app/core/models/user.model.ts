export interface User {
  id: string;
  name: string;
  email: string;
  role: 'SuperAdmin' | 'Admin' | 'User';
  isActive: boolean;
  createdAt: Date;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: {
    id: string;
    name: string;
    email: string;
    role: string;
  };
  tenant: {
    id: string;
    name: string;
    subdomain: string;
    themeColor: string;
  };
}

export interface UpdateUserDto {
  name: string;
  email: string;
}