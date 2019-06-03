import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { CustomerCreateComponent } from './customer-create/customer-create.component';
import { Customers2Component } from './customers2/customers2.component';


const routes: Routes = [
  {path: '', component: CustomerListComponent},
  // {path: ':id', component: CustomerEditComponent},
  {path: 'new', component: CustomerCreateComponent},
  {path: 'customers2', component: Customers2Component}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomersRoutingModule { }
