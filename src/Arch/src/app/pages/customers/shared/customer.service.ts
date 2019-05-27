import { Injectable } from '@angular/core';
import { Customer } from './customer.module';
import { Observable, throwError } from 'rxjs';
import {map, catchError, flatMap} from 'rxjs/operators'
import { HttpClient } from '@angular/common/http';
import { Grid } from './grid';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private apiPath = 'http://localhost:50005/api/customers/v1/public/list';
  constructor(private http: HttpClient) { }

  getAll(skip: number, top: number, sortColumn: string, sortDirection: string, search: string): Observable<Grid<Customer>> {
    const url = `${this.apiPath}?skip=${skip}&top=${top}&sortColumn=${sortColumn}&sortDirection=${sortDirection}&search=${search}`;
    return this.http.get(url).pipe(
      catchError(this.handleError),
      map(this.jsonDataToCustomers)
    );
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

  private jsonDataToCustomers(jsonData): Grid<Customer> {
    
    const customers: Customer[] = [];
    jsonData.items.forEach(element => customers.push(element as Customer));
    const head = jsonData.headGrid;
    const totalItems = jsonData.totalNumberOfItems
    const grid = new Grid<Customer>(customers, head, totalItems);
    return grid;
  }
  private jsonDataToCustomer(jsonData: any): Customer {
    return jsonData as Customer;
  }
  private handleError(error: any): Observable<any> {
    console.log('Erro na requisição => ', error);
    return throwError(error);
  }
}
