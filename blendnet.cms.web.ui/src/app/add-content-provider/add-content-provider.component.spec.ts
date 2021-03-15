import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddContentProviderComponent } from './add-content-provider.component';


describe('AddContentProviderComponent', () => {
  let component: AddContentProviderComponent;
  let fixture: ComponentFixture<AddContentProviderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddContentProviderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddContentProviderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
