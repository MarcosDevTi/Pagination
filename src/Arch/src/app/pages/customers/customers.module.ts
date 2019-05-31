import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CustomersRoutingModule } from './customers-routing.module';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { CustomerCreateComponent } from './customer-create/customer-create.component';

import {MatTableModule} from '@angular/material/table';
import {MatButtonModule} from '@angular/material/button';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatFormFieldModule} from '@angular/material/form-field';

import { SatPopoverModule } from '@ncstate/sat-popover';

import { CustomerHistoryComponent } from './shared/customer-history/customer-history.component';
import { CustomerService } from './shared/customer.service';
import { Customers2Component } from './customers2/customers2.component';
import { EditInlineComponent } from './edit-inline/edit-inline.component';
import { AutofocusDirective } from './autofocus.directive';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    CustomerListComponent,
    CustomerEditComponent,
    CustomerCreateComponent,
    CustomerHistoryComponent,
    Customers2Component,
    EditInlineComponent,
    AutofocusDirective],
  imports: [
    CommonModule,
    CustomersRoutingModule,
    FormsModule,

    SatPopoverModule,

    MatTableModule,
    MatButtonModule,
    MatPaginatorModule,
    MatFormFieldModule,
  ],
  providers: [
    CustomerService
  ],
  exports: [
    MatFormFieldModule
  ]
})
export class CustomersModule { }
