import { ComponentFixture, TestBed } from '@angular/core/testing';

import { L1C1VocalDrillsComponent } from './l1-c1-vocal-drills.component';

describe('L1C1VocalDrillsComponent', () => {
  let component: L1C1VocalDrillsComponent;
  let fixture: ComponentFixture<L1C1VocalDrillsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ L1C1VocalDrillsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(L1C1VocalDrillsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
