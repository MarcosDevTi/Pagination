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
import { InlineEditComponent } from './inline-edit/inline-edit.component';
import { Customers2Component } from './customers2/customers2.component';


@NgModule({
  declarations: [
    CustomerListComponent,
    CustomerEditComponent,
    CustomerCreateComponent,
    CustomerHistoryComponent,
    InlineEditComponent,
    Customers2Component ],
  imports: [
    CommonModule,
    CustomersRoutingModule,

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
