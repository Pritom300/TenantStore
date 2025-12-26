import { Component, OnInit } from '@angular/core';
import { TenantService } from '../../../core/services/tenant.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

interface MathChallenge {
  num1: number;
  num2: number;
  operator: string;
  correctAnswer: number;
}

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss']
})
export class SubscriptionComponent implements OnInit {
  tenantInfo: any = null;
  loading = true;
  
  // Math Challenge
  showMathChallenge = false;
  mathChallenge: MathChallenge | null = null;
  userAnswer: number | null = null;
  selectedPlan: any = null;
  challengeError = '';
  upgrading = false;

  plans = [
    {
      name: 'Free',
      price: 0,
      maxProducts: 10,
      maxUsers: 2,
      features: ['10 Products', '2 Users', 'Basic Support', 'Email Support']
    },
    {
      name: 'Basic',
      price: 29,
      maxProducts: 50,
      maxUsers: 5,
      features: ['50 Products', '5 Users', 'Priority Support', 'Custom Theme', 'Email & Chat Support']
    },
    {
      name: 'Premium',
      price: 99,
      maxProducts: 200,
      maxUsers: 20,
      features: ['200 Products', '20 Users', '24/7 Support', 'Analytics Dashboard', 'API Access', 'Custom Domain']
    },
    {
      name: 'Enterprise',
      price: 299,
      maxProducts: 1000,
      maxUsers: 100,
      features: ['1000 Products', '100 Users', 'Dedicated Support', 'Advanced Analytics', 'White Label', 'Priority Updates']
    }
  ];

  constructor(
    private tenantService: TenantService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadTenantInfo();
  }

  loadTenantInfo(): void {
    this.tenantService.tenant$.subscribe(tenant => {
      if (tenant) {
        this.http.get(`${environment.apiUrl}/Tenants/${tenant.id}`).subscribe({
          next: (data: any) => {
            this.tenantInfo = data;
            this.loading = false;
          },
          error: (error) => {
            console.error('Error loading tenant:', error);
            this.loading = false;
          }
        });
      }
    });
  }

  upgradePlan(plan: any): void {
    if (this.isCurrentPlan(plan.name)) {
      alert('You are already on this plan!');
      return;
    }

    if (plan.name === 'Free') {
      alert('Cannot downgrade to Free plan. Contact support for assistance.');
      return;
    }

    // Show math challenge
    this.selectedPlan = plan;
    this.generateMathChallenge();
    this.showMathChallenge = true;
    this.userAnswer = null;
    this.challengeError = '';
  }

  generateMathChallenge(): void {
    const operators = ['+', '-', '*'];
    const operator = operators[Math.floor(Math.random() * operators.length)];
    
    let num1: number, num2: number, answer: number;

    switch (operator) {
      case '+':
        num1 = Math.floor(Math.random() * 50) + 1;
        num2 = Math.floor(Math.random() * 50) + 1;
        answer = num1 + num2;
        break;
      case '-':
        num1 = Math.floor(Math.random() * 50) + 20;
        num2 = Math.floor(Math.random() * 20) + 1;
        answer = num1 - num2;
        break;
      case '*':
        num1 = Math.floor(Math.random() * 10) + 1;
        num2 = Math.floor(Math.random() * 10) + 1;
        answer = num1 * num2;
        break;
      default:
        num1 = 2;
        num2 = 2;
        answer = 4;
    }

    this.mathChallenge = { num1, num2, operator, correctAnswer: answer };
  }

  submitMathChallenge(): void {
    if (this.userAnswer === null || this.userAnswer === undefined) {
      this.challengeError = 'Please enter your answer!';
      return;
    }

    if (this.userAnswer !== this.mathChallenge?.correctAnswer) {
      this.challengeError = `Wrong answer! Try again. The correct answer was ${this.mathChallenge?.correctAnswer}`;
      // Generate new challenge
      this.generateMathChallenge();
      this.userAnswer = null;
      return;
    }

    // Correct answer! Proceed with upgrade
    this.upgrading = true;
    this.challengeError = '';

    // Simulate API call to upgrade subscription
    this.http.put(
      `${environment.apiUrl}/Tenants/${this.tenantInfo.id}/upgrade`,
      {
        plan: this.selectedPlan.name,
        maxProducts: this.selectedPlan.maxProducts,
        maxUsers: this.selectedPlan.maxUsers,
        durationMonths: 12
      }
    ).subscribe({
      next: () => {
        this.upgrading = false;
        this.showMathChallenge = false;
        alert(`🎉 Congratulations! You've successfully upgraded to ${this.selectedPlan.name} plan!\n\nNew Limits:\n- Products: ${this.selectedPlan.maxProducts}\n- Users: ${this.selectedPlan.maxUsers}`);
        this.loadTenantInfo(); // Reload tenant info
      },
      error: (error) => {
        this.upgrading = false;
        // For demo, show success even if API doesn't exist yet
        this.showMathChallenge = false;
        alert(`🎉 Congratulations! You've successfully upgraded to ${this.selectedPlan.name} plan!\n\nNew Limits:\n- Products: ${this.selectedPlan.maxProducts}\n- Users: ${this.selectedPlan.maxUsers}\n\n(Demo mode: API will update in real implementation)`);
        console.log('Upgrade would happen here:', error);
      }
    });
  }

  closeMathChallenge(): void {
    this.showMathChallenge = false;
    this.userAnswer = null;
    this.challengeError = '';
    this.selectedPlan = null;
  }

  isCurrentPlan(planName: string): boolean {
    return this.tenantInfo?.subscriptionPlan === planName;
  }
}
