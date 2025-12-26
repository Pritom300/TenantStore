export interface Tenant {
  id: string;
  name: string;
  subdomain: string;
  themeColor: string;
  isActive: boolean;
  subscriptionPlan: string;
  subscriptionExpiresAt?: Date;
  maxProducts: number;
  maxUsers: number;
  createdAt: Date;
}

export interface TenantInfo {
  id: string;
  name: string;
  subdomain: string;
  themeColor: string;
}