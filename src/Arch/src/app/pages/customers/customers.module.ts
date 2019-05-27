import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CustomersRoutingModule } from './customers-routing.module';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { CustomerCreateComponent } from './customer-create/customer-create.component';

import {MatTableModule} from '@angular/material/table';
import {MatButtonModule} from '@angular/material/button';
import {MatPaginatorModule} from '@angular/material/paginator';

import { CustomerHistoryComponent } from './shared/customer-history/customer-history.component';
import { CustomerService } from './shared/customer.service';

@NgModule({
  declarations: [CustomerListComponent, CustomerEditComponent, CustomerCreateComponent, CustomerHistoryComponent  ],
  imports: [
    CommonModule,
    CustomersRoutingModule,

    MatTableModule,
    MatButtonModule,
    MatPaginatorModule,
  ],
  providers: [
    CustomerService
  ]
})
export class CustomersModule { }
