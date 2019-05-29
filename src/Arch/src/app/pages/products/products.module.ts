import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductsRoutingModule } from './products-routing.module';
import { ProductListComponent } from './product-list/product-list.component';

import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatButtonModule} from '@angular/material/button';
import {MatRadioModule} from '@angular/material/radio';
import {MatSelectModule} from '@angular/material/select';
import {MatIconModule} from '@angular/material/icon';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatCardModule} from '@angular/material/card';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NewProductComponent } from './new-product/new-product.component';
import { ProductPrincipalComponent } from './new-product/product-principal/product-principal.component';
import { ProductDetailsComponent } from './new-product/product-details/product-details.component';
import {MatTabsModule} from '@angular/material/tabs';
import { ProductService } from './product.service';

@NgModule({
  declarations: [ProductListComponent, NewProductComponent, ProductPrincipalComponent, ProductDetailsComponent],
  imports: [
    CommonModule,
    ProductsRoutingModule,
    MatAutocompleteModule,
    MatInputModule,
    MatFormFieldModule,
    MatCheckboxModule,
    MatButtonModule,
    MatRadioModule,
    MatSelectModule,
    MatIconModule,
    MatGridListModule,
    MatCardModule,
    MatTabsModule,

    ReactiveFormsModule
  ],
  providers: [
    ProductService
  ]
})
export class ProductsModule { }
