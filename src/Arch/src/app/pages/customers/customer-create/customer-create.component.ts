import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerService } from '../shared/customer.service';
import { Customer } from '../shared/customer.model';

@Component({
  selector: 'app-customer-create',
  templateUrl: './customer-create.component.html',
  styleUrls: ['./customer-create.component.scss']
})
export class CustomerCreateComponent implements OnInit {

  customerForm: FormGroup;

  constructor(private formBuilder: FormBuilder, private customerService: CustomerService) { }

  ngOnInit() {
    this.buildCategoryForm();
  }

  submitForm() {
    this.createCustomer();
  }

  private buildCategoryForm(){
    this.customerForm = this.formBuilder.group({
      firstName: [null, [Validators.required, Validators.minLength(2)]],
      lastName: [null, [Validators.required, Validators.minLength(2)]],
      birthDate: [null],
      email: [null],
      score: [null]
    });
  }

  public createCustomer(){
    const customer: Customer = Object.assign(new Customer(), this.customerForm.value)
    this.customerService.create(customer)
    .subscribe(
      customerReturn => {
        return this.actionsForSuccess(customerReturn);
      },
      error => this.actionsForError(error)
    )
  }

  private actionsForSuccess(customer: Customer) {
    console.log('Customer created')
  }

  private actionsForError(error){
    console.log('Error on create customer');
  }
}
