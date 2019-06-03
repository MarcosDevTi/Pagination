import { Injectable } from '@angular/core';
import { Customer } from './customer.model';
import { Observable, throwError } from 'rxjs';
import {map, catchError, flatMap} from 'rxjs/operators'
import { HttpClient, HttpParams } from '@angular/common/http';
import { Grid } from './grid';
import { CsvParams } from 'src/app/components/csv-config/csv.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private apiPath = 'http://localhost:50005/api/customers';
  constructor(private http: HttpClient) { }

  getAllCsv(csvParams: CsvParams): Observable<Customer[]> {
    const params = this.GetParams(csvParams);
    return this.http.get(this.apiPath + '/v1/public/list/csv', {params}).pipe(
      catchError(this.handleError),
      map(this.jsonDataToCustomers)
    );
  }

  getAll(custParams: CustomerParams): Observable<Grid<Customer>> {
    const params = this.GetParams(custParams);
    return this.http.get(this.apiPath + '/v1/public/list', {params}).pipe(
      catchError(this.handleError),
      map(this.jsonDataToGridCustomers)
    );
  }

  private GetParams(obj): HttpParams {
    let params = new HttpParams();
    Object.keys(obj).forEach((item) => {
      params = params.set(item, obj[item]);
  });
    console.log(params);
    return params;
  }


  getHistory(id: string): Observable<any> {
    const url = `http://localhost:50005/api/customers/v1/public/history?aggregateId=${id}`;
    return this.http.get(url).pipe(
      catchError(this.handleError)
    );
  }

  getById(id: string): Observable<Customer> {
    const url = `${this.apiPath}/${id}`;

    return this.http.get(url).pipe(
      catchError(this.handleError),
      map(this.jsonDataToCustomer)
    )
  }

  create(customer: Customer): Observable<Customer> {
    return this.http.post(this.apiPath, customer).pipe(
      catchError(this.handleError),
      map(this.jsonDataToCustomer)
    );
  }

  updateItemList(objUpdate) {
    const url = 'http://localhost:50005/api/customers/update';
    return this.http.post(url, objUpdate).pipe(
      catchError(this.handleError),
      map(this.jsonDataToCustomer)
    );
  }

  update(customer: Customer): Observable<Customer> {
    const url = `${this.apiPath}/${customer.id}`;
    return this.http.put(url, customer).pipe(
      catchError(this.handleError),
      map(this.jsonDataToCustomer)
    );
  }

  delete(id: string): Observable<any> {
    const url = `${this.apiPath}/${id}`;
    return this.http.delete(url).pipe(
      catchError(this.handleError),
      map(() => null)
    )
  }

  private jsonDataToGridCustomers(jsonData): Grid<Customer> {

    const customers: Customer[] = [];
    jsonData.items.forEach(element => customers.push(element as Customer));
    const head = jsonData.headGrid;
    const totalItems = jsonData.totalNumberOfItems
    const grid = new Grid<Customer>(customers, head, totalItems);
    return grid;
  }

  private jsonDataToCustomers(jsonData): Customer[] {
    const customers: Customer[] = [];
    jsonData.forEach(element => customers.push(element as Customer));
    return customers;
  }
  private jsonDataToCustomer(jsonData: any): Customer {
    return jsonData as Customer;
  }
  private handleError(error: any): Observable<any> {
    console.log('Erro na requisição => ', error);
    return throwError(error);
  }
}

export interface CustomerParams {
  skip: number;
  top: number;
  sortColumn: string;
  sortDirection: string;
  search: string;
}
