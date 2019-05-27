import { Component, ViewChild, OnInit } from '@angular/core';
import { MatTableDataSource, MatSort, PageEvent } from '@angular/material';
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
  constructor(private customerService: CustomerService) { }
data: any[]
  customers: Customer[] = [];

  displayedColumns: any[];
  columnsToDisplay: string[];
  dataSource = this.customers;
  expandedElement: PeriodicElement | null;
  orderByDirection = 'Ascending'
  nameOrder = "Name"

  length = 100;
  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  pageIndexCount = 0;

  pageEvent: PageEvent;

  ngOnInit(): void {
    
    this.customerService.getAll(this.pageIndexCount, this.pageSize, this.nameOrder, this.orderByDirection, '').subscribe(
      customers => {
        this.customers = customers.items;
        this.displayedColumns = customers.head;
        this.columnsToDisplay = customers.head.map(x => x.viewPropCamelCase);
        this.length = customers.totalItems;
      },
      error => alert('Erro ao carregar a lista')
    );
  }

  pageChanged(e: PageEvent){
    this.pageIndexCount = e.pageIndex * this.pageSize;
    this.pageSize = e.pageSize;

    this.customerService.getAll(this.pageIndexCount, this.pageSize, this.nameOrder, this.orderByDirection, '').subscribe(
      customers => {
        this.customers = customers.items;
        this.displayedColumns = customers.head;
        this.columnsToDisplay = customers.head.map(x => x.viewPropCamelCase),
        this.length = customers.totalItems;
      },
      error => alert('Erro ao carregar a lista')
    );


  }
  setPageSizeOptions(setPageSizeOptionsInput: string) {
    console.log(setPageSizeOptionsInput)
    this.pageSizeOptions = setPageSizeOptionsInput.split(',').map(str => +str);
  }

  orderBy(name: string) {

    if (this.nameOrder === name) {
      if (this.orderByDirection === 'Ascending') {
        this.orderByDirection = 'Descending';
      } else {
        this.orderByDirection = 'Ascending';
      }
    }

    this.nameOrder = name;
    this.customerService.getAll(this.pageIndexCount, this.pageSize, this.nameOrder, this.orderByDirection, '').subscribe(
      customers => {
        this.customers = customers.items;
        this.displayedColumns = customers.head;
        this.columnsToDisplay = customers.head.map(x => x.viewPropCamelCase)
      },
      error => alert('Erro ao carregar a lista')
    );
  }

}