import { Component, ViewChild, OnInit } from '@angular/core';
import { MatTableDataSource, MatSort, PageEvent } from '@angular/material';
import { CustomerService, CustomerParams } from '../shared/customer.service';
import { Customer } from '../shared/customer.model';

import {animate, state, style, transition, trigger} from '@angular/animations';
import { PeriodicElement } from '../customer-list/customer-list.component';


@Component({
  selector: 'app-customers2',
  templateUrl: './customers2.component.html',
  styleUrls: ['./customers2.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0', display: 'none'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class Customers2Component implements OnInit {

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
  
    customerParams: CustomerParams = {
      skip: this.pageIndexCount,
      top:  this.pageSize,
      sortColumn: this.nameOrder,
      sortDirection: this.orderByDirection,
      search: ''
    };
  
    pageEvent: PageEvent;
  
    ngOnInit(): void {
      this.updateList();
    }

    saveCost(id, column, e: Event){
      this.customerService.updateItemList(id, e, column).subscribe(_ => console.log(_))
      console.log('event', e);
      console.log('id', id)
      console.log('column', column)
        }
  
    pageChanged(e: PageEvent){
      this.customerParams.skip = e.pageIndex * this.pageSize;
      this.customerParams.top = e.pageSize;
  
      this.updateList();
    }
  
    update(el: Customer, comment: string) {
      if (comment == null) { return; }
      const copy = this.customers.slice()
      el.firstName = comment;
    }
  
    orderBy(name: string) {
  
      if (this.customerParams.sortColumn === name) {
        if (this.customerParams.sortDirection === 'Ascending') {
          this.customerParams.sortDirection = 'Descending';
        } else {
          this.customerParams.sortDirection = 'Ascending';
        }
      }
  
      this.customerParams.sortColumn = name;
      this.updateList();
    }
  
    updateList() {
        this.customerService.getAll(this.customerParams).subscribe(
          customers => {
            this.customers = customers.items;
            this.displayedColumns = customers.head;
            this.columnsToDisplay = customers.head.map(x => x.viewPropCamelCase);
            this.length = customers.totalItems;
          },
          error => alert('Erro ao carregar a lista')
        );
      }
}
