import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContentProviderHeaderComponent } from './content-provider-header.component';

describe('ContentProviderHeaderComponent', () => {
  let component: ContentProviderHeaderComponent;
  let fixture: ComponentFixture<ContentProviderHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ContentProviderHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ContentProviderHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
