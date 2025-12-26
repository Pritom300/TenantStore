import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageTenantsComponent } from './manage-tenants.component';

describe('ManageTenantsComponent', () => {
  let component: ManageTenantsComponent;
  let fixture: ComponentFixture<ManageTenantsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ManageTenantsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ManageTenantsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
