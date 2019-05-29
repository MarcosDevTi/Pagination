import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-new-product',
  templateUrl: './new-product.component.html',
  styleUrls: ['./new-product.component.scss']
})
export class NewProductComponent {

 formProduct = this.fb.group({
  isOk: [false],
  name: ['', [Validators.required, Validators.maxLength(10)]],
  price: ['', [Validators.required, Validators.minLength(4)]],
 });


  constructor(private fb: FormBuilder){}

  onSubmit() {
    // TODO: Use EventEmitter with form value
    console.warn(this.formProduct.value);
  }

  mudouIsOk(){
    console.log("IsOk is changed to: " + this.formProduct.value.isOk)
  }
}
