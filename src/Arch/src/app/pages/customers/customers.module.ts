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
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatRadioModule} from '@angular/material/radio';

import { SatPopoverModule } from '@ncstate/sat-popover';

import { CustomerHistoryComponent } from './shared/customer-history/customer-history.component';
import { CustomerService } from './shared/customer.service';
import { Customers2Component } from './customers2/customers2.component';
import { EditInlineComponent } from './edit-inline/edit-inline.component';
import { AutofocusDirective } from './autofocus.directive';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatNativeDateModule, MatInputModule, MatDialogModule } from '@angular/material';
import {MatIconModule} from '@angular/material/icon';
import { CsvConfigComponent } from 'src/app/components/csv-config/csv-config.component';


@NgModule({
  declarations: [
    CustomerListComponent,
    CustomerEditComponent,
    CustomerCreateComponent,
    CustomerHistoryComponent,
    Customers2Component,
    EditInlineComponent,
    AutofocusDirective,
    CsvConfigComponent,
  ],
  imports: [
    CommonModule,
    CustomersRoutingModule,
    FormsModule,
    ReactiveFormsModule,

    SatPopoverModule,

    MatTableModule,
    MatButtonModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    MatCheckboxModule,
    MatDialogModule,
    MatRadioModule,
  ],
  providers: [
    CustomerService,
    MatDatepickerModule,
  ],
  exports: [
    MatFormFieldModule
  ],
  entryComponents: [
    CsvConfigComponent
  ]
})
export class CustomersModule { }
