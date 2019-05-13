import { Component, ViewChild, OnInit } from '@angular/core';
import { MatTableDataSource, MatSort } from '@angular/material';
import { CustomerService } from '../shared/customer.service';
import { Customer } from '../shared/customer.module';

import {animate, state, style, transition, trigger} from '@angular/animations';

export interface PeriodicElement {
  name: string;
  position: number;
  weight: number;
  symbol: string;
}

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0', display: 'none'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class CustomerListComponent implements OnInit {
data: any[]
  customers: Customer[] = [];
  constructor(private customerService: CustomerService) { }

  displayedColumns: string[];
  columnsToDisplay: string[];
  dataSource = this.customers;
  expandedElement: PeriodicElement | null;

  ngOnInit(): void {
    this.customerService.getAll(0, 20, 'FirstName', 'Descending', 'Teste').subscribe(
      customers => {
        this.customers = customers;
        this.displayedColumns = Object.keys(this.customers[0]);
        this.columnsToDisplay = Object.keys(this.customers[0]);
      },
      error => alert('Erro ao carregar a lista')
    );
  }

}