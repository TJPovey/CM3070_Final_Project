import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { IonButton, IonContent, IonHeader, IonImg, IonItem, IonLabel, IonText, IonTitle, IonToolbar } from '@ionic/angular/standalone';
import { ProfileFacadeService } from 'src/app/facade/Profile/profile-facade.service';
import { AppRoute } from '../app-routes.enum';
import { UserDtoExtensions } from 'src/app/extensions/UserDtoExtensions';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonItem,
    IonContent,
    IonLabel,
    IonButton,
    IonText,
    IonImg,
    ReactiveFormsModule,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegisterComponent {
  

  private _formBuilder = inject(FormBuilder);
  private _router = inject(Router);
  private _profileFacadeService = inject(ProfileFacadeService);

  protected registerForm: FormGroup;
  protected isLoginForm = true;

  constructor() {
    this.registerForm = this._formBuilder.group({
      username: ['', [Validators.required, Validators.minLength(6)]],
      firstname: ['', [this.booleanValidator()]],
      lastname: ['', [this.booleanValidator()]],
      email: ['', [this.booleanValidator()]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }


  protected onSubmit() {
    if (this.registerForm.valid) {

      if (this.isLoginForm) {
        this.login();
      } else {
        this.register();
      }
    }
  }

  protected toggleForm() {
    this.isLoginForm = !this.isLoginForm;
    this.registerForm.updateValueAndValidity();
    this.registerForm.reset();
  }

  private register() {
      const formData = this.registerForm.value;

      const dto = UserDtoExtensions.ToUserPostDto(
        formData["firstname"],
        formData["lastname"],
        formData["username"],
        formData["email"],
        formData["password"]);

      this._profileFacadeService.registerUser(dto).subscribe(res => {
        this._router.navigate([`${AppRoute.Home}/{${AppRoute.Property_List}}`])
      });
  }

  private login() {
    const formData = this.registerForm.value;

    const dto = UserDtoExtensions.ToTokenPostDto(
      formData["username"],
      formData["password"]);

    this._profileFacadeService.login(dto).subscribe(res => {
      this._router.navigate([`${AppRoute.Home}/${AppRoute.Property_List}`])
    });;
  }

  private booleanValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {

      if (!this.isLoginForm && !control.value) {
        return { required: true };
      }
      
      return null;
    };
  }

}
