import { Component, ViewChild, OnInit } from '@angular/core';
import { MatTableDataSource, MatSort, PageEvent, MatDialog } from '@angular/material';
import { CustomerService, CustomerParams } from '../shared/customer.service';
import { Customer } from '../shared/customer.model';

import {animate, state, style, transition, trigger} from '@angular/animations';
import { PeriodicElement } from '../customer-list/customer-list.component';
import { AngularCsv } from 'angular7-csv/dist/Angular-csv';
import { CsvConfigComponent, PropertyForSave } from 'src/app/components/csv-config/csv-config.component';
import { CsvParams } from 'src/app/components/csv-config/csv.model';


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

  constructor(
    private customerService: CustomerService,
    public dialog: MatDialog) { }
    data: any[];
    customers: Customer[] = [];

    displayedColumns: any[];
    columnsToDisplay: string[];
    dataSource = this.customers;
    expandedElement: PeriodicElement | null;
    orderByDirection = 'Ascending';
    nameOrder = 'Name';
    modelAssembly = '';
    viewModelAssembly = '';

    length = 100;
    
    pageSizeOptions: number[] = [5, 10, 25, 100];
    pageSize = 10;
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

    openDialog(): void {
      const resultData = this.displayedColumns.map(_ => new PropertyForSave(_.displayProp, _.modelProp, true, false));

      const dialogRef = this.dialog.open(CsvConfigComponent, {
        width: '600px',

        data: {itens: resultData, resultOrder: resultData[0].modelProp}
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          const itens: PropertyForSave[] = result.itens;
          const csvParams: CsvParams = {
            properties: itens.filter(_ => _.isSave).map(m => m.modelProp),
            order: result.resultOrder,
            modelAssemblyFullName: this.modelAssembly,
            viewModelAssemblyFullName: this.viewModelAssembly
           };
          this.customerService.getAllCsv(csvParams)
            .subscribe(_ => new AngularCsv(_, 'Customers'));
        }
      });
    }

    saveCost(id, column, e: Event){
      const objUpdateList = {
        id,
        key: column.modelProp,
        value: e,
        AssemblyModel: column.assemblyModel
      }
      
      this.customerService.updateItemList(objUpdateList).subscribe(_ => _)
        }

    pageChanged(e: PageEvent){
      this.pageSize = e.pageSize;
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
            this.modelAssembly = customers.head[0].assemblyModel;
            this.viewModelAssembly = customers.head[0].assemblyViewModel;

            let columns = customers.head.filter(_ => _.displayable).map(_ => _.viewPropCamelCase)
            this.customers = customers.items;
            this.displayedColumns = customers.head;
            this.columnsToDisplay = columns;
            this.length = customers.totalItems;
          },
          error => alert('Erro ao carregar a lista')
        );
      }

      SaveCsv() {
        const responseCsv = new AngularCsv(this.customers, 'My Report');
      }
}
