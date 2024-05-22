import { Component, EventEmitter, Input, Output, output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  @Output() cancelRegister = new EventEmitter();
  model : any = {};
  registerForm : FormGroup = new FormGroup({});
  maxDate : Date = new Date();
  validationErrors: string[] = [];

  constructor(private accountService:AccountService, private toastr: ToastrService, private fb: FormBuilder, private route: Router) {}

  ngOnInit() : void{
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }
  
  initializeForm(){
    this.registerForm = this.fb.group
    ({
        username : ['', Validators.required],
        gender : ['Male'],
        knownAs : ['', Validators.required],
        dateOfBirth : ['', Validators.required],
        city : ['', Validators.required],
        country : ['', Validators.required],
        password : ['', [Validators.minLength(4), Validators.maxLength(8), Validators.required]],
        confirmPassword : ['', [Validators.required, this.matchValues('password')]]
    })
    this.registerForm.controls['password'].valueChanges.subscribe({
      next : () =>  this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      if (!control.parent || !control.parent.get(matchTo)) {
        return null;
      }
      return control.value === control.parent.get(matchTo)?.value ? null : { notMatching: true };
    };
  }
  
  register(){
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value, dateOfBirth : dob };
    this.accountService.register(values).subscribe({
      next : () => {
        this.route.navigateByUrl('/members')
      },
      error: error => this.validationErrors = error
    })

  }

  cancel(){
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob : string | undefined){
    if(!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())).toISOString().slice(0,10);
  }

}
