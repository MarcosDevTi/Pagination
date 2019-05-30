import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private apiPath = 'http://localhost:50005/api/products/v1/public/list';
  constructor(private http: HttpClient) { }

  getAll(): Observable<ProductDw[]> {
    var search = {seach: 'test'}
    let params = new HttpParams()
    .set('seach', 'test')

    return this.http.get(this.apiPath, {params}).pipe(
      catchError(this.handleError),
      map(this.jsonDataToCustomers)
    );
  }

  private jsonDataToCustomers(jsonData): ProductDw[] {
    const customers: ProductDw[] = [];
    jsonData.forEach(element => customers.push(element as ProductDw));
    return customers;
  }

  private handleError(error: any): Observable<any> {
    console.log('Erro na requisição => ', error);
    return throwError(error);
  }
}


export class ProductDw {
  constructor(id: string, name: string) {
    this.id = id;
    this.name = name;
  }
  id: string;
  name: string;
}
